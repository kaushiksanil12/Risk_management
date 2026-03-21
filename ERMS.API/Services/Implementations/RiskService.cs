using ERMS.API.Helpers;
using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;
using ERMS.API.Services.Interfaces;

namespace ERMS.API.Services.Implementations
{
    public class RiskService : IRiskService
    {
        private readonly IRiskRepository _riskRepo;
        private readonly IEmailService _emailService;
        private readonly ILogger<RiskService> _logger;

        public RiskService(IRiskRepository riskRepo, IEmailService emailService, ILogger<RiskService> logger)
        {
            _riskRepo = riskRepo;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ApiResponse<List<RiskResponse>>> SearchAsync(int userId, string adminFlag, string? buId, string? fyId, int? riskCatId, string? status, string? search)
        {
            var result = await _riskRepo.SearchAsync(userId, adminFlag, buId, fyId, riskCatId, status, search);
            return ApiResponse<List<RiskResponse>>.Ok(result.ToList());
        }

        public async Task<ApiResponse<RiskResponse>> GetByIdAsync(int riskId)
        {
            var risk = await _riskRepo.GetByIdAsync(riskId);
            if (risk == null) return ApiResponse<RiskResponse>.NotFound("Risk not found.");

            var history = await _riskRepo.GetHistoryByRiskAsync(riskId);
            risk.History = history.ToList();

            return ApiResponse<RiskResponse>.Ok(risk);
        }

        public async Task<ApiResponse<int>> CreateAsync(RiskRequest request, int createdBy)
        {
            if (string.IsNullOrWhiteSpace(request.BUId))
                return ApiResponse<int>.Fail("Business Unit is required.");
            if (string.IsNullOrWhiteSpace(request.FYId))
                return ApiResponse<int>.Fail("Fiscal Year is required.");
            if (request.RiskCatId <= 0)
                return ApiResponse<int>.Fail("Risk Category is required.");
            if (request.RiskSubCatId <= 0)
                return ApiResponse<int>.Fail("Risk Sub-Category is required. Please select a Category first, then select a Sub-Category.");
            if (request.FunctionId <= 0)
                return ApiResponse<int>.Fail("Function is required.");
            if (string.IsNullOrWhiteSpace(request.RiskTitle))
                return ApiResponse<int>.Fail("Risk title is required.");
            if (string.IsNullOrWhiteSpace(request.Description) || request.Description.Length < 20)
                return ApiResponse<int>.Fail("Description must be at least 20 characters.");

            var newId = await _riskRepo.InsertAsync(request, createdBy);
            await _riskRepo.InsertHistoryAsync(newId, null, ApiConstants.RiskStatuses.Draft, "Created", "", createdBy);
            return ApiResponse<int>.Ok(newId, "Risk created successfully.");
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int riskId, RiskRequest request, int updatedBy)
        {
            var existing = await _riskRepo.GetByIdAsync(riskId);
            if (existing == null) return ApiResponse<RiskResponse>.NotFound("Risk not found.") as dynamic;

            if (existing.Status != ApiConstants.RiskStatuses.Draft && existing.Status != ApiConstants.RiskStatuses.RevisionRequired)
                return ApiResponse<bool>.Fail("Risk can only be edited in Draft or Revision Required status.");

            if (request.RiskCatId <= 0)
                return ApiResponse<bool>.Fail("Risk Category is required.");
            if (request.RiskSubCatId <= 0)
                return ApiResponse<bool>.Fail("Risk Sub-Category is required.");
            if (request.FunctionId <= 0)
                return ApiResponse<bool>.Fail("Function is required.");
            if (string.IsNullOrWhiteSpace(request.RiskTitle))
                return ApiResponse<bool>.Fail("Risk title is required.");

            await _riskRepo.UpdateAsync(riskId, request, updatedBy);
            await _riskRepo.InsertHistoryAsync(riskId, existing.Status, existing.Status, "Edited", "", updatedBy);
            return ApiResponse<bool>.Ok(true, "Risk updated successfully.");
        }

        public async Task<ApiResponse<bool>> SubmitAsync(int riskId, int userId)
        {
            var risk = await _riskRepo.GetByIdAsync(riskId);
            if (risk == null) return ApiResponse<bool>.NotFound("Risk not found.");

            if (risk.Status != ApiConstants.RiskStatuses.Draft && risk.Status != ApiConstants.RiskStatuses.RevisionRequired)
                return ApiResponse<bool>.Fail("Risk can only be submitted from Draft or Revision Required status.");

            var oldStatus = risk.Status;
            await _riskRepo.UpdateStatusAsync(riskId, ApiConstants.RiskStatuses.Submitted, "", userId);
            await _riskRepo.InsertHistoryAsync(riskId, oldStatus, ApiConstants.RiskStatuses.Submitted, "Submitted for Review", "", userId);

            // Fire-and-forget: email all Champions of this BU
            var recipients = await _riskRepo.GetEmailRecipientsAsync(riskId);
            if (recipients.Owner != null && recipients.Champions.Any())
            {
                foreach (var champion in recipients.Champions)
                {
                    _ = Task.Run(() => _emailService.SendRiskSubmittedAsync(
                        champion.ChampionEmail,
                        champion.ChampionName,
                        recipients.Owner.RiskTitle,
                        riskId,
                        recipients.Owner.BUName
                    ));
                }
            }
            else
            {
                _logger.LogWarning("No active champions found for RiskId {RiskId}", riskId);
            }

            return ApiResponse<bool>.Ok(true, "Risk submitted for review.");
        }

        public async Task<ApiResponse<bool>> ReviewAsync(int riskId, RiskReviewRequest request, int userId)
        {
            var risk = await _riskRepo.GetByIdAsync(riskId);
            if (risk == null) return ApiResponse<bool>.NotFound("Risk not found.");

            string newStatus;
            switch (request.Action)
            {
                case "Approve":
                    newStatus = ApiConstants.RiskStatuses.Approved;
                    break;
                case "Reject":
                    if (string.IsNullOrWhiteSpace(request.Remarks))
                        return ApiResponse<bool>.Fail("Remarks are required for rejection.");
                    newStatus = ApiConstants.RiskStatuses.Rejected;
                    break;
                case "SendBack":
                    if (string.IsNullOrWhiteSpace(request.Remarks))
                        return ApiResponse<bool>.Fail("Remarks are required when sending back for revision.");
                    newStatus = ApiConstants.RiskStatuses.RevisionRequired;
                    break;
                default:
                    return ApiResponse<bool>.Fail("Invalid review action. Must be Approve, Reject, or SendBack.");
            }

            var oldStatus = risk.Status;
            await _riskRepo.UpdateStatusAsync(riskId, newStatus, request.Remarks, userId);
            await _riskRepo.InsertHistoryAsync(riskId, oldStatus, newStatus, request.Action, request.Remarks, userId);

            // Fire-and-forget: email the Risk Owner
            var recipients = await _riskRepo.GetEmailRecipientsAsync(riskId);
            var owner = recipients.Owner;
            if (owner != null)
            {
                _ = request.Action switch
                {
                    "Approve"  => Task.Run(() => _emailService.SendRiskApprovedAsync(
                                      owner.OwnerEmail, owner.OwnerName,
                                      owner.RiskTitle,  riskId)),

                    "Reject"   => Task.Run(() => _emailService.SendRiskRejectedAsync(
                                      owner.OwnerEmail, owner.OwnerName,
                                      owner.RiskTitle,  riskId, request.Remarks ?? "")),

                    "SendBack" => Task.Run(() => _emailService.SendRiskSentBackAsync(
                                      owner.OwnerEmail, owner.OwnerName,
                                      owner.RiskTitle,  riskId, request.Remarks ?? "")),

                    _          => Task.CompletedTask
                };
            }
            else
            {
                _logger.LogWarning("No owner found for RiskId {RiskId} — skipping email", riskId);
            }

            return ApiResponse<bool>.Ok(true, $"Risk {request.Action.ToLower()}d successfully.");
        }

        public async Task<ApiResponse<bool>> CloseAsync(int riskId, int userId)
        {
            var risk = await _riskRepo.GetByIdAsync(riskId);
            if (risk == null) return ApiResponse<bool>.NotFound("Risk not found.");

            var oldStatus = risk.Status;
            await _riskRepo.UpdateStatusAsync(riskId, ApiConstants.RiskStatuses.Closed, "", userId);
            await _riskRepo.InsertHistoryAsync(riskId, oldStatus, ApiConstants.RiskStatuses.Closed, "Closed", "", userId);

            // Fire-and-forget: email the Risk Owner
            var recipients = await _riskRepo.GetEmailRecipientsAsync(riskId);
            var owner = recipients.Owner;
            if (owner != null)
            {
                _ = Task.Run(() => _emailService.SendRiskClosedAsync(
                    owner.OwnerEmail,
                    owner.OwnerName,
                    owner.RiskTitle,
                    riskId
                ));
            }

            return ApiResponse<bool>.Ok(true, "Risk closed successfully.");
        }
    }
}

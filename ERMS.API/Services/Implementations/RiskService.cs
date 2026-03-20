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

        public RiskService(IRiskRepository riskRepo)
        {
            _riskRepo = riskRepo;
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
            if (string.IsNullOrWhiteSpace(request.RiskTitle))
                return ApiResponse<int>.Fail("Risk title is required.");
            if (string.IsNullOrWhiteSpace(request.Description) || request.Description.Length < 20)
                return ApiResponse<int>.Fail("Description must be at least 20 characters.");
            if (string.IsNullOrWhiteSpace(request.BUId))
                return ApiResponse<int>.Fail("Business Unit is required.");

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
            return ApiResponse<bool>.Ok(true, $"Risk {request.Action.ToLower()}d successfully.");
        }

        public async Task<ApiResponse<bool>> CloseAsync(int riskId, int userId)
        {
            var risk = await _riskRepo.GetByIdAsync(riskId);
            if (risk == null) return ApiResponse<bool>.NotFound("Risk not found.");

            var oldStatus = risk.Status;
            await _riskRepo.UpdateStatusAsync(riskId, ApiConstants.RiskStatuses.Closed, "", userId);
            await _riskRepo.InsertHistoryAsync(riskId, oldStatus, ApiConstants.RiskStatuses.Closed, "Closed", "", userId);
            return ApiResponse<bool>.Ok(true, "Risk closed successfully.");
        }
    }
}

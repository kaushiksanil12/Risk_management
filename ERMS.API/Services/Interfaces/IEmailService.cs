namespace ERMS.API.Services.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Called when Risk Owner submits a risk for review.
        /// Recipient: Risk Champion(s) of that BU.
        /// </summary>
        Task SendRiskSubmittedAsync(string toEmail, string toName, string riskTitle,
                                    int riskId, string buName);

        /// <summary>
        /// Called when Champion approves a risk.
        /// Recipient: Risk Owner (CreatedBy).
        /// </summary>
        Task SendRiskApprovedAsync(string toEmail, string toName, string riskTitle,
                                   int riskId);

        /// <summary>
        /// Called when Champion rejects a risk.
        /// Recipient: Risk Owner (CreatedBy).
        /// </summary>
        Task SendRiskRejectedAsync(string toEmail, string toName, string riskTitle,
                                   int riskId, string remarks);

        /// <summary>
        /// Called when Champion sends a risk back for revision.
        /// Recipient: Risk Owner (CreatedBy).
        /// </summary>
        Task SendRiskSentBackAsync(string toEmail, string toName, string riskTitle,
                                   int riskId, string remarks);

        /// <summary>
        /// Called when a risk is closed.
        /// Recipient: Risk Owner (CreatedBy).
        /// </summary>
        Task SendRiskClosedAsync(string toEmail, string toName, string riskTitle,
                                 int riskId);
    }
}

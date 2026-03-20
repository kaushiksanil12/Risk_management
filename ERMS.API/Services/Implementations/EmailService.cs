using System.Net;
using System.Net.Mail;
using ERMS.API.Models.Email;
using ERMS.API.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace ERMS.API.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtp;
        private readonly string _webBaseUrl;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SmtpSettings> smtpOptions, IConfiguration configuration,
                            ILogger<EmailService> logger)
        {
            _smtp = smtpOptions.Value;
            _webBaseUrl = configuration["WebBaseUrl"] ?? "https://localhost:7002";
            _logger = logger;
        }

        public async Task SendRiskSubmittedAsync(string toEmail, string toName, string riskTitle,
                                                  int riskId, string buName)
        {
            var subject = "[ERMS] New Risk Submitted for Review";
            var bodyContent = $@"
                <p>Hi {Encode(toName)},</p>
                <p>A new risk has been submitted for your review.</p>
                <div class=""info-row""><span>Risk Title:</span> {Encode(riskTitle)}</div>
                <div class=""info-row""><span>Business Unit:</span> {Encode(buName)}</div>
                <div style=""margin:12px 0"">
                    <span class=""badge badge-submitted"">SUBMITTED</span>
                </div>
                <p>Please log in to ERMS to review and take action.</p>
                <a href=""{_webBaseUrl}/Risk/Detail/{riskId}"" class=""btn"">View Risk</a>";

            await SendAsync(toEmail, toName, subject, WrapHtml(bodyContent));
        }

        public async Task SendRiskApprovedAsync(string toEmail, string toName, string riskTitle,
                                                 int riskId)
        {
            var subject = "[ERMS] Your Risk Has Been Approved";
            var bodyContent = $@"
                <p>Hi {Encode(toName)},</p>
                <p>Great news! Your submitted risk has been approved.</p>
                <div class=""info-row""><span>Risk Title:</span> {Encode(riskTitle)}</div>
                <div style=""margin:12px 0"">
                    <span class=""badge badge-approved"">APPROVED</span>
                </div>
                <a href=""{_webBaseUrl}/Risk/Detail/{riskId}"" class=""btn"">View Risk</a>";

            await SendAsync(toEmail, toName, subject, WrapHtml(bodyContent));
        }

        public async Task SendRiskRejectedAsync(string toEmail, string toName, string riskTitle,
                                                 int riskId, string remarks)
        {
            var subject = "[ERMS] Your Risk Has Been Rejected";
            var bodyContent = $@"
                <p>Hi {Encode(toName)},</p>
                <p>Your submitted risk has been rejected by the Risk Champion.</p>
                <div class=""info-row""><span>Risk Title:</span> {Encode(riskTitle)}</div>
                <div style=""margin:12px 0"">
                    <span class=""badge badge-rejected"">REJECTED</span>
                </div>
                <div class=""remarks-box"">
                    <strong>Reviewer Remarks:</strong><br/>
                    {Encode(remarks)}
                </div>
                <a href=""{_webBaseUrl}/Risk/Detail/{riskId}"" class=""btn"">View Risk</a>";

            await SendAsync(toEmail, toName, subject, WrapHtml(bodyContent));
        }

        public async Task SendRiskSentBackAsync(string toEmail, string toName, string riskTitle,
                                                 int riskId, string remarks)
        {
            var subject = "[ERMS] Your Risk Requires Revision";
            var bodyContent = $@"
                <p>Hi {Encode(toName)},</p>
                <p>Your submitted risk has been sent back for revision.</p>
                <div class=""info-row""><span>Risk Title:</span> {Encode(riskTitle)}</div>
                <div style=""margin:12px 0"">
                    <span class=""badge badge-sentback"">REVISION REQUIRED</span>
                </div>
                <div class=""remarks-box"">
                    <strong>Reviewer Remarks:</strong><br/>
                    {Encode(remarks)}
                </div>
                <p>Please update your risk and resubmit for review.</p>
                <a href=""{_webBaseUrl}/Risk/Detail/{riskId}"" class=""btn"">View Risk</a>";

            await SendAsync(toEmail, toName, subject, WrapHtml(bodyContent));
        }

        public async Task SendRiskClosedAsync(string toEmail, string toName, string riskTitle,
                                               int riskId)
        {
            var subject = "[ERMS] Risk Has Been Closed";
            var bodyContent = $@"
                <p>Hi {Encode(toName)},</p>
                <p>Your risk has been marked as closed.</p>
                <div class=""info-row""><span>Risk Title:</span> {Encode(riskTitle)}</div>
                <div style=""margin:12px 0"">
                    <span class=""badge badge-closed"">CLOSED</span>
                </div>
                <a href=""{_webBaseUrl}/Risk/Detail/{riskId}"" class=""btn"">View Risk</a>";

            await SendAsync(toEmail, toName, subject, WrapHtml(bodyContent));
        }

        // ────────────────────────────────────────────────────────────────
        //  Private helpers
        // ────────────────────────────────────────────────────────────────

        private async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(toEmail))
                {
                    _logger.LogWarning("Email skipped — recipient email is empty for subject: {Subject}", subject);
                    return;
                }

                using var smtp = new SmtpClient(_smtp.Host, _smtp.Port)
                {
                    Credentials = new NetworkCredential(_smtp.Username, _smtp.Password),
                    EnableSsl = true
                };

                var message = new MailMessage
                {
                    From = new MailAddress(_smtp.From),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(toEmail, toName));

                await smtp.SendMailAsync(message);
                _logger.LogInformation("Email sent to {Email} — {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            }
        }

        private static string Encode(string? value)
        {
            return System.Web.HttpUtility.HtmlEncode(value ?? string.Empty);
        }

        private static string WrapHtml(string bodyContent)
        {
            return $@"<!DOCTYPE html>
<html>
<head>
  <style>
    body {{ font-family: Inter, Arial, sans-serif; background:#f2f3f4; margin:0; padding:0; }}
    .container {{ max-width:600px; margin:32px auto; background:#ffffff;
                 border-radius:12px; overflow:hidden;
                 box-shadow:0 2px 12px rgba(0,0,0,0.08); }}
    .header    {{ background:#0D2B55; padding:28px 32px; }}
    .header h1 {{ color:#ffffff; margin:0; font-size:20px; }}
    .header p  {{ color:#AED6F1; margin:4px 0 0; font-size:13px; }}
    .body      {{ padding:32px; color:#2C3E50; line-height:1.6; }}
    .badge     {{ display:inline-block; padding:4px 14px; border-radius:50px;
                 font-size:12px; font-weight:600; margin:8px 0; }}
    .badge-submitted {{ background:#D6EAF8; color:#1A5276; }}
    .badge-approved  {{ background:#D1F2EB; color:#148F77; }}
    .badge-rejected  {{ background:#FADBD8; color:#C0392B; }}
    .badge-sentback  {{ background:#FDEBD0; color:#E67E22; }}
    .badge-closed    {{ background:#E5E7E9; color:#2C3E50; }}
    .info-row  {{ margin:8px 0; font-size:14px; }}
    .info-row span {{ font-weight:600; }}
    .remarks-box {{ background:#F8F9FA; border-left:4px solid #E67E22;
                   padding:12px 16px; border-radius:4px; margin:16px 0;
                   font-size:14px; color:#2C3E50; }}
    .btn {{ display:inline-block; padding:12px 28px; background:#2E86C1;
           color:#ffffff !important; text-decoration:none; border-radius:50px;
           font-weight:600; font-size:14px; margin-top:20px; }}
    .footer {{ background:#F2F3F4; padding:16px 32px; font-size:12px;
              color:#7F8C8D; text-align:center; }}
  </style>
</head>
<body>
  <div class=""container"">
    <div class=""header"">
      <h1>Enterprise Risk Management System</h1>
      <p>Automated Notification</p>
    </div>
    <div class=""body"">
      {bodyContent}
    </div>
    <div class=""footer"">
      This is an automated message from ERMS. Please do not reply to this email.
    </div>
  </div>
</body>
</html>";
        }
    }
}

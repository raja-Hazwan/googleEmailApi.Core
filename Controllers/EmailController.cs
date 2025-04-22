using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MimeKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GoogleEmailApi.Core.Services;
using GoogleEmailApi.Core.Models;

namespace GoogleEmailApi.Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private const string ApplicationName = "GoogleEmailApi.Core";
        private readonly CredentialService _credService;

        public EmailController(CredentialService credService)
        {
            _credService = credService;
        }

        private async Task<UserCredential> BuildCredentialAsync()
        {
            var creds = _credService.Get("gmail");
            if (creds == null)
                throw new InvalidOperationException("Gmail credentials not found in MongoDB.");

            var token = new TokenResponse { RefreshToken = creds.RefreshToken };
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = creds.ClientId,
                    ClientSecret = creds.ClientSecret
                },
                Scopes = new[] { GmailService.Scope.GmailSend },
                DataStore = new FileDataStore("Google.Api.TokenStore")
            });

            var userCred = new UserCredential(flow, "user", token);
            await userCred.RefreshTokenAsync(CancellationToken.None);
            return userCred;
        }

        private async Task<GmailService> GetGmailServiceAsync()
        {
            var credential = await BuildCredentialAsync();
            return new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail(
            [FromForm] string to,
            [FromForm] string subject,
            [FromForm] string body,
            [FromForm] IFormFile[]? attachments)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("me"));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder { TextBody = body };

            if (attachments != null)
            {
                foreach (var file in attachments)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    builder.Attachments.Add(file.FileName, ms.ToArray());
                }
            }

            message.Body = builder.ToMessageBody();

            var service = await GetGmailServiceAsync();
            using var outStream = new MemoryStream();
            message.WriteTo(outStream);

            var raw = Convert.ToBase64String(outStream.ToArray())
                                .Replace('+', '-')
                                .Replace('/', '_')
                                .TrimEnd('=');

            await service.Users.Messages
                         .Send(new Google.Apis.Gmail.v1.Data.Message { Raw = raw }, "me")
                         .ExecuteAsync();

            return Ok(new { status = "Sent" });
        }
    }
}

using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AppointmentManager.Presentation.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        [Obsolete("This constructor is using ISystemClock, which must be replaced by TimeProvider")]
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger,
            UrlEncoder encoder, 
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out var authorizationHeader);
            if (authorizationHeader == null || string.IsNullOrEmpty(authorizationHeader.Parameter))
            {
                throw new AuthenticationException("Authorization Header not found");
            }

            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authorizationHeader.Parameter)).Split(':');
            var username = credentials.First();
            var password = credentials.Last();
            if (CredentialsAreNotValid(username, password))
            {
                throw new AuthenticationException("Invalid credentials");
            }

            var claims = new[] { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        private static bool CredentialsAreNotValid(string username, string password)
        {
            var appointmentManagerUserName = Environment.GetEnvironmentVariable("APPOINTMENT_MANAGER_USERNAME");
            var appointmentManagerPassword = Environment.GetEnvironmentVariable("APPOINTMENT_MANAGER_PASSWORD");
            return !username.Equals(appointmentManagerUserName) && !password.Equals(appointmentManagerPassword);
        }
    }
}

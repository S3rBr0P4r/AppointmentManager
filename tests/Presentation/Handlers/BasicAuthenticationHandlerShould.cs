using System.Security.Authentication;
using System.Text.Encodings.Web;
using AppointmentManager.Presentation.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;

namespace AppointmentManager.Presentation.Tests.Handlers
{
    public class BasicAuthenticationHandlerShould
    {
        [Fact]
        public async Task ThrowAuthenticationExceptionWhenAuthenticationHeaderIsNotFound()
        {
            // Arrange
            var optionsMonitor = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            optionsMonitor.Setup(x => x.Get(It.IsAny<string>())).Returns(new AuthenticationSchemeOptions());
            var logger = new Mock<ILogger<BasicAuthenticationHandler>>();
            var loggerFactory = new Mock<ILoggerFactory>();
            loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

            var sut = new BasicAuthenticationHandler(optionsMonitor.Object, loggerFactory.Object, UrlEncoder.Default);
            await sut.InitializeAsync(new AuthenticationScheme("SchemeName", null, typeof(BasicAuthenticationHandler)), new DefaultHttpContext());

            // Act
            var exception = Assert.ThrowsAsync<AuthenticationException>(async () => await sut.AuthenticateAsync());

            // Assert
            Assert.Equal("Authorization Header not found", exception.Result.Message);
        }

        [Fact]
        public async Task ThrowAuthenticationExceptionWhenAuthenticationHeaderHasNotParameter()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add(HeaderNames.Authorization, new StringValues("Basic"));
            var optionsMonitor = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            optionsMonitor.Setup(x => x.Get(It.IsAny<string>())).Returns(new AuthenticationSchemeOptions());
            var logger = new Mock<ILogger<BasicAuthenticationHandler>>();
            var loggerFactory = new Mock<ILoggerFactory>();
            loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

            var sut = new BasicAuthenticationHandler(optionsMonitor.Object, loggerFactory.Object, UrlEncoder.Default);
            await sut.InitializeAsync(new AuthenticationScheme("SchemeName", null, typeof(BasicAuthenticationHandler)), context);

            // Act
            var exception = Assert.ThrowsAsync<AuthenticationException>(async () => await sut.AuthenticateAsync());

            // Assert
            Assert.Equal("Authorization Header not found", exception.Result.Message);
        }

        [Fact]
        public async Task ThrowAuthenticationExceptionWhenCredentialsAreNotValid()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add(HeaderNames.Authorization, new StringValues("Basic aW52YWxpZDpjcmVkZW50aWFscw=="));
            var optionsMonitor = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            optionsMonitor.Setup(x => x.Get(It.IsAny<string>())).Returns(new AuthenticationSchemeOptions());
            var logger = new Mock<ILogger<BasicAuthenticationHandler>>();
            var loggerFactory = new Mock<ILoggerFactory>();
            loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

            var sut = new BasicAuthenticationHandler(optionsMonitor.Object, loggerFactory.Object, UrlEncoder.Default);
            await sut.InitializeAsync(new AuthenticationScheme("SchemeName", null, typeof(BasicAuthenticationHandler)), context);

            // Act
            var exception = Assert.ThrowsAsync<AuthenticationException>(async () => await sut.AuthenticateAsync());

            // Assert
            Assert.Equal("Invalid credentials", exception.Result.Message);
        }

        [Fact]
        public async Task ReturnSucceededWhenCredentialsAreValid()
        {
            // Arrange
            Environment.SetEnvironmentVariable("APPOINTMENT_MANAGER_USERNAME", "test");
            Environment.SetEnvironmentVariable("APPOINTMENT_MANAGER_PASSWORD", "test");
            var context = new DefaultHttpContext();
            context.Request.Headers.Add(HeaderNames.Authorization, new StringValues("Basic dGVzdDp0ZXN0"));
            var optionsMonitor = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            optionsMonitor.Setup(x => x.Get(It.IsAny<string>())).Returns(new AuthenticationSchemeOptions());
            var logger = new Mock<ILogger<BasicAuthenticationHandler>>();
            var loggerFactory = new Mock<ILoggerFactory>();
            loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

            var sut = new BasicAuthenticationHandler(optionsMonitor.Object, loggerFactory.Object, UrlEncoder.Default);
            await sut.InitializeAsync(new AuthenticationScheme("SchemeName", null, typeof(BasicAuthenticationHandler)), context);

            // Act
            var authenticateResult = await sut.AuthenticateAsync();

            // Assert
            Assert.True(authenticateResult.Succeeded);
        }
    }
}

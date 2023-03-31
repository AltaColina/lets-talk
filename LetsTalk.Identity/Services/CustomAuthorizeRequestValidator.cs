using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.ResponseHandling;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Authentication;

namespace LetsTalk.Services;

internal sealed class CustomAuthorizeRequestValidator : ICustomAuthorizeRequestValidator
{
    public Task ValidateAsync(CustomAuthorizeRequestValidationContext context)
    {
        var prompt = context.Result.ValidatedRequest.Raw.Get("prompt");
        if (!string.IsNullOrWhiteSpace(prompt) && prompt.Equals("create", StringComparison.OrdinalIgnoreCase))
            context.Result.ValidatedRequest.PromptModes = new[] { "create" };

        return Task.CompletedTask;
    }
}
internal sealed class CustomAuthorizeInteractionResponseGenerator : AuthorizeInteractionResponseGenerator
{
    public CustomAuthorizeInteractionResponseGenerator(
        IdentityServerOptions options,
        ISystemClock clock,
        ILogger<AuthorizeInteractionResponseGenerator> logger,
        IConsentService consent,
        IProfileService profile)
     : base(options, clock, logger, consent, profile)
    {
    }

    public override async Task<InteractionResponse> ProcessInteractionAsync(ValidatedAuthorizeRequest request, ConsentResponse? consent = null)
    {
        if (!request.PromptModes.Contains("create"))
            return await base.ProcessInteractionAsync(request, consent);

        request.Raw.Remove("prompt");
        var response = new InteractionResponse
        {
            RedirectUrl = "/Account/Register"
        };

        return response;
    }
}

using FluentValidation;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using AppointmentBooking.Application.DTOs;
using AppointmentBooking.Application.Common;

namespace AppointmentBooking.Application.Validation;

public class AppointmentBookingRequestValidator : AbstractValidator<AppointmentBookingRequest>
{
    public AppointmentBookingRequestValidator(IConfiguration configuration)
    {
        var languageRegexPatters = configuration.GetSection(Constants.SupportedLanguagePatternKey).Value;
        RuleFor(r => r.Language)
            .Must(lan => IsSupported(lan, languageRegexPatters))
            .WithMessage("Language is not supported");

        var ratingRegexPattern = configuration.GetSection(Constants.SupportedRatingPatternKey).Value;
        RuleFor(r => r.Rating)
            .Must(rat => IsSupported(rat, ratingRegexPattern))
            .WithMessage("Rating is not supported");

        RuleFor(r => r.Products).NotEmpty();
        var productRegexPattern = configuration.GetSection(Constants.SupportedProductPatternKey).Value;
        RuleForEach(r => r.Products)
            .Must(pro => IsSupported(pro, productRegexPattern))
            .WithMessage("Product is not supported");

        RuleFor(r => r.Date).NotEmpty();
    }

    private static bool IsSupported(string input, string? pattern)
    {
        return string.IsNullOrEmpty(pattern) || Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
    }
}
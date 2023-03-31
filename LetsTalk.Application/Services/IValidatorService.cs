using FluentValidation.Results;

namespace LetsTalk.Services;

public interface IValidatorService<T>
{
    public Task<ValidationResult> ValidateAsync(T value, CancellationToken cancellationToken = default);
}
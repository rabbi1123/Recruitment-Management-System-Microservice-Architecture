using Common.Platform.Domain.Abstractions;
namespace Organization.Application.Abstractions.Validation
{
    public interface IRequestValidator<TRequest>
    {
        Result Validate(TRequest request);
    }
}

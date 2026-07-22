using Common.Platform.Domain.Abstractions;
namespace Recruitment.Application.Abstractions.Validation
{
    public interface IRequestValidator<TRequest>
    {
        Result Validate(TRequest request);
    }
}

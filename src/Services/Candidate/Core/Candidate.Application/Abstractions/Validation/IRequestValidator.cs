using Common.Platform.Domain.Abstractions;
namespace Candidate.Application.Abstractions.Validation
{
    public interface IRequestValidator<TRequest>
    {
        Result Validate(TRequest request);
    }
}

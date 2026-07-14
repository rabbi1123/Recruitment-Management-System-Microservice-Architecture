using Common.Platform.Domain.Abstractions;
namespace Job.Application.Abstractions.Validation
{
    public interface IRequestValidator<TRequest>
    {
        Result Validate(TRequest request);
    }
}

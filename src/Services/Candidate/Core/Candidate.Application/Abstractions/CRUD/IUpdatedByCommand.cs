namespace Candidate.Application.Abstractions.CRUD
{
    public interface IUpdatedByCommand
    {
        string? UpdatedBy { get; set; }
    }
}

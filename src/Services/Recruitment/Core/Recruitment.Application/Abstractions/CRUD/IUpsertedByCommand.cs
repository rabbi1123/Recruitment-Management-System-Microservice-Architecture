namespace Recruitment.Application.Abstractions.CRUD
{
    public interface IUpsertedByCommand
    {
        string? UpsertedBy { get; set; }
    }
}

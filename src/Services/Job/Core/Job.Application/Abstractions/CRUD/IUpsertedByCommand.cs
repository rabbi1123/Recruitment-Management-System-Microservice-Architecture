namespace Job.Application.Abstractions.CRUD
{
    public interface IUpsertedByCommand
    {
        string? UpsertedBy { get; set; }
    }
}

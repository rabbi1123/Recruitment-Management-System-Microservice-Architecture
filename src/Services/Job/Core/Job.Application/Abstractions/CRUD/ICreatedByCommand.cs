namespace Job.Application.Abstractions.CRUD
{
    public interface ICreatedByCommand
    {
        string? CreatedBy { get; set; }
    }
}

namespace Organization.Application.Abstractions.CRUD
{
    public interface ICreatedByCommand
    {
        string? CreatedBy { get; set; }
    }
}

namespace Job.Domain.Abstractions
{
    public interface IValueObject<TSelf>
    {
        string Value { get; }
        static abstract (bool ok, string? error) Validate(string value);
        static abstract TSelf FromString(string s);
    }
}

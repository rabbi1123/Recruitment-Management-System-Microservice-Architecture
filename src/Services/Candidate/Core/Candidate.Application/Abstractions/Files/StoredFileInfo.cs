namespace Candidate.Application.Abstractions.Files
{
    public sealed class StoredFileInfo
    {
        public required string RelativePath { get; init; } // e.g. "employees/18/nid/abcd1234.pdf"
        public required string FileName { get; init; }     // original or normalized
        public required string Extension { get; init; }    // ".pdf"
        public string? ContentType { get; init; }          // if you infer/persist it
    }
}

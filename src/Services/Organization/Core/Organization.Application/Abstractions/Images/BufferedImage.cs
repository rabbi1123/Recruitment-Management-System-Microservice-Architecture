namespace Organization.Application.Abstractions.Images
{
    public sealed record BufferedImage(byte[]? Bytes = null,
                                        string? CanonicalExt = null,
                                        string? Mime = null);
}

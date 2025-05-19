using Microsoft.AspNetCore.Components.Forms;

namespace HsaLedger.Client.Common.Extensions;

internal static class BrowserFileExtensions
{
    internal static async Task<byte[]> GetFileBytesAsync(this IBrowserFile file)
    {
        await using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // e.g., 10MB max
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}
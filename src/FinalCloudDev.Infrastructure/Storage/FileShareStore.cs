using Azure;
using Azure.Storage.Files.Shares;
using FinalCloudDev.Application.Configuration;
using Microsoft.Extensions.Options;

namespace FinalCloudDev.Infrastructure.Storage;

public sealed class AzureFileShareStore(IOptions<StorageOptions> options) : IFileShareStore
{
    public async Task UploadAsync(string shareName, string fileName, Stream content, CancellationToken cancellationToken)
    {
        var share = new ShareClient(options.Value.FileShareConnectionString, shareName);
        await share.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var file = share.GetRootDirectoryClient().GetFileClient(fileName);

        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);
        var bytes = buffer.ToArray();
        await file.CreateAsync(bytes.Length, cancellationToken: cancellationToken);
        await file.UploadRangeAsync(
            new HttpRange(0, bytes.Length),
            new MemoryStream(bytes),
            cancellationToken: cancellationToken);
    }
}

public sealed class UnconfiguredFileShareStore : IFileShareStore
{
    public Task UploadAsync(string shareName, string fileName, Stream content, CancellationToken cancellationToken) =>
        throw new InvalidOperationException("File share storage is not configured. Set Storage:FileShareConnectionString when the brief requires Azure Files.");
}

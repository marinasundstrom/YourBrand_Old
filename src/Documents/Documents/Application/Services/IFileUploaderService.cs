﻿namespace YourBrand.Documents.Application.Services;

public interface IFileUploaderService
{
    Task UploadFileAsync(string id, Stream stream, CancellationToken cancellationToken = default);

    Task DeleteFileAsync(string id, CancellationToken cancellationToken = default);

    Task<Stream> DownloadFileAsync(string blobId);
}
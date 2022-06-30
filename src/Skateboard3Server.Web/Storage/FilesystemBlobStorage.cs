using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NLog;

namespace Skateboard3Server.Web.Storage;

public interface IBlobStorage
{
    Task PutObject(string bucket, string objectName, byte[] data);

    Task<byte[]> GetObject(string bucket, string objectName);
    bool ObjectExists(string bucket, string objectName);
}

/// <summary>
/// Poor mans object storage
/// </summary>
public class FilesystemBlobStorage : IBlobStorage
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly string _basePath;

    public FilesystemBlobStorage(IOptions<WebConfig> webConfig)
    {
        if (Path.IsPathFullyQualified(webConfig.Value.BlobStorageLocation))
        {
            _basePath = webConfig.Value.BlobStorageLocation;
        }
        else
        {
            _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, webConfig.Value.BlobStorageLocation); //TODO feels bad
        }
    }

    public async Task PutObject(string bucket, string objectName, byte[] data)
    {
        //TODO path traversal?
        if (data == null)
        {
            throw new ArgumentException("data cannot be null");
        }
        var fullPath = Path.Combine(_basePath, bucket, objectName);
        Logger.Debug($"Saving {objectName} in bucket: {bucket} fullPath: {fullPath}");
        //create dir if not exist
        var directoryName = Path.GetDirectoryName(fullPath);
        if (directoryName == null)
        {
            Logger.Warn($"Could not get directory name for {fullPath}");
            return;
        }
        Directory.CreateDirectory(directoryName);
        await File.WriteAllBytesAsync(fullPath, data);
    }

    public Task<byte[]> GetObject(string bucket, string objectName)
    {
        //TODO path traversal?
        if (ObjectExists(bucket, objectName))
        {
            var fullPath = Path.Combine(_basePath, bucket, objectName);
            Logger.Debug($"Getting {objectName} in bucket: {bucket} fullPath: {fullPath}");
            return File.ReadAllBytesAsync(fullPath);
        }

        throw new ArgumentException($"Object {objectName} does not exist in {bucket}");
    }

    public bool ObjectExists(string bucket, string objectName)
    {
        //TODO path traversal?
        var fullPath = Path.Combine(_basePath, bucket, objectName);
        return File.Exists(fullPath);
    }
}
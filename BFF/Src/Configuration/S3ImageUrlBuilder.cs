using Microsoft.Extensions.Options;

namespace BFF.Configuration;

public interface IS3ImageUrlBuilder
{
    IReadOnlyList<string> BuildUrls(IEnumerable<string> s3Keys);
}

public sealed class S3ImageUrlBuilder(IOptions<S3Configuration> options) : IS3ImageUrlBuilder
{
    public IReadOnlyList<string> BuildUrls(IEnumerable<string> s3Keys)
    {
        S3Configuration s3 = options.Value;
        string baseUrl = s3.ServiceUrl.TrimEnd('/');
        if (baseUrl.Contains("localstack", StringComparison.OrdinalIgnoreCase))
        {
            baseUrl = baseUrl.Replace("localstack", "localhost", StringComparison.OrdinalIgnoreCase);
        }

        return s3Keys
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .Select(key => $"{baseUrl}/{s3.BucketName}/{key.Trim()}")
            .ToList();
    }
}

using System.ComponentModel.DataAnnotations;

namespace BFF.Configuration;

public class S3Configuration
{
    public const string SectionName = "S3";
    [Required]
    public required string AwsAccessKeyId { get; init; }
    [Required]
    public required string AwsSecretAccessKey { get; init; }
    [Required]
    public required string ServiceUrl { get; init; }
    [Required]
    public required string Region { get; init; }
    [Required]
    public required string BucketName { get; init; }
}

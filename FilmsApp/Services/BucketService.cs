using Amazon.S3;
using Amazon.S3.Model;

namespace FilmsApp.Services
{
    public class BucketService(IAmazonS3 s3, string bucketName)
    {
        public async Task UploadAsync(string key, IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            await s3.PutObjectAsync(new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = stream,
                ContentType = file.ContentType
            });
        }

        public Task DeleteAsync(string key) => s3.DeleteObjectAsync(bucketName, key);

        public Task<string> GetReadUrlAsync(string key, TimeSpan lifetime) =>
            s3.GetPreSignedURLAsync(new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.Add(lifetime)
            });
    }
}

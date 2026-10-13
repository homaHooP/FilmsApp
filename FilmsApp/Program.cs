using Amazon;
using Amazon.S3;
using FilmsApp.Data;
using FilmsApp.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

const long maxUploadSize = 2L * 1024 * 1024 * 1024;
builder.Services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = maxUploadSize);
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = maxUploadSize);

var s3Config = builder.Configuration.GetSection("S3");
var region = RegionEndpoint.GetBySystemName(s3Config["Region"] ?? "eu-central-1");
var s3 = string.IsNullOrEmpty(s3Config["AccessKey"])
    ? new AmazonS3Client(region)
    : new AmazonS3Client(s3Config["AccessKey"], s3Config["SecretKey"], region);
var bucketName = s3Config["BucketName"] ?? throw new InvalidOperationException("S3:BucketName is not configured");
builder.Services.AddSingleton(new BucketService(s3, bucketName));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

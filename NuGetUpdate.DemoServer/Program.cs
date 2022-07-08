using System.IO.Compression;
using System.Web;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/FindPackagesById()", async context =>
{
    string packageId = ((string)context.Request.Query["id"]).Trim('\'');

    string packageName = Directory.GetFiles(@"..\Build\Distrib\Packages", packageId + "*.nupkg").Single();

    string version = Path.GetFileNameWithoutExtension(packageName).Substring(packageId.Length + 1);

    string template;

    using (var stream = typeof(Program).Assembly.GetManifestResourceStream("NuGetUpdate.DemoServer.FeedTemplate.xml"))
    using (var reader = new StreamReader(stream))
    {
        template = reader.ReadToEnd();
    }

    template = template
        .Replace("{ID}", HttpUtility.HtmlEncode(packageId))
        .Replace("{VERSION}", version)
        .Replace("{TARGET}", "http://localhost:5166/Download?id=" + Uri.EscapeDataString(packageId));

    await context.Response.WriteAsync(template);
});

app.MapGet("/Download", async context =>
{
    string packageId = ((string)context.Request.Query["id"]).Trim('\'');

    string packageName = Directory.GetFiles(@"..\Build\Distrib\Packages", packageId + "*.nupkg").Single();

    using (var stream = File.OpenRead(packageName))
    {
        await stream.CopyToAsync(context.Response.Body);
    }
});

app.Run();

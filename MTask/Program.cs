using Microsoft.EntityFrameworkCore;
using MTask.Data;
using MTask.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TagDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MTaskDb")));

builder.Services.AddHttpClient("StackExchangeClient", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://api.stackexchange.com/2.2/");
    httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
    httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
});
builder.Services.AddScoped<TagService>();
builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

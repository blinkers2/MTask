using MTask.Services;
using MTask.Extensions;
using MTask.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.UseSerilogConfiguration();

builder.Services
    .AddScoped<ITagService, TagService>()
    .AddScoped<ErrorLoggingMiddleware>()
    .AddEndpointsApiExplorer()
    .AddSwaggerDocumentation()
    .AddDatabase(builder.Configuration)
    .AddHttpClientServices()
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

var app = builder.Build();

app.UseMiddleware<ErrorLoggingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthorization();
app.MapControllers();
app.Run();
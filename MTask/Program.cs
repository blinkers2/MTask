using Microsoft.EntityFrameworkCore;
using MTask.Data;
using MTask.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TagDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MTaskDb")));

builder.Services.AddHttpClient(); // PóŸniej us³ugi do wywalenia do oddzielnego startConfiguration
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

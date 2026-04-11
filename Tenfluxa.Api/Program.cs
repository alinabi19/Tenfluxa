using Microsoft.EntityFrameworkCore;
using Tenfluxa.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TenfluxaDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=tenfluxa_db;Username=postgres;Password=@ni$@59"));

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
using GestionStock.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<GestionStockDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// CORS pour permettre les appels depuis l'app mobile en développement
builder.Services.AddCors(options =>
{
    options.AddPolicy("MobileDev", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Désactivé pour permettre les appels HTTP depuis l'émulateur Android (port 5026)
// app.UseHttpsRedirection();

app.UseCors("MobileDev");

app.UseAuthorization();

app.MapControllers();

app.Run();


using GestionStock.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Écouter sur toutes les interfaces réseau (nécessaire pour téléphone physique)
// Port 5026 = HTTP (accessible depuis le réseau Wi-Fi local)
builder.WebHost.UseUrls("http://0.0.0.0:5026");

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

// Automatically apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GestionStockDbContext>();
    context.Database.Migrate();
}

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


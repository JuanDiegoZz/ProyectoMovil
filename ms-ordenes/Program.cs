using Microsoft.EntityFrameworkCore;
using MsOrdenes.Data;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SQL Server
builder.Services.AddDbContext<OrdenesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// Migración automática al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdenesDbContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => "MS Órdenes corriendo ✅");

app.Run();
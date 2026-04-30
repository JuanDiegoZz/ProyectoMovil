using Microsoft.EntityFrameworkCore;
using MsOrdenes.Data;
using MsOrdenes.Models;
using MsOrdenes.Services;
using System.Text.Json;

namespace MsOrdenes.Endpoints;

public static class OrdenesEndpoints
{
    public static void MapOrdenesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/ordenes")
            .WithTags("Ordenes");

        // POST /ordenes - Crear orden con idempotencia
        group.MapPost("/", async (
            HttpContext http,
            CrearOrdenRequest request,
            OrdenService service,
            OrdenesDbContext db) =>
        {
            // 1. Verificar Idempotency-Key
            if (!http.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey) 
                || string.IsNullOrEmpty(idempotencyKey))
            {
                return Results.BadRequest("Se requiere el header 'Idempotency-Key'");
            }

            // 2. Verificar si ya existe esa clave
            var existente = await db.Idempotencias
                .FirstOrDefaultAsync(i => i.IdempotencyKey == idempotencyKey.ToString());

            if (existente is not null)
            {
                var ordenExistente = JsonSerializer.Deserialize<OrdenResponse>(existente.ResponseJson);
                return Results.Ok(ordenExistente);
            }

            try
            {
                // 3. Crear la orden
                var orden = await service.CrearOrdenAsync(request);

                // 4. Guardar la clave de idempotencia
                db.Idempotencias.Add(new OutboxIdempotencia
                {
                    IdempotencyKey = idempotencyKey.ToString(),
                    Endpoint = "POST /ordenes",
                    ResponseJson = JsonSerializer.Serialize(orden),
                    CreadoEn = DateTime.UtcNow
                });
                await db.SaveChangesAsync();

                return Results.Created($"/ordenes/{orden.Id}", orden);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        })
        .WithName("CrearOrden")
        .WithSummary("Crea una nueva orden aplicando descuentos automáticos");

        // GET /ordenes/{id} - Obtener orden por ID
        group.MapGet("/{id:int}", async (int id, OrdenService service) =>
        {
            var orden = await service.ObtenerOrdenPorIdAsync(id);
            return orden is null ? Results.NotFound() : Results.Ok(orden);
        })
        .WithName("ObtenerOrden")
        .WithSummary("Obtiene una orden por ID");

        // GET /ordenes/usuario/{usuarioId} - Obtener órdenes por usuario
        group.MapGet("/usuario/{usuarioId:int}", async (int usuarioId, OrdenService service) =>
        {
            var ordenes = await service.ObtenerOrdenesPorUsuarioAsync(usuarioId);
            return Results.Ok(ordenes);
        })
        .WithName("ObtenerOrdenesPorUsuario")
        .WithSummary("Obtiene todas las órdenes de un usuario");
    }
}
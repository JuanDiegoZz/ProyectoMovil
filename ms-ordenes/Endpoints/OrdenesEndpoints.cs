using MsOrdenes.Models;
using MsOrdenes.Services;

namespace MsOrdenes.Endpoints;

public static class OrdenesEndpoints
{
    public static void MapOrdenesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/ordenes")
            .WithTags("Ordenes");

        // POST /ordenes - Crear orden
        group.MapPost("/", async (CrearOrdenRequest request, OrdenService service) =>
        {
            try
            {
                var orden = await service.CrearOrdenAsync(request);
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
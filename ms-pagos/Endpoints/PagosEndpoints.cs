using MsPagos.Services;

namespace MsPagos.Endpoints;

public static class PagosEndpoints
{
    public static void MapPagosEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/pagos")
            .WithTags("Pagos");

        // GET /pagos/orden/{ordenId}
        group.MapGet("/orden/{ordenId:int}", async (int ordenId, PagoService service) =>
        {
            var pagos = await service.ObtenerPagosPorOrdenAsync(ordenId);
            return Results.Ok(pagos);
        })
        .WithName("ObtenerPagosPorOrden")
        .WithSummary("Obtiene los pagos de una orden");
    }
}
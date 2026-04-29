using System.Text.Json;

namespace MsPagos.Services;

public class OpenPayService
{
    private readonly IConfiguration _config;
    private readonly ILogger<OpenPayService> _logger;

    public OpenPayService(IConfiguration config, ILogger<OpenPayService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<OpenPayResult> ProcesarPagoAsync(decimal monto, int mesesMsi)
    {
        try
        {
            // Simulación de OpenPay Sandbox
            // En producción aquí iría la llamada real a la API de OpenPay
            await Task.Delay(500); // Simula latencia de red

            // Simulamos que el 90% de los pagos son aprobados
            var random = new Random();
            bool aprobado = random.Next(1, 11) <= 9;

            if (aprobado)
            {
                return new OpenPayResult
                {
                    Exitoso = true,
                    TransaccionId = $"txn_{Guid.NewGuid():N}",
                    Referencia = $"ref_{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Mensaje = "Pago aprobado"
                };
            }
            else
            {
                return new OpenPayResult
                {
                    Exitoso = false,
                    TransaccionId = null,
                    Referencia = null,
                    ErrorCodigo = "3001",
                    Mensaje = "Tarjeta declinada"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando pago en OpenPay");
            return new OpenPayResult
            {
                Exitoso = false,
                ErrorCodigo = "9999",
                Mensaje = "Error interno al procesar pago"
            };
        }
    }
}

public class OpenPayResult
{
    public bool Exitoso { get; set; }
    public string? TransaccionId { get; set; }
    public string? Referencia { get; set; }
    public string? ErrorCodigo { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}
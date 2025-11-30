using System.Net.Http.Json;

namespace MyPlanU.Services;

public class PromptyClient : IPromptyClient
{
    private readonly HttpClient _http;

    public PromptyClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<PromptyResponse> EnviarMensajeAsync(
        string mensaje,
        IEnumerable<HistorialItem>? historial = null)
    {
        var payload = new
        {
            mensaje = mensaje,
            historial = historial ?? new List<HistorialItem>()
        };

        var resp = await _http.PostAsJsonAsync("/api/chat", payload);

        if (!resp.IsSuccessStatusCode)
        {
            return new PromptyResponse
            {
                Exito = false,
                Respuesta = $"Error HTTP {resp.StatusCode}"
            };
        }

        var json = await resp.Content.ReadFromJsonAsync<PromptyResponse>();
        return json ?? new PromptyResponse { Exito = false, Respuesta = "(Sin respuesta)" };
    }
}

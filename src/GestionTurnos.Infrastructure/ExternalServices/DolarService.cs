using GestionTurnos.Application.Abstraction.Infrastructure.External_Interface;
using GestionTurnos.Application.Response;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace GestionTurnos.Infrastructure.ExternalServices
{
    public class DolarService : IDolarService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public DolarService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<decimal> CurrentDolarPrice()
        {
            var client = _httpClientFactory.CreateClient("DolarApi"); // nombre del cliente registrado

            var dolarData = await client.GetFromJsonAsync<DolarResponseDto>("v1/dolares/oficial")
                ?? throw new Exception("La respuesta de la API vino vacía.");

            return dolarData.Venta;
        }
    }
}

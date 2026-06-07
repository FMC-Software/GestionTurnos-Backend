using GestionTurnos.Application.Abstraction.Infrastructure.External_Interface;
using GestionTurnos.Application.Response;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace GestionTurnos.Infrastructure.ExternalServices
{
    public class DolarPriceService : IDolarPriceService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DolarPriceService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }


        public async Task<decimal>  CurrentDolarPrice()
        {
            var dolarData = await _httpClient.GetFromJsonAsync<DolarResponseDto>("v1/dolares/oficial")
             ?? throw new Exception("La respuesta de la API vino vacía.");
 
            return dolarData.Venta;

        }
    }
}

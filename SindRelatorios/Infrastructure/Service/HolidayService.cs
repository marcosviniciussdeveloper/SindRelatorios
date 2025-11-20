using SindRelatorios.Models.Entities;
using System.Text.Json;
using SindRelatorios.Application.Interfaces;

namespace SindRelatorios.Infrastructure.Service
{
    
    /// Serviço para obter feriados de um determinado ano.
    public class HolidayService : IHolidayService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        
        private static Dictionary<int, HashSet<DateTime>> _cache = new();

        public HolidayService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HashSet<DateTime>> GetHolidays(int ano)
        {
            if (_cache.TryGetValue(ano, out var feriadosEmCache))
            {
                return feriadosEmCache;
            }
            
            var feriados = new HashSet<DateTime>();
            var client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.GetAsync($"https://brasilapi.com.br/api/feriados/v1/{ano}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var listaFeriados = JsonSerializer.Deserialize<List<Holiday>>(json);

                    if (listaFeriados != null)
                    {
                        foreach (var feriado in listaFeriados)
                        {
                            feriados.Add(feriado.Data.Date);
                        }
                    }
                }
                _cache[ano] = feriados; 
                return feriados;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar feriados: {ex.Message}");
                return feriados; 
            }
        }
    }
}
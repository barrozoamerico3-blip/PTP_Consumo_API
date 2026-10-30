
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Consumo_API_CSharp
{
    // Classe que representa os dados meteorologicos
    public class CurrentWeather
    {
        [JsonPropertyName("time")]
        public string Time { get; set; } = "";

        [JsonPropertyName("temperature_2m")]
        public double Temperature { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public int Humidity { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public double WindSpeed { get; set; }

        [JsonPropertyName("weather_code")]
        public int WeatherCode { get; set; }
    }

    // Classe que representa a resposta da API
    public class WeatherResponse
    {
        [JsonPropertyName("current")]
        public CurrentWeather? Current { get; set; }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Sistema de Consulta Meteorologica";

            Console.WriteLine("========================================");
            Console.WriteLine("     SISTEMA DE CONSULTA METEOROLOGICA");
            Console.WriteLine("========================================");
            Console.WriteLine();

            // Coordenadas de Maputo
            string latitude = "-25.9692";
            string longitude = "32.5732";

            string url =
                "https://api.open-meteo.com/v1/forecast" +
                "?latitude=" + latitude +
                "&longitude=" + longitude +
                "&current=temperature_2m,relative_humidity_2m,wind_speed_10m,weather_code" +
                "&timezone=Africa%2FMaputo";

            using HttpClient cliente = new HttpClient();

            try
            {
                Console.WriteLine("A conectar a API meteorologica...");
                Console.WriteLine();

                // Requisicao HTTP GET
                HttpResponseMessage resposta =
                    await cliente.GetAsync(url);

                resposta.EnsureSuccessStatusCode();

                // Receber os dados JSON
                string json =
                    await resposta.Content.ReadAsStringAsync();

                // Converter JSON em objetos C#
                WeatherResponse? dados =
                    JsonSerializer.Deserialize<WeatherResponse>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                Console.WriteLine("CONEXAO REALIZADA COM SUCESSO!");
                Console.WriteLine();

                if (dados?.Current != null)
                {
                    CurrentWeather clima = dados.Current;

                    Console.WriteLine("DADOS METEOROLOGICOS DE MAPUTO");
                    Console.WriteLine("----------------------------------------");

                    Console.WriteLine(
                        $"Data e hora: {clima.Time}");

                    Console.WriteLine(
                        $"Temperatura: {clima.Temperature} °C");

                    Console.WriteLine(
                        $"Humidade: {clima.Humidity}%");

                    Console.WriteLine(
                        $"Velocidade do vento: {clima.WindSpeed} km/h");

                    Console.WriteLine(
                        $"Condicoes: {ObterCondicao(clima.WeatherCode)}");

                    Console.WriteLine("----------------------------------------");
                    Console.WriteLine("CONSULTA REALIZADA COM SUCESSO!");
                }
                else
                {
                    Console.WriteLine("Nao foram encontrados dados meteorologicos.");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("ERRO DE CONEXAO COM A API!");
                Console.WriteLine(ex.Message);
            }
            catch (JsonException ex)
            {
                Console.WriteLine("ERRO AO PROCESSAR OS DADOS JSON!");
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("OCORREU UM ERRO!");
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine("           FIM DA EXECUCAO");
            Console.WriteLine("========================================");

            Console.WriteLine("Pressiona qualquer tecla para sair...");
            Console.ReadKey();
        }

        // Interpretar os codigos meteorologicos WMO
        static string ObterCondicao(int codigo)
        {
            return codigo switch
            {
                0 => "Ceu limpo",
                1 => "Predominantemente limpo",
                2 => "Parcialmente nublado",
                3 => "Nublado",
                45 or 48 => "Nevoeiro",
                51 or 53 or 55 => "Chuvisco",
                61 or 63 or 65 => "Chuva",
                71 or 73 or 75 => "Queda de neve",
                80 or 81 or 82 => "Aguaceiros",
                95 or 96 or 99 => "Trovoada",
                _ => "Condicao meteorologica variavel"
            };
        }
    }
}

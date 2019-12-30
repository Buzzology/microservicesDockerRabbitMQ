using System.Threading.Tasks;
using gRPCTest;
using static gRPCTest.Weather;

namespace publisher_api.Services
{

    public class WeatherService : WeatherBase
    {
        public override Task<WeatherForecastResponse> Forecast(WeatherForecastRequest request, Grpc.Core.ServerCallContext context)
        {
            var weatherForecast = new WeatherForecast();
            
            return Task.FromResult(
                new WeatherForecastResponse {
                    Date = new Google.Protobuf.WellKnownTypes.Timestamp { Seconds = weatherForecast.Date.Second },
                    TemperatureC = weatherForecast.TemperatureC,
                    TemperatureF = weatherForecast.TemperatureF,
                    Summary = weatherForecast.Summary,
                }
            );
        }
    }
}
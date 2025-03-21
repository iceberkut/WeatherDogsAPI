using System;
using System.Threading.Tasks;
using Models;
using UnityEngine;
using Zenject;

namespace Services
{
    public class WeatherService
    {
        private readonly WeatherApiService _weatherApiService;
        private readonly SignalBus _signalBus;

        [Inject]
        public WeatherService(WeatherApiService weatherApiService, SignalBus signalBus)
        {
            _weatherApiService = weatherApiService;
            _signalBus = signalBus;
        }

        public async Task<WeatherModel> GetWeatherAsync()
        {
            try
            {
                Debug.Log("WeatherService: Starting weather request");
                var response = await _weatherApiService.GetWeatherDataAsync();
                
                if (response?.properties?.periods == null || response.properties.periods.Length == 0)
                {
                    Debug.LogError("WeatherService: No weather periods in response");
                    return CreateDefaultWeatherModel();
                }

                var weatherPeriod = response.properties.periods[0];
                var weather = new WeatherModel
                {
                    temperature = $"{weatherPeriod.temperature}°F",
                    icon = weatherPeriod.icon,
                    description = weatherPeriod.shortForecast
                };
                
                Debug.Log($"WeatherService: Weather data prepared - Temperature: {weather.temperature}, Description: {weather.description}");
                return weather;
            }
            catch (Exception e)
            {
                Debug.LogError($"WeatherService: Error getting weather data - {e.Message}");
                return CreateDefaultWeatherModel();
            }
        }

        private WeatherModel CreateDefaultWeatherModel()
        {
            return new WeatherModel
            {
                temperature = "N/A",
                icon = "",
                description = "No data available"
            };
        }
    }
} 
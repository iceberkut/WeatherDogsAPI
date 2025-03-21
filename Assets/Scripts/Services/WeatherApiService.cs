using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Models;

namespace Services
{
    public class WeatherApiService
    {
        private const string WEATHER_API_URL = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
        private const string USER_AGENT = "WeatherApp/1.0";

        public async Task<WeatherResponse> GetWeatherDataAsync()
        {
            try
            {
                using (var request = CreateRequest())
                {
                    var operation = request.SendWebRequest();
                    while (!operation.isDone)
                        await Task.Yield();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        Debug.Log($"WeatherApiService: Response received - {request.downloadHandler.text}");
                        return JsonUtility.FromJson<WeatherResponse>(request.downloadHandler.text);
                    }
                    else
                    {
                        Debug.LogError($"WeatherApiService: Request failed - {request.error}");
                        throw new Exception($"Request failed: {request.error}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"WeatherApiService: Exception during request - {e.Message}");
                Debug.LogError($"WeatherApiService: Stack trace - {e.StackTrace}");
                throw;
            }
        }

        private UnityWebRequest CreateRequest()
        {
            var request = new UnityWebRequest(WEATHER_API_URL, "GET");
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("User-Agent", USER_AGENT);
            return request;
        }
    }
} 
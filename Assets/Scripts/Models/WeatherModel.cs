using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class WeatherModel
    {
        public string temperature;
        public string icon;
        public string description;
    }

    [Serializable]
    public class WeatherResponse
    {
        public WeatherProperties properties;
    }

    [Serializable]
    public class WeatherProperties
    {
        public WeatherPeriod[] periods;
    }

    [Serializable]
    public class WeatherPeriod
    {
        public int temperature;
        public string icon;
        public string shortForecast;
    }
} 
using System;
using System.Threading;
using System.Threading.Tasks;
using Models;
using Services;
using Signals;
using UnityEngine;
using Zenject;

namespace Controllers
{
    public class WeatherController : IInitializable, IDisposable
    {
        private readonly WeatherApiService _weatherApiService;
        private readonly SignalBus _signalBus;
        private bool _isActive;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _updateTask;

        [Inject]
        public WeatherController(WeatherApiService weatherApiService, SignalBus signalBus)
        {
            _weatherApiService = weatherApiService;
            _signalBus = signalBus;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Initialize()
        {
            SubscribeToSignals();
        }

        public void Dispose()
        {
            UnsubscribeFromSignals();
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        private void SubscribeToSignals()
        {
            _signalBus.Subscribe<GameSignals.WeatherTabActivatedSignal>(OnWeatherTabActivated);
            _signalBus.Subscribe<GameSignals.WeatherTabDeactivatedSignal>(OnWeatherTabDeactivated);
        }

        private void UnsubscribeFromSignals()
        {
            _signalBus.Unsubscribe<GameSignals.WeatherTabActivatedSignal>(OnWeatherTabActivated);
            _signalBus.Unsubscribe<GameSignals.WeatherTabDeactivatedSignal>(OnWeatherTabDeactivated);
        }

        private void OnWeatherTabActivated()
        {
            _isActive = true;
            StartWeatherUpdates();
        }

        private void OnWeatherTabDeactivated()
        {
            _isActive = false;
            StopWeatherUpdates();
        }

        private void StartWeatherUpdates()
        {
            if (_updateTask != null && !_updateTask.IsCompleted)
            {
                return;
            }

            _updateTask = UpdateWeatherLoop();
        }

        private void StopWeatherUpdates()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private async Task UpdateWeatherLoop()
        {
            while (_isActive)
            {
                try
                {
                    await LoadWeather();
                    await Task.Delay(5000, _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("WeatherController: Update loop cancelled");
                    break;
                }
                catch (Exception e)
                {
                    Debug.LogError($"WeatherController: Error in update loop - {e.Message}");
                    await Task.Delay(5000, _cancellationTokenSource.Token);
                }
            }
        }

        private async Task LoadWeather()
        {
            if (!_isActive) return;

            try
            {
                _signalBus.Fire(new GameSignals.LoadingStartedSignal());
                var weatherResponse = await _weatherApiService.GetWeatherDataAsync();
                
                if (_isActive)
                {
                    var weatherModel = new WeatherModel
                    {
                        temperature = weatherResponse.properties.periods[0].temperature.ToString(),
                        description = weatherResponse.properties.periods[0].shortForecast,
                        icon = weatherResponse.properties.periods[0].icon
                    };
                    _signalBus.Fire(new GameSignals.WeatherUpdatedSignal(weatherModel));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"WeatherController: Error loading weather - {e.Message}");
                if (_isActive)
                {
                    _signalBus.Fire(new GameSignals.ErrorSignal("Failed to load weather data"));
                }
            }
            finally
            {
                if (_isActive)
                {
                    _signalBus.Fire(new GameSignals.LoadingFinishedSignal());
                }
            }
        }
    }
} 
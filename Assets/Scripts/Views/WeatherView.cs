using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using Signals;
using Models;
using UnityEngine.Networking;

namespace Views
{
    public class WeatherView : MonoBehaviour
    {
        [SerializeField] private GameObject container;
        [SerializeField] private TextMeshProUGUI temperatureText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image weatherIcon;
        [SerializeField] private GameObject loadingIndicator;
        [SerializeField] private GameObject errorText;

        private SignalBus _signalBus;
        private bool _isActive;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
            Debug.Log("WeatherView: Constructor called");
            _isActive = true;
            SubscribeToSignals();
        }

        private void Awake()
        {
            Debug.Log("WeatherView: Awake called");
            ValidateComponents();
        }

        private void Start()
        {
            Debug.Log("WeatherView: Start called");
            if (container != null)
            {
                container.SetActive(false);
                Debug.Log("WeatherView: Weather container deactivated on start");
            }
        }

        private void OnDestroy()
        {
            Debug.Log("WeatherView: OnDestroy called");
            UnsubscribeFromSignals();
        }

        private void SubscribeToSignals()
        {
            if (!_isActive || _signalBus == null)
            {
                Debug.LogError("WeatherView: Cannot subscribe to signals - not active or SignalBus is null!");
                return;
            }

            Debug.Log("WeatherView: Subscribing to signals");
            _signalBus.Subscribe<GameSignals.WeatherUpdatedSignal>(OnWeatherUpdated);
            _signalBus.Subscribe<GameSignals.LoadingStartedSignal>(OnLoadingStarted);
            _signalBus.Subscribe<GameSignals.LoadingFinishedSignal>(OnLoadingFinished);
            _signalBus.Subscribe<GameSignals.ErrorSignal>(OnError);
            _signalBus.Subscribe<GameSignals.WeatherTabActivatedSignal>(OnWeatherTabActivated);
            _signalBus.Subscribe<GameSignals.WeatherTabDeactivatedSignal>(OnWeatherTabDeactivated);
        }

        private void UnsubscribeFromSignals()
        {
            if (!_isActive || _signalBus == null)
            {
                Debug.LogError("WeatherView: Cannot unsubscribe from signals - not active or SignalBus is null!");
                return;
            }

            Debug.Log("WeatherView: Unsubscribing from signals");
            _signalBus.Unsubscribe<GameSignals.WeatherUpdatedSignal>(OnWeatherUpdated);
            _signalBus.Unsubscribe<GameSignals.LoadingStartedSignal>(OnLoadingStarted);
            _signalBus.Unsubscribe<GameSignals.LoadingFinishedSignal>(OnLoadingFinished);
            _signalBus.Unsubscribe<GameSignals.ErrorSignal>(OnError);
            _signalBus.Unsubscribe<GameSignals.WeatherTabActivatedSignal>(OnWeatherTabActivated);
            _signalBus.Unsubscribe<GameSignals.WeatherTabDeactivatedSignal>(OnWeatherTabDeactivated);
        }

        private void ValidateComponents()
        {
            if (weatherIcon == null)
            {
                Debug.LogError("WeatherView: weatherIcon reference is missing!");
                return;
            }

            if (temperatureText == null)
            {
                Debug.LogError("WeatherView: temperatureText reference is missing!");
                return;
            }

            if (descriptionText == null)
            {
                Debug.LogError("WeatherView: descriptionText reference is missing!");
                return;
            }

            if (container == null)
            {
                Debug.LogError("WeatherView: container reference is missing!");
                return;
            }

            if (loadingIndicator == null)
            {
                Debug.LogError("WeatherView: loadingIndicator reference is missing!");
                return;
            }

            if (errorText == null)
            {
                Debug.LogError("WeatherView: errorText reference is missing!");
                return;
            }

            Debug.Log("WeatherView: All components validated successfully");
        }

        private void OnWeatherTabActivated()
        {
            _isActive = true;
            container.SetActive(true);
        }

        private void OnWeatherTabDeactivated()
        {
            _isActive = false;
            container.SetActive(false);
        }

        private void OnWeatherUpdated(GameSignals.WeatherUpdatedSignal signal)
        {
            if (!_isActive || !gameObject.activeInHierarchy) return;

            var weather = signal.Weather;
            temperatureText.text = $"{weather.temperature}°C";
            descriptionText.text = weather.description;
            
            if (!string.IsNullOrEmpty(weather.icon))
            {
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(LoadWeatherIcon(weather.icon));
                }
            }
        }

        private void OnLoadingStarted()
        {
            if (!_isActive || !gameObject.activeInHierarchy) return;
            loadingIndicator.SetActive(true);
            errorText.SetActive(false);
        }

        private void OnLoadingFinished()
        {
            if (!_isActive || !gameObject.activeInHierarchy) return;
            loadingIndicator.SetActive(false);
        }

        private void OnError(GameSignals.ErrorSignal signal)
        {
            if (!_isActive || !gameObject.activeInHierarchy) return;
            loadingIndicator.SetActive(false);
            errorText.SetActive(true);
            errorText.GetComponent<TextMeshProUGUI>().text = signal.Message;
        }

        private System.Collections.IEnumerator LoadWeatherIcon(string iconUrl)
        {
            if (!gameObject.activeInHierarchy) yield break;

            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(iconUrl))
            {
                yield return request.SendWebRequest();

                if (!gameObject.activeInHierarchy) yield break;

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    weatherIcon.sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        Vector2.zero
                    );
                }
                else
                {
                    Debug.LogError($"Failed to load weather icon: {request.error}");
                }
            }
        }
    }
} 
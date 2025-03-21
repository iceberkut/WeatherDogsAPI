using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using Signals;

namespace Views
{
    public class TabNavigationView : MonoBehaviour
    {
        [SerializeField] private Button weatherTabButton;
        [SerializeField] private Button dogsTabButton;
        [SerializeField] private GameObject weatherTab;
        [SerializeField] private GameObject dogsTab;

        private SignalBus _signalBus;
        private bool _isInitialized;

        private void Awake()
        {
            Debug.Log("TabNavigationView: Awake called");
            ValidateComponents();
        }

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            Debug.Log("TabNavigationView: Construct called");
            _signalBus = signalBus;
            _isInitialized = true;
            SubscribeToSignals();
        }

        private void Start()
        {
            Debug.Log("TabNavigationView: Start called");
            if (!_isInitialized)
            {
                Debug.LogError("TabNavigationView: Not initialized yet! SignalBus is null.");
                return;
            }
            weatherTab.SetActive(true);
            dogsTab.SetActive(false);
        }

        private void OnDestroy()
        {
            Debug.Log("TabNavigationView: OnDestroy called");
            if (_isInitialized)
            {
                UnsubscribeFromSignals();
            }
        }

        private void ValidateComponents()
        {
            if (weatherTabButton == null)
            {
                Debug.LogError("TabNavigationView: weatherTabButton reference is missing!");
                return;
            }

            if (dogsTabButton == null)
            {
                Debug.LogError("TabNavigationView: dogsTabButton reference is missing!");
                return;
            }

            if (weatherTab == null)
            {
                Debug.LogError("TabNavigationView: weatherTab reference is missing!");
                return;
            }

            if (dogsTab == null)
            {
                Debug.LogError("TabNavigationView: dogsTab reference is missing!");
                return;
            }

            Debug.Log("TabNavigationView: All components validated successfully");
        }

        private void SubscribeToSignals()
        {
            if (!_isInitialized || _signalBus == null)
            {
                Debug.LogError("TabNavigationView: Cannot subscribe to signals - not initialized or SignalBus is null!");
                return;
            }

            Debug.Log("TabNavigationView: Subscribing to signals");
            weatherTabButton.onClick.AddListener(OnWeatherTabClicked);
            dogsTabButton.onClick.AddListener(OnDogsTabClicked);
        }

        private void UnsubscribeFromSignals()
        {
            if (!_isInitialized || _signalBus == null)
            {
                Debug.LogError("TabNavigationView: Cannot unsubscribe from signals - not initialized or SignalBus is null!");
                return;
            }

            Debug.Log("TabNavigationView: Unsubscribing from signals");
            weatherTabButton.onClick.RemoveListener(OnWeatherTabClicked);
            dogsTabButton.onClick.RemoveListener(OnDogsTabClicked);
        }

        private void OnWeatherTabClicked()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("TabNavigationView: Ignoring weather tab click - not initialized yet");
                return;
            }

            Debug.Log("TabNavigationView: Weather tab clicked");
            weatherTab.SetActive(true);
            dogsTab.SetActive(false);
            _signalBus.Fire(new GameSignals.WeatherTabActivatedSignal());
        }

        private void OnDogsTabClicked()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("TabNavigationView: Ignoring dogs tab click - not initialized yet");
                return;
            }

            Debug.Log("TabNavigationView: Dogs tab clicked");
            weatherTab.SetActive(false);
            dogsTab.SetActive(true);
            _signalBus.Fire(new GameSignals.DogTabActivatedSignal());
        }
    }
} 
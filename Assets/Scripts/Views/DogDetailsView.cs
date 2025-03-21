using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using Signals;
using Models;

namespace Views
{
    public class DogDetailsView : MonoBehaviour
    {
        [SerializeField] private GameObject container;
        [SerializeField] private Image dogImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI temperamentText;
        [SerializeField] private TextMeshProUGUI lifeSpanText;
        [SerializeField] private TextMeshProUGUI weightText;
        [SerializeField] private TextMeshProUGUI heightText;
        [SerializeField] private GameObject loadingIndicator;
        [SerializeField] private GameObject errorText;

        private SignalBus _signalBus;
        private bool _isInitialized;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            Debug.Log("DogDetailsView: Construct called");
            _signalBus = signalBus;
            _isInitialized = true;
            SubscribeToSignals();
        }

        private void OnDestroy()
        {
            if (_isInitialized)
            {
                UnsubscribeFromSignals();
            }
        }

        private void SubscribeToSignals()
        {
            if (!_isInitialized || _signalBus == null)
            {
                Debug.LogError("DogDetailsView: Cannot subscribe to signals - not initialized or SignalBus is null!");
                return;
            }

            Debug.Log("DogDetailsView: Subscribing to signals");
            _signalBus.Subscribe<GameSignals.DogBreedDetailsLoadedSignal>(OnBreedDetailsLoaded);
            _signalBus.Subscribe<GameSignals.LoadingStartedSignal>(OnLoadingStarted);
            _signalBus.Subscribe<GameSignals.LoadingFinishedSignal>(OnLoadingFinished);
            _signalBus.Subscribe<GameSignals.ErrorSignal>(OnError);
        }

        private void UnsubscribeFromSignals()
        {
            if (!_isInitialized || _signalBus == null)
            {
                Debug.LogError("DogDetailsView: Cannot unsubscribe from signals - not initialized or SignalBus is null!");
                return;
            }

            Debug.Log("DogDetailsView: Unsubscribing from signals");
            _signalBus.Unsubscribe<GameSignals.DogBreedDetailsLoadedSignal>(OnBreedDetailsLoaded);
            _signalBus.Unsubscribe<GameSignals.LoadingStartedSignal>(OnLoadingStarted);
            _signalBus.Unsubscribe<GameSignals.LoadingFinishedSignal>(OnLoadingFinished);
            _signalBus.Unsubscribe<GameSignals.ErrorSignal>(OnError);
        }

        private void OnBreedDetailsLoaded(GameSignals.DogBreedDetailsLoadedSignal signal)
        {
            var breed = signal.Breed;
            container.SetActive(true);
            
            nameText.text = breed.name;
            descriptionText.text = breed.description;
            temperamentText.text = $"Temperament: {breed.temperament}";
            lifeSpanText.text = $"Life Span: {breed.lifeSpan}";
            weightText.text = $"Weight: {breed.weight}";
            heightText.text = $"Height: {breed.height}";

        }

        private void OnLoadingStarted()
        {
            loadingIndicator.SetActive(true);
            errorText.SetActive(false);
        }

        private void OnLoadingFinished()
        {
            loadingIndicator.SetActive(false);
        }

        private void OnError(GameSignals.ErrorSignal signal)
        {
            loadingIndicator.SetActive(false);
            errorText.SetActive(true);
            errorText.GetComponent<TextMeshProUGUI>().text = signal.Message;
        }
    }
} 
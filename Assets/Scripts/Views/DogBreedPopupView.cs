using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using Signals;
using Models;

namespace Views
{
    public class DogBreedPopupView : MonoBehaviour
    {
        [SerializeField] private GameObject container;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private GameObject errorText;
        [SerializeField] private Button closeButton;

        private SignalBus _signalBus;
        private bool _isInitialized;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            Debug.Log("DogBreedPopupView: Construct called");
            _signalBus = signalBus;
            _isInitialized = true;
            SubscribeToSignals();
            
            if (container != null)
            {
                container.SetActive(false);
            }
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
                Debug.LogError("DogBreedPopupView: Cannot subscribe to signals - not initialized or SignalBus is null!");
                return;
            }

            Debug.Log("DogBreedPopupView: Subscribing to signals");
            _signalBus.Subscribe<GameSignals.DogBreedDetailsLoadedSignal>(OnBreedDetailsLoaded);
            _signalBus.Subscribe<GameSignals.ErrorSignal>(OnError);

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseClicked);
            }
        }

        private void UnsubscribeFromSignals()
        {
            if (!_isInitialized || _signalBus == null)
            {
                Debug.LogError("DogBreedPopupView: Cannot unsubscribe from signals - not initialized or SignalBus is null!");
                return;
            }

            Debug.Log("DogBreedPopupView: Unsubscribing from signals");
            _signalBus.Unsubscribe<GameSignals.DogBreedDetailsLoadedSignal>(OnBreedDetailsLoaded);
            _signalBus.Unsubscribe<GameSignals.ErrorSignal>(OnError);

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(OnCloseClicked);
            }
        }

        private void OnBreedDetailsLoaded(GameSignals.DogBreedDetailsLoadedSignal signal)
        {
            var breed = signal.Breed;
            container.SetActive(true);
            
            nameText.text = breed.name;
            descriptionText.text = $"Bred for: {breed.bred_for}\n" +
                                  $"Breed group: {breed.breed_group}\n" +
                                  $"Temperament: {breed.temperament}\n" +
                                  $"Weight: {breed.weight.imperial} (imperial) / {breed.weight.metric} (metric)\n" +
                                  $"Height: {breed.height.imperial} (imperial) / {breed.height.metric} (metric)";
        }

        private void OnError(GameSignals.ErrorSignal signal)
        {
            if (errorText != null)
            {
                errorText.SetActive(true);
                var errorTextComponent = errorText.GetComponent<TextMeshProUGUI>();
                if (errorTextComponent != null)
                {
                    errorTextComponent.text = signal.Message;
                }
            }
        }

        private void OnCloseClicked()
        {
            container.SetActive(false);
            if (errorText != null)
            {
                errorText.SetActive(false);
            }
        }
    }
} 
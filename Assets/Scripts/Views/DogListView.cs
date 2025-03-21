using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using Signals;
using static Signals.GameSignals;
using Models;

namespace Views
{
    public class DogListView : MonoBehaviour
    {
        [SerializeField] private GameObject dogBreedItemPrefab;
        [SerializeField] private RectTransform contentContainer;
        [SerializeField] private GameObject loadingIndicator;
        [SerializeField] private GameObject errorText;
        [SerializeField] private ScrollRect scrollView;

        private SignalBus _signalBus;
        private List<GameObject> breedItems = new List<GameObject>();

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            Debug.Log("DogListView: Constructor called");
            _signalBus = signalBus;
            Debug.Log("DogListView: Subscribing to signals");
            SubscribeToSignals();

            ValidateComponents();
        }

        private void ValidateComponents()
        {
            Debug.Log("DogListView: Validating components");
            if (dogBreedItemPrefab == null)
            {
                Debug.LogError("DogListView: dogBreedItemPrefab is not assigned!");
            }
            
            if (contentContainer == null)
            {
                Debug.LogError("DogListView: contentContainer is not assigned!");
            }
            
            if (loadingIndicator == null)
            {
                Debug.LogError("DogListView: loadingIndicator is not assigned!");
            }
            
            if (errorText == null)
            {
                Debug.LogError("DogListView: errorText is not assigned!");
            }
            
            if (scrollView == null)
            {
                Debug.LogError("DogListView: scrollView is not assigned!");
            }
        }

        private void SubscribeToSignals()
        {
            Debug.Log("DogListView: Subscribing to signals");
            _signalBus.Subscribe<GameSignals.DogBreedsLoadedSignal>(OnBreedsLoaded);
            _signalBus.Subscribe<GameSignals.LoadingStartedSignal>(OnLoadingStarted);
            _signalBus.Subscribe<GameSignals.LoadingFinishedSignal>(OnLoadingFinished);
            _signalBus.Subscribe<GameSignals.ErrorSignal>(OnError);
            Debug.Log("DogListView: Signal subscriptions completed");
        }

        private void OnDestroy()
        {
            Debug.Log("DogListView: OnDestroy called");
            _signalBus.Unsubscribe<GameSignals.DogBreedsLoadedSignal>(OnBreedsLoaded);
            _signalBus.Unsubscribe<GameSignals.LoadingStartedSignal>(OnLoadingStarted);
            _signalBus.Unsubscribe<GameSignals.LoadingFinishedSignal>(OnLoadingFinished);
            _signalBus.Unsubscribe<GameSignals.ErrorSignal>(OnError);
        }

        private void OnBreedsLoaded(DogBreedsLoadedSignal signal)
        {
            Debug.Log($"DogListView: Creating items for {signal.Breeds.Length} breeds");
            
            ClearBreedItems();
            
            List<int> randomIndices = new List<int>();
            for (int i = 0; i < signal.Breeds.Length; i++)
            {
                randomIndices.Add(i);
            }
            
            for (int i = randomIndices.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                int temp = randomIndices[i];
                randomIndices[i] = randomIndices[j];
                randomIndices[j] = temp;
            }
            
            int breedsToShow = Mathf.Min(15, signal.Breeds.Length);
            for (int i = 0; i < breedsToShow; i++)
            {
                CreateBreedItem(signal.Breeds[randomIndices[i]]);
            }
            
            Debug.Log($"DogListView: Created {breedsToShow} random breed items");
        }

        private void CreateBreedItem(DogBreed breed)
        {
            Debug.Log($"DogListView: Creating item for breed {breed.name}");
            
            GameObject item = Instantiate(dogBreedItemPrefab, contentContainer);
            breedItems.Add(item);
            
            Button button = item.GetComponent<Button>();
            if (button != null)
            {
                Debug.Log($"DogListView: Adding Button component to item for breed {breed.name}");
                
                button.onClick.AddListener(() => OnBreedSelected(breed));
                
                TextMeshProUGUI buttonText = item.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = breed.name;
                }
            }
            
            Debug.Log($"DogListView: Successfully created item for breed {breed.name}");
        }

        private void ClearBreedItems()
        {
            foreach (var item in breedItems)
            {
                Destroy(item);
            }
            breedItems.Clear();
        }

        private void OnLoadingStarted(GameSignals.LoadingStartedSignal signal)
        {
            Debug.Log("DogListView: Loading started");
            if (loadingIndicator != null)
            {
                loadingIndicator.SetActive(true);
                Debug.Log("DogListView: Loading indicator activated");
            }
            else
            {
                Debug.LogError("DogListView: Loading indicator is null!");
            }
            
            if (errorText != null)
            {
                errorText.SetActive(false);
                Debug.Log("DogListView: Error text deactivated");
            }
        }

        private void OnLoadingFinished(GameSignals.LoadingFinishedSignal signal)
        {
            Debug.Log("DogListView: Loading finished");
            if (loadingIndicator != null)
            {
                loadingIndicator.SetActive(false);
                Debug.Log("DogListView: Loading indicator deactivated");
            }
        }

        private void OnError(GameSignals.ErrorSignal signal)
        {
            Debug.Log($"DogListView: Error received - {signal.Message}");
            if (loadingIndicator != null)
            {
                loadingIndicator.SetActive(false);
                Debug.Log("DogListView: Loading indicator deactivated");
            }
            
            if (errorText != null)
            {
                errorText.SetActive(true);
                var errorTextComponent = errorText.GetComponent<TextMeshProUGUI>();
                if (errorTextComponent != null)
                {
                    errorTextComponent.text = signal.Message;
                    Debug.Log("DogListView: Error text updated");
                }
                else
                {
                    Debug.LogError("DogListView: TextMeshProUGUI component missing from errorText!");
                }
            }
        }

        private void OnBreedSelected(DogBreed breed)
        {
            Debug.Log($"Selected breed: {breed.name}");
            _signalBus.Fire(new GameSignals.DogBreedSelectedSignal(breed.id));
        }

        private void OnEnable()
        {
            if (scrollView != null)
            {
                Debug.Log($"ScrollView active: {scrollView.gameObject.activeInHierarchy}");
            }
            
            if (contentContainer != null)
            {
                Debug.Log($"Content active: {contentContainer.gameObject.activeInHierarchy}");
                Debug.Log($"Content size: {contentContainer.rect.size}");
            }
        }
    }
} 
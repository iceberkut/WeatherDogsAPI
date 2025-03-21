using System;
using System.Threading.Tasks;
using Models;
using Services;
using Signals;
using UnityEngine;
using Zenject;

namespace Controllers
{
    public class DogController : IInitializable, IDisposable
    {
        private readonly DogApiService _dogApiService;
        private readonly SignalBus _signalBus;
        private bool _isActive;
        private Task<DogBreed[]> _currentBreedsTask;

        [Inject]
        public DogController(DogApiService dogApiService, SignalBus signalBus)
        {
            _dogApiService = dogApiService;
            _signalBus = signalBus;
            Debug.Log("DogController: Constructor called");
        }

        public void Initialize()
        {
            Debug.Log("DogController: Initialize called");
            SubscribeToSignals();
        }

        public void Dispose()
        {
            Debug.Log("DogController: Dispose called");
            UnsubscribeFromSignals();
        }

        private void SubscribeToSignals()
        {
            Debug.Log("DogController: Subscribing to signals");
            _signalBus.Subscribe<GameSignals.DogTabActivatedSignal>(OnDogTabActivated);
            _signalBus.Subscribe<GameSignals.DogBreedSelectedSignal>(OnDogBreedSelected);
        }

        private void UnsubscribeFromSignals()
        {
            Debug.Log("DogController: Unsubscribing from signals");
            _signalBus.Unsubscribe<GameSignals.DogTabActivatedSignal>(OnDogTabActivated);
            _signalBus.Unsubscribe<GameSignals.DogBreedSelectedSignal>(OnDogBreedSelected);
        }

        private void OnDogTabActivated()
        {
            Debug.Log("DogController: Dog tab activated");
            _isActive = true;
            LoadBreeds();
        }

        private void OnDogBreedSelected(GameSignals.DogBreedSelectedSignal signal)
        {
            if (!_isActive) return;
            LoadBreedDetails(signal.BreedId);
        }

        private async void LoadBreeds()
        {
            if (!_isActive)
            {
                Debug.Log("DogController: LoadBreeds called but tab is not active");
                return;
            }

            try
            {
                Debug.Log("DogController: Starting breeds load");
                _signalBus.Fire(new GameSignals.LoadingStartedSignal());
                
                _currentBreedsTask = _dogApiService.GetBreedsAsync();
                Debug.Log("DogController: Waiting for breeds to load");
                var breeds = await _currentBreedsTask;
                Debug.Log($"DogController: Breeds loaded successfully, count: {breeds.Length}");

                if (!_isActive)
                {
                    Debug.Log("DogController: Tab deactivated during breeds load");
                    return;
                }

                Debug.Log($"DogController: Received {breeds.Length} breeds");
                _signalBus.Fire(new GameSignals.DogBreedsLoadedSignal(breeds));
                Debug.Log("DogController: DogBreedsLoadedSignal fired");
            }
            catch (Exception e)
            {
                Debug.LogError($"DogController: Error loading breeds - {e.Message}");
                Debug.LogError($"DogController: Stack trace - {e.StackTrace}");
                
                if (_isActive)
                {
                    _signalBus.Fire(new GameSignals.ErrorSignal("Failed to load dog breeds"));
                }
            }
            finally
            {
                if (_isActive)
                {
                    _signalBus.Fire(new GameSignals.LoadingFinishedSignal());
                    Debug.Log("DogController: Loading finished signal fired");
                }
            }
        }

        private async void LoadBreedDetails(string breedId)
        {
            if (!_isActive) return;

            try
            {
                Debug.Log($"DogController: Starting breed details load for {breedId}");
                _signalBus.Fire(new GameSignals.LoadingStartedSignal());
                
                var breed = await _dogApiService.GetBreedDetailsAsync(breedId);

                if (!_isActive)
                {
                    Debug.Log("DogController: Tab deactivated during breed details load");
                    return;
                }

                Debug.Log($"DogController: Received details for breed {breed.name}");
                _signalBus.Fire(new GameSignals.DogBreedDetailsLoadedSignal(breed));
            }
            catch (Exception e)
            {
                Debug.LogError($"DogController: Error loading breed details - {e.Message}");
                Debug.LogError($"DogController: Stack trace - {e.StackTrace}");
                
                if (_isActive)
                {
                    _signalBus.Fire(new GameSignals.ErrorSignal("Failed to load breed details"));
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
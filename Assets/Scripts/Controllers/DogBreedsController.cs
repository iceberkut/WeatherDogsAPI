using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using WeatherApp.Models;
using WeatherApp.Services;
using WeatherApp.Signals;
using Zenject;

namespace WeatherApp.Controllers
{
    public class DogBreedsController : IInitializable, IDisposable
    {
        private readonly NetworkService _networkService;
        private readonly SignalBus _signalBus;
        private NetworkRequest _currentBreedsRequest;
        private NetworkRequest _currentBreedDetailsRequest;


        public DogBreedsController(NetworkService networkService, SignalBus signalBus)
        {
            _networkService = networkService;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<DogBreedsTabActivatedSignal>(OnDogBreedsTabActivated);
            _signalBus.Subscribe<DogBreedsTabDeactivatedSignal>(OnDogBreedsTabDeactivated);
        }

        private void OnDogBreedsTabActivated()
        {

            LoadDogBreeds();
        }

        private void OnDogBreedsTabDeactivated()
        {

            CancelCurrentRequests();
        }

        private async void LoadDogBreeds()
        {
            try
            {
                _signalBus.Fire(new LoadingStartedSignal());
                
                var response = await _networkService.EnqueueRequest(
                    "https://api.thedogapi.com/v1/breeds",
                    CancelCurrentRequests
                );

                var breeds = new List<DogBreedModel>();
                for (int i = 0; i < 10; i++)
                {
                    breeds.Add(new DogBreedModel
                    {
                        Id = $"breed_{i}",
                        Name = $"Порода {i + 1}",
                        Description = $"Описание породы {i + 1}"
                    });
                }

                _signalBus.Fire(new DogBreedsLoadedSignal(breeds));
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading dog breeds: {e.Message}");
            }
            finally
            {
                _signalBus.Fire(new LoadingFinishedSignal());
            }
        }

        public async void LoadBreedDetails(string breedId)
        {
            try
            {
                _signalBus.Fire(new LoadingStartedSignal());
                
                var response = await _networkService.EnqueueRequest(
                    $"https://api.thedogapi.com/v1/breeds/{breedId}",
                    CancelCurrentRequests
                );

                var breedDetails = new DogBreedModel
                {
                    Id = breedId,
                    Name = $"Порода {breedId}",
                    Description = $"Подробное описание породы {breedId}"
                };

                _signalBus.Fire(new BreedDetailsLoadedSignal(breedDetails));
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading breed details: {e.Message}");
            }
            finally
            {
                _signalBus.Fire(new LoadingFinishedSignal());
            }
        }

        private void CancelCurrentRequests()
        {
            if (_currentBreedsRequest != null)
            {
                _networkService.CancelCurrentRequest();
                _currentBreedsRequest = null;
            }
            if (_currentBreedDetailsRequest != null)
            {
                _networkService.CancelCurrentRequest();
                _currentBreedDetailsRequest = null;
            }
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<DogBreedsTabActivatedSignal>(OnDogBreedsTabActivated);
            _signalBus.Unsubscribe<DogBreedsTabDeactivatedSignal>(OnDogBreedsTabDeactivated);
            CancelCurrentRequests();
        }
    }
} 
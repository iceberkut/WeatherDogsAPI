using System;
using System.Threading.Tasks;
using Core.RequestSystem;
using Models;
using UnityEngine;

namespace Services
{
    public class DogApiService
    {
        private readonly RequestQueue _requestQueue;
        private const string BASE_URL = "https://api.thedogapi.com/v1";

        public DogApiService(RequestQueue requestQueue)
        {
            _requestQueue = requestQueue;
        }

        public async Task<DogBreed[]> GetBreedsAsync()
        {
            try
            {
                Debug.Log("DogApiService: Requesting breeds list");
                var response = await _requestQueue.AddRequest<DogBreed[]>($"{BASE_URL}/breeds");
                Debug.Log($"DogApiService: Received {response.Length} breeds");
                foreach (var breed in response)
                {
                    Debug.Log($"DogApiService: Breed - {breed.name} (ID: {breed.id})");
                }
                return response;
            }
            catch (Exception e)
            {
                Debug.LogError($"DogApiService: Error getting breeds - {e.Message}");
                Debug.LogError($"DogApiService: Stack trace - {e.StackTrace}");
                throw;
            }
        }

        public async Task<DogBreed> GetBreedDetailsAsync(string breedId)
        {
            try
            {
                Debug.Log($"DogApiService: Requesting details for breed {breedId}");
                var response = await _requestQueue.AddRequest<DogBreed>($"{BASE_URL}/breeds/{breedId}");
                Debug.Log($"DogApiService: Received details for breed {response.name}");
                return response;
            }
            catch (Exception e)
            {
                Debug.LogError($"DogApiService: Error getting breed details - {e.Message}");
                throw;
            }
        }
    }
} 
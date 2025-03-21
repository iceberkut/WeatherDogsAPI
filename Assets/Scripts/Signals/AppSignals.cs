using Models;
using System.Collections.Generic;
using WeatherApp.Models;

namespace WeatherApp.Signals
{
    public class WeatherTabActivatedSignal { }
    public class WeatherTabDeactivatedSignal { }
    public class WeatherUpdatedSignal
    {
        public WeatherModel Weather { get; }

        public WeatherUpdatedSignal(WeatherModel weather)
        {
            Weather = weather;
        }
    }

    public class DogBreedsTabActivatedSignal { }
    public class DogBreedsTabDeactivatedSignal { }
    public class DogBreedsLoadedSignal
    {
        public List<DogBreedModel> Breeds { get; }

        public DogBreedsLoadedSignal(List<DogBreedModel> breeds)
        {
            Breeds = breeds;
        }
    }

    public class BreedDetailsLoadedSignal
    {
        public DogBreedModel Breed { get; }

        public BreedDetailsLoadedSignal(DogBreedModel breed)
        {
            Breed = breed;
        }
    }

    public class BreedSelectedSignal
    {
        public string BreedId { get; }

        public BreedSelectedSignal(string breedId)
        {
            BreedId = breedId;
        }
    }

    public class LoadingStartedSignal { }
    public class LoadingFinishedSignal { }
} 
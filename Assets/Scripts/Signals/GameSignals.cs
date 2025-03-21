using Zenject;

namespace Signals
{
    public class GameSignals
    {
        public class WeatherUpdatedSignal
        {
            public Models.WeatherModel Weather { get; }

            public WeatherUpdatedSignal(Models.WeatherModel weather)
            {
                Weather = weather;
            }
        }

        public class WeatherTabActivatedSignal { }
        public class WeatherTabDeactivatedSignal { }
        public class DogTabActivatedSignal { }
        public class DogBreedSelectedSignal
        {
            public string BreedId { get; }

            public DogBreedSelectedSignal(string breedId)
            {
                BreedId = breedId;
            }
        }
        public class DogBreedsLoadedSignal
        {
            public Models.DogBreed[] Breeds { get; }

            public DogBreedsLoadedSignal(Models.DogBreed[] breeds)
            {
                Breeds = breeds;
            }
        }
        public class DogBreedDetailsLoadedSignal
        {
            public Models.DogBreed Breed { get; }

            public DogBreedDetailsLoadedSignal(Models.DogBreed breed)
            {
                Breed = breed;
            }
        }
        public class LoadingStartedSignal { }
        public class LoadingFinishedSignal { }
        public class ErrorSignal
        {
            public string Message { get; }

            public ErrorSignal(string message)
            {
                Message = message;
            }
        }
        public class DogBreedsTabActivatedSignal { }
        public class DogBreedsTabDeactivatedSignal { }
    }
} 
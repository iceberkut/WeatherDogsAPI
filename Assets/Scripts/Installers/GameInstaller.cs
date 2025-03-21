using Core.RequestSystem;
using Controllers;
using Services;
using Signals;
using UnityEngine;
using Zenject;
using Views;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private WeatherView weatherView;
        [SerializeField] private TabNavigationView tabNavigationView;
        [SerializeField] private DogListView dogListView;
        [SerializeField] private DogBreedPopupView dogBreedPopupView;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            // Bind Signals
            Container.DeclareSignal<GameSignals.WeatherUpdatedSignal>();
            Container.DeclareSignal<GameSignals.WeatherTabActivatedSignal>();
            Container.DeclareSignal<GameSignals.WeatherTabDeactivatedSignal>();
            Container.DeclareSignal<GameSignals.DogTabActivatedSignal>();
            Container.DeclareSignal<GameSignals.DogBreedSelectedSignal>();
            Container.DeclareSignal<GameSignals.DogBreedsLoadedSignal>();
            Container.DeclareSignal<GameSignals.DogBreedDetailsLoadedSignal>();
            Container.DeclareSignal<GameSignals.LoadingStartedSignal>();
            Container.DeclareSignal<GameSignals.LoadingFinishedSignal>();
            Container.DeclareSignal<GameSignals.ErrorSignal>();
            Container.DeclareSignal<ProcessNextRequestSignal>().OptionalSubscriber();

            // Bind Services
            Container.Bind<WeatherService>().AsSingle();
            Container.Bind<WeatherApiService>().AsSingle();
            Container.Bind<DogApiService>().AsSingle();
            Container.Bind<RequestQueue>().AsSingle();

            // Bind Controllers
            Container.BindInterfacesAndSelfTo<WeatherController>().AsSingle();
            Container.BindInterfacesAndSelfTo<DogController>().AsSingle();

            // Bind Views last
            Container.Bind<WeatherView>().FromInstance(weatherView).AsSingle();
            Container.Bind<TabNavigationView>().FromInstance(tabNavigationView).AsSingle();
            Container.Bind<DogListView>().FromInstance(dogListView).AsSingle();
            Container.Bind<DogBreedPopupView>().FromInstance(dogBreedPopupView).AsSingle();
        }
    }
} 
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using Signals;

namespace Managers
{
    public class TabManager : MonoBehaviour
    {
        [SerializeField] private Button weatherTabButton;
        [SerializeField] private Button dogBreedsTabButton;
        [SerializeField] private GameObject weatherTab;
        [SerializeField] private GameObject dogBreedsTab;
        [SerializeField] private TextMeshProUGUI weatherTabText;
        [SerializeField] private TextMeshProUGUI dogBreedsTabText;

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
            SetupButtons();
        }

        private void SetupButtons()
        {
            weatherTabButton.onClick.AddListener(() => SwitchTab(0));
            dogBreedsTabButton.onClick.AddListener(() => SwitchTab(1));
        }

        private void SwitchTab(int tabIndex)
        {
            bool isWeatherTab = tabIndex == 0;

            weatherTab.SetActive(isWeatherTab);
            dogBreedsTab.SetActive(!isWeatherTab);

            if (isWeatherTab)
            {
                _signalBus.Fire(new GameSignals.WeatherTabActivatedSignal());
                _signalBus.Fire(new GameSignals.DogBreedsTabDeactivatedSignal());
            }
            else
            {
                _signalBus.Fire(new GameSignals.WeatherTabDeactivatedSignal());
                _signalBus.Fire(new GameSignals.DogBreedsTabActivatedSignal());
            }
        }

        private void Start()
        {
            SwitchTab(0);
        }
    }
} 
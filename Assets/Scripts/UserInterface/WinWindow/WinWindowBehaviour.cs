using DIFramework;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UserInterface.WinWindow
{
    public class WinWindowBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject _actualWindow;
        [SerializeField] private Text _winText;
        [Inject] private SignalBus _signalBus;

        [Inject]
        private void Initialize()
        {
            _signalBus.Subscribe<EndGameSignal>(ShowWinWindow);
            _signalBus.Subscribe<StartGameSignal>(Hide);
            Hide();
        }

        private void Hide()
        {
            _actualWindow.SetActive(false);
        }

        private void ShowWinWindow(EndGameSignal obj)
        {
            _actualWindow.SetActive(true);
            _winText.text = obj.Color.ToString() + " player won!";
        }
    }
}
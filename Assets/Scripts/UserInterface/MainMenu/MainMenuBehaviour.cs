using DIFramework;
using UnityEngine;
using Zenject;

namespace UserInterface.MainMenu
{
    public class MainMenuBehaviour : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;
        [SerializeField] private GameObject _actualMenu;

        [Inject]
        private void Initialize()
        {
            _signalBus.Subscribe<StartGameSignal>(() => _actualMenu.SetActive(false));
        }
    }
}
using System.Runtime.InteropServices;
using DIFramework;
using UnityEngine;
using Zenject;

namespace UserInterface
{
    public class SwitchButton : MonoBehaviour
    {
        [Inject] private GameState _gameState;

        [Inject]
        private void Initialize(SignalBus signalBus)
        {
            signalBus.Subscribe<StartGameSignal>(() => gameObject.SetActive(false));
            signalBus.Subscribe<EndGameSignal>(() => gameObject.SetActive(true));
        }
        
        public void OnClick()
        {
            _gameState.Switch();
        }
    }
}
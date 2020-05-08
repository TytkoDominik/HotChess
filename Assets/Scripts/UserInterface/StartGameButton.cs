using DIFramework;
using UnityEngine;
using Zenject;

namespace UserInterface
{
    public class StartGameButton : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;

        public void OnClick()
        {
            _signalBus.Fire<StartGameSignal>();
        }
    }
}
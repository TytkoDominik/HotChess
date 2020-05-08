using BoardLogic;
using DIFramework;
using UnityEngine;
using Zenject;

namespace UserInterface
{
    public class PromotionWindowBehaviour : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;
        [SerializeField] private GameObject Window;

        [Inject]
        private void Initialize()
        {
            _signalBus.Subscribe<OpenPromotionWindowSignal>(Show);
            Hide();
        }

        private void Show()
        {
            Window.SetActive(true);
        }

        private void Hide()
        {
            Window.SetActive(false);
        }

        public void ChoosePieceType(PieceType type)
        {
            _signalBus.Fire(new PromotionTypeChosenSignal(type));
            Hide();
        }
    }
}
using DIFramework;
using Extensions;
using Zenject;

namespace BoardLogic
{
    public class ActivePlayer
    {
        private PieceColor _activePlayerColor = PieceColor.White;

        [Inject]
        private void Initialize(SignalBus signalBus)
        {
            signalBus.Subscribe<ChangePlayerSignal>(ChangePlayer);
        }
        
        public PieceColor GetActivePlayerColor()
        {
            return _activePlayerColor;
        }

        private void ChangePlayer()
        {
            _activePlayerColor = _activePlayerColor.Opposite();
        }
    }
}
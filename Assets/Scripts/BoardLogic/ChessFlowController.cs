using System;
using DG.Tweening;
using DIFramework;
using Extensions;
using Zenject;

namespace BoardLogic
{
    public class ChessFlowController
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private BoardVisualController _boardVisualController;
        [Inject] private HighlightController _highlightController;
        [Inject] private PromotionBehaviour _promotionBehaviour;
        private float _delay = 0.5f;
        private Action _changePlayerAction;
        private Action _promoteAction;

        [Inject]
        private void Initialize()
        {
            _signalBus.Subscribe<PerformMoveSignal>(PerformMove);
            _changePlayerAction = ChangePlayer;
        }

        private void PerformMove(PerformMoveSignal signal)
        {
            _highlightController.ClearHighlightedData();

            if (signal.Move.GetType() == typeof(KillingMove))
            {
                PerformKillingMove((KillingMove)signal.Move);
            }
            else
            {
                PerformNormalMove(signal.Move);
            }
        }

        private void PerformNormalMove(Move move)
        {
            _boardVisualController.MovePiece(move, () => CheckForGameEnd(move));
        }

        private void PerformKillingMove(KillingMove move)
        {
            _boardVisualController.PerformKillingAnimation(move.KilledFigurePosition, move.Color.Opposite(), _delay, () => CheckForGameEnd(move));
            _boardVisualController.MovePiece(move, () => { });

        }

        private void TryPerformPromotion(Move move)
        {
            _promotionBehaviour.TryPerformPromotionDialogue(move, _changePlayerAction);
        }

        private void CheckForGameEnd(Move move)
        {
            if (move.IsMate)
            {
                _signalBus.Fire(new EndGameSignal(move.Color));
            }
            else
            {
               TryPerformPromotion(move); 
            }
        }
        
        private void ChangePlayer()
        {
            _signalBus.Fire<ChangePlayerSignal>();
        }
    }
}
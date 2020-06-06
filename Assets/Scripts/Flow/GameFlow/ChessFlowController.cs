using System;
using DIFramework;
using Extensions;
using Flow.Highlight;
using GameLogic.Board;
using UserInterface;
using Zenject;

namespace Flow.GameFlow
{
    public class ChessFlowController
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private BoardVisualController _boardVisualController;
        [Inject] private HighlightController _highlightController;
        [Inject] private PromotionBehaviour _promotionBehaviour;
        [Inject] private GameState _gameState;
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
            _gameState.Player(signal.Move.Color).CurrentGameStats.MovesPerformed++;
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
                _gameState.Player(move.Color).CurrentGameStats.Won = true;
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
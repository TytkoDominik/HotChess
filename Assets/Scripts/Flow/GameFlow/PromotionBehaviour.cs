using System;
using DIFramework;
using GameLogic.Board;
using UnityEngine;
using Zenject;

namespace Flow.GameFlow
{
    public class PromotionBehaviour
    {
        [Inject] private SignalBus _signalBus;
        private Action _callback;
        private Vector2 _position;
        private PieceColor _color;

        [Inject]
        private void Initialize()
        {
            _signalBus.Subscribe<PromotionTypeChosenSignal>(PromotePawn);
        }

        private void PromotePawn(PromotionTypeChosenSignal obj)
        {
            _signalBus.Fire(new PromotionSignal(_position, obj.Type, _color));
            _callback?.Invoke();
        }

        public void TryPerformPromotionDialogue(Move move, Action callback)
        {
            if (!IsMovePromotionMove(move))
            {
                callback.Invoke();
                return;
            }

            _callback = callback;
            _position = move.To;
            _color = move.Color;
            _signalBus.Fire<OpenPromotionWindowSignal>();
        }

        private bool IsMovePromotionMove(Move move)
        {
            return move.Type == PieceType.Pawn && ((int)move.To.y == 0 || (int)move.To.y == 7);
        }
    }
}
using DIFramework;
using GameLogic.Board;
using UnityEngine;
using Zenject;

namespace Flow.GameFlow
{
    public class BoardInteraction : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;
        private BoardPosition _boardPosition;

        [Inject]
        private void Initialize()
        {
            _boardPosition = GetComponent<BoardPosition>();
        }

        private void OnMouseEnter()
        {
            _signalBus.Fire(new BoardMouseEnterSignal(_boardPosition.GetBoardPosition()));
        }

        private void OnMouseDown()
        {
            _signalBus.Fire( new BoardMouseDownSignal(_boardPosition.GetBoardPosition()));
        }

        private void OnMouseExit()
        {
            _signalBus.Fire(new BoardMouseExitSignal(_boardPosition.GetBoardPosition()));
        }
    }
}
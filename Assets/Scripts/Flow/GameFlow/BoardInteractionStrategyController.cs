using DIFramework;
using Flow.Highlight;
using GameLogic.GameRules;
using Zenject;

namespace Flow.GameFlow
{
    public class BoardInteractionStrategyController
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private HighlightController _highlightController;
        [Inject] private ChessRulesCore _chessRulesCore;

        [Inject]
        private void Initialize()
        {
            _signalBus.Subscribe<BoardMouseEnterSignal>(MouseEnterStrategy);
            _signalBus.Subscribe<BoardMouseExitSignal>(MouseExitStrategy);
            _signalBus.Subscribe<BoardMouseDownSignal>(MouseDownStrategy);
        }

        private void MouseDownStrategy(BoardMouseDownSignal obj)
        {
            _chessRulesCore.PositionWasClicked(obj.Position);
        }

        private void MouseExitStrategy(BoardMouseExitSignal obj)
        {
            _highlightController.ResetHighlightSelection();
        }

        private void MouseEnterStrategy(BoardMouseEnterSignal obj)
        {
            _highlightController.HighlightSelection(obj.Position);
        }
    }
}
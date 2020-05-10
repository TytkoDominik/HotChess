using System.Collections.Generic;
using System.Linq;
using DIFramework;
using GameLogic.Board;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Flow.Highlight
{
    public class HighlightController
    {
        [Inject] private SignalBus _signalBus;
        private List<HighlightData> _highlightedData = new List<HighlightData>();

        public void HighlightSelection(Vector2 position)
        {
            HighlightData[] highlightedDataArray = new HighlightData[_highlightedData.Count];
            _highlightedData.CopyTo(highlightedDataArray);
            List<HighlightData> positionsToHighlight = highlightedDataArray.ToList();

            positionsToHighlight.Add(new HighlightData(BoardHighlightType.Selected, position));
            
            _signalBus.Fire(new HighlightSignal(positionsToHighlight));
        }

        public void SetHighlightedData(List<Move> moves)
        {
            _highlightedData.Clear();

            if (moves.IsEmpty())
            {
                return;
            }
            
            foreach (var move in moves)
            {
                var type = move.GetType() == typeof(KillingMove) ? BoardHighlightType.Attack : BoardHighlightType.Move;
                
                _highlightedData.Add(new HighlightData(type, move.To));
            }
            
            HighlightSelection(moves.First().From);
        }

        public void ResetHighlightSelection()
        {
            _signalBus.Fire(new HighlightSignal(_highlightedData));
        }

        public void ClearHighlightedData()
        {
            _highlightedData.Clear();
            _signalBus.Fire(new HighlightSignal(_highlightedData));
        }
    }

    public class HighlightData
    {
        public BoardHighlightType Type;
        public Vector2 Position;

        public HighlightData(BoardHighlightType type, Vector2 position)
        {
            Type = type;
            Position = position;
        }
    }
    
    public enum BoardHighlightType
    {
        None,
        Selected,
        Move,
        Attack
    }
}
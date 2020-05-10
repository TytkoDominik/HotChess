using System.Collections.Generic;
using Flow.Highlight;
using GameLogic.Board;
using UnityEngine;

namespace DIFramework
{
    public class HighlightSignal
    {
        public readonly List<HighlightData> SelectionData;

        public HighlightSignal(List<HighlightData> selectionData)
        {
            SelectionData = selectionData;
        }
    }
    
    public class BoardMouseEnterSignal
    {
        public readonly Vector2 Position;

        public BoardMouseEnterSignal(Vector2 position)
        {
            Position = position;
        }
    }
        
    public class BoardMouseExitSignal
    {
        public readonly Vector2 Position;

        public BoardMouseExitSignal(Vector2 position)
        {
            Position = position;
        }
    }
    
    public class BoardMouseDownSignal
    {
        public readonly Vector2 Position;

        public BoardMouseDownSignal(Vector2 position)
        {
            Position = position;
        }
    }

    public class ChangePlayerSignal
    {
    }

    public class PerformMoveSignal
    {
        public readonly Move Move;

        public PerformMoveSignal(Move move)
        {
            Move = move;
        }
    }

    public class OpenPromotionWindowSignal
    {
    }

    public class PromotionTypeChosenSignal
    {
        public readonly PieceType Type;

        public PromotionTypeChosenSignal(PieceType type)
        {
            Type = type;
        }
    }

    public class PromotionSignal
    {
        public readonly Vector2 Position;
        public readonly PieceType Type;
        public readonly PieceColor Color;

        public PromotionSignal(Vector2 position, PieceType type, PieceColor color)
        {
            Position = position;
            Type = type;
            Color = color;
        }
    }

    public class CreateBoardSignal
    {
        public readonly Board Board;

        public CreateBoardSignal(Board board)
        {
            Board = board;
        }
    }

    public class StartGameSignal
    {
    }

    public class EndGameSignal
    {
        public readonly PieceColor Color;

        public EndGameSignal(PieceColor color)
        {
            Color = color;
        }
    }
}
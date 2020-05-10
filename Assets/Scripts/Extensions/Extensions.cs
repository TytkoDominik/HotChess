using GameLogic.Board;
using UnityEngine;

namespace Extensions
{
    public static class Extensions
    {
        private const int AsciiACode = 65;
        
        public static bool IsOppositeColorAs(this PieceColor first, PieceColor second)
        {
            if (first == PieceColor.None || second == PieceColor.None)
            {
                return false;
            }

            return first != second;
        }

        public static PieceColor Opposite(this PieceColor color)
        {
            return color == PieceColor.Black ? PieceColor.White : PieceColor.Black;
        }
        
        public static string GetNotationFromPosition(this Vector2 position)
        {
            return (char) (position.x + AsciiACode) + (position.y + 1).ToString();
        }
    }
}
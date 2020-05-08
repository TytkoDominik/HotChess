using BoardLogic;

namespace Extensions
{
    public static class Extensions
    {
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
    }
}
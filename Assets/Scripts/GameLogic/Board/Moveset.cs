using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace GameLogic.Board
{
    public abstract class Moveset
    {
        public abstract List<Move> GetPossibleMoves(Vector2 startingPosition, Board board);
        public abstract List<Move> GetFightingMoves(Vector2 startingPosition, Board board);
        protected List<Move> GetPossibleMovesInDirection(Vector2 startingPosition, PieceColor color, Board board, Vector2 direction)
        {
            var moves = new List<Move>();
            var checkPosition = startingPosition + direction;
            
            while (board.IsPositionAvailableFor(color, checkPosition))
            {
                
                if (board.IsPositionOccupiedByColor(checkPosition, color.Opposite()))
                {
                    moves.Add(new KillingMove(startingPosition, checkPosition, board.GetPieceData(startingPosition).Type, color, checkPosition));
                    break;
                }
                else
                {
                    moves.Add(new Move(startingPosition, checkPosition, board.GetPieceData(startingPosition).Type, color));
                }

                
                checkPosition += direction;
            }

            return moves;
        }
        
        protected void TryAddPositionToMoveset(int xMove, int yMove, PieceColor color, Vector2 startingPosition,
            Board board, List<Move> moves)
        {
            var checkingPosition = startingPosition + new Vector2(xMove, yMove);
            
            if (board.IsPositionAvailableFor(color, checkingPosition))
            {
                if (board.GetPieceColorFromPosition(checkingPosition).IsOppositeColorAs(color))
                {
                    moves.Add(new KillingMove(startingPosition, checkingPosition, 
                        board.GetPieceData(startingPosition).Type, color, checkingPosition));
                }
                else
                {
                    moves.Add(new Move(startingPosition, checkingPosition, 
                        board.GetPieceData(startingPosition).Type, color));
                }
            }
        }
    }

    public class PawnMoveset : Moveset
    {
        public override List<Move> GetPossibleMoves(Vector2 startingPosition, Board board)
        {
            var pawnData = board.GetPieceData(startingPosition);

            if (pawnData == null)
            {
                return null;
            }

            var moves = new List<Move>();
            var color = board.GetPieceColorFromPosition(startingPosition);
            
            var movingDirection = pawnData.Color == PieceColor.White ? Vector2.up : Vector2.down;

            if (board.IsPositionEmpty(startingPosition + movingDirection))
            {
                
                
                var move = new Move(startingPosition, startingPosition + movingDirection, PieceType.Pawn, color);
                moves.Add(move);
            }

            if (!pawnData.MovedAlready && board.IsPositionEmpty(startingPosition + 2 * movingDirection))
            {
                var move = new Move(startingPosition, startingPosition + 2 * movingDirection, PieceType.Pawn, color);
                moves.Add(move);
            }

            moves.AddRange(GetFightingMoves(startingPosition, board));

            return moves;
        }

        public override List<Move> GetFightingMoves(Vector2 startingPosition, Board board)
        {
            var pawnData = board.GetPieceData(startingPosition);

            if (pawnData == null)
            {
                return null;
            }

            var moves = new List<Move>();
            var color = board.GetPieceColorFromPosition(startingPosition);
            
            var movingDirection = pawnData.Color == PieceColor.White ? Vector2.up : Vector2.down;
            
            var leftCorner = startingPosition + Vector2.left + movingDirection;
            var rightCorner = startingPosition + Vector2.right + movingDirection;
            
            if (pawnData.Color.IsOppositeColorAs(board.GetPieceColorFromPosition(leftCorner)))
            {
                var move = new KillingMove(startingPosition, leftCorner, PieceType.Pawn, color, leftCorner);
                moves.Add(move);
            }
            
            if (pawnData.Color.IsOppositeColorAs(board.GetPieceColorFromPosition(rightCorner)))
            {
                var move = new KillingMove(startingPosition, rightCorner, PieceType.Pawn, color, rightCorner);
                moves.Add(move);
            }
            
            if (pawnData.Color.IsOppositeColorAs(board.GetPieceColorFromPosition(startingPosition + Vector2.left)) &&
                                                 board.IsPositionEmpty(leftCorner))
            {
                var move = new KillingMove(startingPosition, leftCorner, PieceType.Pawn, color, startingPosition + Vector2.left);
                moves.Add(move);
            }
            
            if (pawnData.Color.IsOppositeColorAs(board.GetPieceColorFromPosition(startingPosition + Vector2.right)) &&
                board.IsPositionEmpty(rightCorner))
            {
                var move = new KillingMove(startingPosition, rightCorner, PieceType.Pawn, color, startingPosition + Vector2.right);
                moves.Add(move);
            }

            return moves;
        }
    }
    
    public class RookMoveset : Moveset
    {
        public override List<Move> GetPossibleMoves(Vector2 startingPosition, Board board)
        {
            var moves = new List<Move>();
            
            var towerData = board.GetPieceData(startingPosition);

            if (towerData == null)
            {
                return null;
            }

            moves.AddRange(GetPossibleMovesInDirection(startingPosition, towerData.Color, board, Vector2.up));
            moves.AddRange(GetPossibleMovesInDirection(startingPosition, towerData.Color, board, Vector2.down));
            moves.AddRange(GetPossibleMovesInDirection(startingPosition, towerData.Color, board, Vector2.left));
            moves.AddRange(GetPossibleMovesInDirection(startingPosition, towerData.Color, board, Vector2.right));

            return moves;
        }

        public override List<Move> GetFightingMoves(Vector2 startingPosition, Board board)
        {
            return GetPossibleMoves(startingPosition, board);
        }
    }

    public class BishopMoveset : Moveset
    {
        public override List<Move> GetPossibleMoves(Vector2 startingPosition, Board board)
        {
            var moves = new List<Move>();
            
            var towerData = board.GetPieceData(startingPosition);

            if (towerData == null)
            {
                return null;
            }

            moves.AddRange(GetPossibleMovesInDirection(startingPosition, towerData.Color, board, new Vector2(1,1)));
            moves.AddRange(GetPossibleMovesInDirection(startingPosition, towerData.Color, board, new Vector2(-1,1)));
            moves.AddRange(GetPossibleMovesInDirection(startingPosition, towerData.Color, board, new Vector2(1,-1)));
            moves.AddRange(GetPossibleMovesInDirection(startingPosition, towerData.Color, board, new Vector2(-1,-1)));

            return moves;
        }

        public override List<Move> GetFightingMoves(Vector2 startingPosition, Board board)
        {
            return GetPossibleMoves(startingPosition, board);
        }
    }

    public class KnightMoveset : Moveset
    {
        public override List<Move> GetPossibleMoves(Vector2 startingPosition, Board board)
        {
            var moves = new List<Move>();
            var data = board.GetPieceData(startingPosition);
            
            TryAddPositionToMoveset(1, 2, data.Color, startingPosition, board, moves);
            TryAddPositionToMoveset(-1, 2, data.Color, startingPosition, board, moves);
            TryAddPositionToMoveset(-1, -2, data.Color, startingPosition, board, moves);
            TryAddPositionToMoveset(1, -2, data.Color, startingPosition, board, moves);
            
            TryAddPositionToMoveset(2,  1, data.Color, startingPosition, board, moves);
            TryAddPositionToMoveset( 2, -1,data.Color, startingPosition, board, moves);
            TryAddPositionToMoveset( -2,-1, data.Color, startingPosition, board, moves);
            TryAddPositionToMoveset(-2, 1, data.Color, startingPosition, board, moves);


            return moves;
        }

        public override List<Move> GetFightingMoves(Vector2 startingPosition, Board board)
        {
            return GetPossibleMoves(startingPosition, board);
        }
    }

    public class QueenMoveset : Moveset
    {
        public override List<Move> GetPossibleMoves(Vector2 startingPosition, Board board)
        {
            var moves = new List<Move>();
            
            var towerData = board.GetPieceData(startingPosition);

            if (towerData == null)
            {
                return null;
            }

            moves.AddRange(GetPossibleMovesInDirection(startingPosition, towerData.Color, board, Vector2.up));
            moves.AddRange(GetPossibleMovesInDirection(startingPosition, towerData.Color, board, Vector2.down));
            moves.AddRange(GetPossibleMovesInDirection(startingPosition, towerData.Color, board, Vector2.left));
            moves.AddRange(GetPossibleMovesInDirection(startingPosition, towerData.Color, board, Vector2.right));
            
            moves.AddRange(GetPossibleMovesInDirection(startingPosition, towerData.Color, board, new Vector2(1,1)));
            moves.AddRange(GetPossibleMovesInDirection(startingPosition, towerData.Color, board, new Vector2(-1,1)));
            moves.AddRange(GetPossibleMovesInDirection(startingPosition, towerData.Color, board, new Vector2(1,-1)));
            moves.AddRange(GetPossibleMovesInDirection(startingPosition, towerData.Color, board, new Vector2(-1,-1)));

            return moves;
        }

        public override List<Move> GetFightingMoves(Vector2 startingPosition, Board board)
        {
            return GetPossibleMoves(startingPosition, board);
        }
    }
    
    public class KingMoveset : Moveset
    {
        public override List<Move> GetPossibleMoves(Vector2 startingPosition, Board board)
        {
            var moves = new List<Move>();
            var data = board.GetPieceData(startingPosition);
            
            TryAddPositionToMoveset(1, 1, data.Color, startingPosition, board, moves);
            TryAddPositionToMoveset(-1, 1, data.Color, startingPosition, board, moves);
            TryAddPositionToMoveset(-1, -1, data.Color, startingPosition, board, moves);
            TryAddPositionToMoveset(1, -1, data.Color, startingPosition, board, moves);
            
            TryAddPositionToMoveset(0,  1, data.Color, startingPosition, board, moves);
            TryAddPositionToMoveset( 0, -1,data.Color, startingPosition, board, moves);
            TryAddPositionToMoveset( 1,0, data.Color, startingPosition, board, moves);
            TryAddPositionToMoveset(-1, 0, data.Color, startingPosition, board, moves);

            return moves;
        }

        public override List<Move> GetFightingMoves(Vector2 startingPosition, Board board)
        {
            return GetPossibleMoves(startingPosition, board);
        }
    }
    
    public class Move
    {
        public PieceColor Color;
        public PieceType Type;
        public Vector2 From;
        public Vector2 To;
        public bool IsCheck = false;
        public bool IsMate = false;

        public Move(Vector2 from, Vector2 to, PieceType type, PieceColor color)
        {
            To = to;
            From = from;
            Type = type;
            Color = color;
        }
    }
    
    public class DoubleMove : Move
    {
        public Vector2 SecondFrom;
        public Vector2 SecondTo;
        
        public DoubleMove(Vector2 from, Vector2 to, PieceColor color, Vector2 secondFrom, Vector2 secondTo) : base(from, to, PieceType.King, color)
        {
            SecondFrom = secondFrom;
            SecondTo = secondTo;
        }
    }
    
    public class KillingMove : Move
    {
        public Vector2 KilledFigurePosition;
        
        public KillingMove(Vector2 from, Vector2 to, PieceType type, PieceColor color, Vector2 killedFigurePosition) : base(from, to, type, color)
        {
            KilledFigurePosition = killedFigurePosition;
        }
    }
}
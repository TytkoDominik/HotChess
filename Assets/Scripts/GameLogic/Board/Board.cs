using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameLogic.Board
{
    public class Board
    {
        public List<ChessPieceData> ChessPieceData;

        public Board()
        {
            ChessPieceData = new List<ChessPieceData>();
            for (int i = 0; i < 8; i++)
            {
                ChessPieceData.Add(new ChessPieceData(new Vector2(i, 1), PieceType.Pawn, PieceColor.White));
                ChessPieceData.Add(new ChessPieceData(new Vector2(i, 6), PieceType.Pawn, PieceColor.Black));
            }
            
            ChessPieceData.Add(new ChessPieceData(new Vector2(0, 0), PieceType.Rook, PieceColor.White));
            ChessPieceData.Add(new ChessPieceData(new Vector2(0, 7), PieceType.Rook, PieceColor.Black));
            ChessPieceData.Add(new ChessPieceData(new Vector2(7, 0), PieceType.Rook, PieceColor.White));
            ChessPieceData.Add(new ChessPieceData(new Vector2(7, 7), PieceType.Rook, PieceColor.Black));
            
            ChessPieceData.Add(new ChessPieceData(new Vector2(1, 0), PieceType.Knight, PieceColor.White));
            ChessPieceData.Add(new ChessPieceData(new Vector2(1, 7), PieceType.Knight, PieceColor.Black));
            ChessPieceData.Add(new ChessPieceData(new Vector2(6, 0), PieceType.Knight, PieceColor.White));
            ChessPieceData.Add(new ChessPieceData(new Vector2(6, 7), PieceType.Knight, PieceColor.Black));
            
            ChessPieceData.Add(new ChessPieceData(new Vector2(2, 0), PieceType.Bishop, PieceColor.White));
            ChessPieceData.Add(new ChessPieceData(new Vector2(2, 7), PieceType.Bishop, PieceColor.Black));
            ChessPieceData.Add(new ChessPieceData(new Vector2(5, 0), PieceType.Bishop, PieceColor.White));
            ChessPieceData.Add(new ChessPieceData(new Vector2(5, 7), PieceType.Bishop, PieceColor.Black));
            
            ChessPieceData.Add(new ChessPieceData(new Vector2(3, 0), PieceType.Queen, PieceColor.White));
            ChessPieceData.Add(new ChessPieceData(new Vector2(3, 7), PieceType.Queen, PieceColor.Black));
            
            ChessPieceData.Add(new ChessPieceData(new Vector2(4, 0), PieceType.King, PieceColor.White));
            ChessPieceData.Add(new ChessPieceData(new Vector2(4, 7), PieceType.King, PieceColor.Black));
        }

        public Board(Board board)
        {
            ChessPieceData = new List<ChessPieceData>();
            
            foreach (var chessPieceData in board.ChessPieceData)
            {
                ChessPieceData.Add(new ChessPieceData(chessPieceData));
            }
        }
        
        public ChessPieceData GetPieceData(Vector2 position)
        {
            return ChessPieceData.FirstOrDefault(p => p.Position == position);
        }

        public PieceColor GetPieceColorFromPosition(Vector2 position)
        {
            var data = GetPieceData(position);

            if (data == null)
            {
                return PieceColor.None;
            }

            return data.Color;
        }

        public bool IsPositionAvailableFor(PieceColor color, Vector2 position)
        {
            return PositionInsideBoard(position) && color != GetPieceColorFromPosition(position);
        }

        public bool IsPositionEmpty(Vector2 position)
        {
            return PositionInsideBoard(position) && GetPieceColorFromPosition(position) == PieceColor.None;
        }

        public bool PositionInsideBoard(Vector2 position)
        {
            return position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8;
        }

        public bool IsPositionOccupiedByColor(Vector2 position, PieceColor color)
        {
            var data = ChessPieceData.FirstOrDefault(d => d.Position == position);
            return data != null && data.Color == color;
        }

        public void MovePiece(Move move)
        {
            var toData = ChessPieceData.FirstOrDefault(d => d.Position == move.To);
            
            if (toData != null)
            {
                ChessPieceData.Remove(toData);
            }

            var fromData = ChessPieceData.FirstOrDefault(d => d.Position == move.From);
            
            fromData.Position = move.To;
            fromData.MovedAlready = true;
        }

        public Vector2 KingPosition(PieceColor color)
        {
            return ChessPieceData.First(d => d.Type == PieceType.King && d.Color == color).Position;
        }

        public void PromotePawn(Vector2 position, PieceType type)
        {
            var pawnData = ChessPieceData.First(d => d.Position == position);
            pawnData.Type = type;
        }
    }
    
    [Serializable]
    public class ChessPieceData
    {
        public Vector2 Position;
        public PieceType Type;
        public PieceColor Color;
        public bool MovedAlready;

        public ChessPieceData(Vector2 position, PieceType type, PieceColor color)
        {
            Position = position;
            Type = type;
            Color = color;
            MovedAlready = false;
        }

        public ChessPieceData(ChessPieceData data)
        {
            Position = data.Position;
            Type = data.Type;
            Color = data.Color;
            MovedAlready = data.MovedAlready;
        }
    }

    public enum PieceType
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }

    public enum PieceColor
    {
        None = 0,
        Black = -1,
        White = 1
    }
}
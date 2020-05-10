using System.Collections.Generic;
using System.Linq;
using DIFramework;
using Extensions;
using Flow.Highlight;
using GameLogic.Board;
using GameLogic.PlayerData;
using ModestTree;
using UnityEngine;
using Zenject;

namespace GameLogic.GameRules
{
    public class ChessRulesCore
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private HighlightController _highlightController;
        [Inject] private ActivePlayer _activePlayer;
        public Board.Board Board = new Board.Board();
        private Dictionary<PieceType, Moveset> _movesets = new Dictionary<PieceType, Moveset>();
        private PieceColor ActivePlayerColor => _activePlayer.GetActivePlayerColor();
        private List<Move> _possibleLegalMoves = new List<Move>();

        [Inject]
        public void Initialize()
        {
            _movesets.Add(PieceType.Pawn, new PawnMoveset());
            _movesets.Add(PieceType.Rook, new RookMoveset());
            _movesets.Add(PieceType.Knight, new KnightMoveset());
            _movesets.Add(PieceType.Bishop, new BishopMoveset());
            _movesets.Add(PieceType.Queen, new QueenMoveset());
            _movesets.Add(PieceType.King, new KingMoveset());
            
            _signalBus.Subscribe<PromotionSignal>(PromotePawn);
            _signalBus.Subscribe<StartGameSignal>(StartGame);
        }

        private void StartGame()
        {
            Board = new Board.Board();
            _signalBus.Fire(new CreateBoardSignal(Board));
        }

        private void PromotePawn(PromotionSignal obj)
        {
            Board.PromotePawn(obj.Position, obj.Type);
        }

        public void PositionWasClicked(Vector2 position)
        {
            if (CouldMovePieceOnPosition(position))
            {
                SelectBoardPiece(position);
                return;
            }
            
            if (_possibleLegalMoves.Any(m => m.To == position))
            {
                var move = _possibleLegalMoves.FirstOrDefault(m => m.To == position);
                PerformMove(move);
            }
        }

        private void PerformMove(Move move)
        {
            Board.MovePiece(move);

            if (IsCheck(move.Color.Opposite(), Board))
            {
                move.IsCheck = true;
            }

            if (IsCheckMate(move.Color.Opposite(), Board))
            {
                move.IsMate = true;
            }
            
            _signalBus.Fire(new PerformMoveSignal(move));
            
            _possibleLegalMoves.Clear();
        }

        private void SelectBoardPiece(Vector2 position)
        {
            _possibleLegalMoves = GetLegalMoves(position, Board);
            _highlightController.SetHighlightedData(_possibleLegalMoves);
        }

        private bool CouldMovePieceOnPosition(Vector2 position)
        {
            return Board.IsPositionOccupiedByColor(position, ActivePlayerColor);
        }

        private List<Move> GetLegalMoves(Vector2 position, Board.Board board)
        {
            var possibleMoves = GetPossibleMoves(position, board);

            var illegalMoves = possibleMoves.Where(IsIllegalMove).ToList();

            illegalMoves.ForEach(m => possibleMoves.Remove(m));
            return possibleMoves;
        }
        
        private List<Move> GetPossibleMoves(Vector2 position, Board.Board board)
        {
            var data = board.GetPieceData(position);
            
            if (data == null)
            {
                return new List<Move>();
            }

            var type = data.Type;
            return _movesets[type].GetPossibleMoves(position, board);
        }

        private List<Move> GetThreatMoves(Vector2 position, Board.Board board)
        {
            var data = board.GetPieceData(position);
            
            if (data == null)
            {
                return new List<Move>();
            }

            var type = data.Type;
            return _movesets[type].GetFightingMoves(position, board);
        }

        private bool IsIllegalMove(Move move)
        {
            var tempBoard = new Board.Board(Board);
            tempBoard.MovePiece(move);

            return IsCheck(move.Color, tempBoard);
        }

        private bool IsCheck(PieceColor color, Board.Board board)
        {
            return GetThreatPositionsForColor(color, board).Any(p => p == board.KingPosition(color));
        }
        
        private List<Vector2> GetThreatPositionsForColor(PieceColor color, Board.Board board)
        {
            var threatPositionsForColor = new List<Vector2>();
            
            foreach (var data in board.ChessPieceData.Where(d => d.Color == color.Opposite()))
            {
                threatPositionsForColor.AddRange(GetThreatMoves(data.Position, board).Select(m => m.To));
            }

            return threatPositionsForColor;
        }

        private bool IsCheckMate(PieceColor color, Board.Board board)
        {
            return IsCheck(color, board) && GetAntiCheckMoves(color, board).IsEmpty();
        }

        private List<Move> GetAntiCheckMoves(PieceColor color, Board.Board board)
        {
            var antiCheckMoves = new List<Move>();

            foreach (var data in board.ChessPieceData.Where(d => d.Color == color))
            {
                var possibleMoves = GetPossibleMoves(data.Position, board);

                foreach (var possibleMove in possibleMoves)
                {
                    var tempBoard = new Board.Board(board);
                    tempBoard.MovePiece(possibleMove);

                    if (!IsCheck(color, tempBoard))
                    {
                        antiCheckMoves.Add(possibleMove);
                    }
                }
            }

            return antiCheckMoves;
        }
    }

    
}
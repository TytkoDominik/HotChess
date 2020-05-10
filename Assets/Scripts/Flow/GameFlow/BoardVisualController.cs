using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DIFramework;
using GameLogic.Board;
using GameLogic.GameRules;
using UnityEngine;
using Zenject;

namespace Flow.GameFlow
{
    public class BoardVisualController : MonoBehaviour
    {
        [SerializeField] private List<ChessPiecePrefab> _chessPiecePrefabs;
        [SerializeField] private Transform _chessPieceParent;
        [SerializeField] private Transform _whiteKilledPosition;
        [SerializeField] private Transform _blackKilledPosition;
        private int killedWhites = 0;
        private int killedBlacks = 0;
        private int deadGridSize = 8;
        private Dictionary<Vector2, GameObject> _boardPositions;
        private Dictionary<Vector2, GameObject> _boardPieces = new Dictionary<Vector2, GameObject>();
        
        [Inject]
        private void Initialize(ChessRulesCore core, SignalBus signalBus)
        {
            _boardPositions = new Dictionary<Vector2, GameObject>();
            
            foreach (var position in GetComponentsInChildren<BoardPosition>())
            {
                _boardPositions.Add(position.GetBoardPosition(), position.gameObject);
            }
            
            signalBus.Subscribe<PromotionSignal>(PromotePawn);
            signalBus.Subscribe<CreateBoardSignal>(CreateBoard);
        }

        private void CreateBoard(CreateBoardSignal signal)
        {
            foreach (var boardPiece in 
                _chessPieceParent.GetComponentsInChildren<Transform>().Where(t => t.parent == _chessPieceParent))
            {
                Destroy(boardPiece.gameObject);
            }
            
            _boardPieces.Clear();
            
            var board = signal.Board;
            
            foreach (var data in board.ChessPieceData)
            {
                CreatePiece(data.Position, data.Type, data.Color);
            }
        }

        private void CreatePiece(Vector2 position, PieceType type, PieceColor color)
        {
            var piecePrefab = _chessPiecePrefabs.FirstOrDefault(p => p.Type ==type && p.Color == color);

            if (piecePrefab == null)
            {
                return;
            }

            var boardPiece = Instantiate(piecePrefab.Prefab, _chessPieceParent);
            boardPiece.transform.position = _boardPositions[position].transform.position;
            _boardPieces.Add(position, boardPiece);
        }

        public void MovePiece(Move move, Action callback)
        {
            var piece = _boardPieces[move.From];
            _boardPieces.Remove(move.From);
            _boardPieces.Add(move.To, piece);
            
            piece.transform.DOMove(_boardPositions[move.To].transform.position, 0.8f).OnComplete(callback.Invoke);
        }

        public void PerformKillingAnimation(Vector2 position, PieceColor color, float delay, Action callback)
        {
            var killedPiece = _boardPieces[position];
            _boardPieces.Remove(position);

            var counter = color == PieceColor.White ? killedWhites : killedBlacks;
            var startPosition = color == PieceColor.White ? _whiteKilledPosition : _blackKilledPosition;

            var translation = color == PieceColor.White ? 1 : -1;

            var endPosition = startPosition.position 
                              + counter % deadGridSize * new Vector3(0, 0, translation)
                                + (counter/deadGridSize) * new Vector3(-translation, 0, 0);

            switch (color)
            {
                case PieceColor.Black:
                    killedBlacks++;
                    break;
                case PieceColor.White:
                    killedWhites++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
            
            killedPiece.transform.DOMove(endPosition, 0.8f).SetDelay(delay).OnComplete(callback.Invoke);
        }

        private void PromotePawn(PromotionSignal signal)
        {
            var pawnModel = _boardPieces[signal.Position];
            _boardPieces.Remove(signal.Position);
            Destroy(pawnModel);
            
            CreatePiece(signal.Position, signal.Type, signal.Color);
        }
    }
    
    [Serializable]
    public class ChessPiecePrefab
    {
        public GameObject Prefab;
        public PieceType Type;
        public PieceColor Color;
    }
}
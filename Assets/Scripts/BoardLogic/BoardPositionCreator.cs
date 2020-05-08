using System;
using UnityEngine;

namespace BoardLogic
{
    [ExecuteInEditMode]
    public class BoardPositionCreator : MonoBehaviour
    {
        [SerializeField] private BoardPosition _boardPositionPrefab;
        [SerializeField] private bool _createBoard;

        private void Update()
        {
            if (_createBoard)
            {
                _createBoard = false;
                CreateBoard();
            }
        }

        public void CreateBoard()
        {
            foreach (var child in GetComponentsInChildren<Transform>())
            {
                if (child.parent == transform)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
            
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    CreateBoardPosition(x, y);
                }
            }
        }

        private void CreateBoardPosition(int x, int y)
        {
            var boardPosition = Instantiate(_boardPositionPrefab, transform);
            boardPosition.transform.localPosition = new Vector3(x, 0, y);
            boardPosition.SetBoardPosition(x, y);
            
            var notationTranslator = new PositionToNotationTranslator();

            boardPosition.name = notationTranslator.GetNotationFromPosition(new Vector2(x, y));
        }
    }
}
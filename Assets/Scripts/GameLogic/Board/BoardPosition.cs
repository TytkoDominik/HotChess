using UnityEngine;

namespace GameLogic.Board
{
    public class BoardPosition : MonoBehaviour
    {
        [SerializeField] private Vector2 _position;

        public Vector2 GetBoardPosition()
        {
            return _position;
        }

        public void SetBoardPosition(int x, int y)
        {
            _position = new Vector2(x, y);
        }
    }
}

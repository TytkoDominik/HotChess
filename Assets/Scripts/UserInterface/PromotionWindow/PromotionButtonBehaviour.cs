using GameLogic.Board;
using UnityEngine;

namespace UserInterface.PromotionWindow
{
    public class PromotionButtonBehaviour : MonoBehaviour
    {
        [SerializeField] private PieceType _pieceType;
        private PromotionWindowBehaviour _promotionWindowBehaviour;
        private void Start()
        {
            _promotionWindowBehaviour = transform.parent.GetComponentInParent<PromotionWindowBehaviour>();
        }

        public void OnClick()
        {
            _promotionWindowBehaviour.ChoosePieceType(_pieceType);
        }
    }
}
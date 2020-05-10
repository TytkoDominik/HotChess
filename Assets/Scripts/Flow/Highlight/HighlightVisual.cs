using System.Linq;
using DIFramework;
using GameLogic.Board;
using UnityEngine;
using Zenject;

namespace Flow.Highlight
{
    public class HighlightVisual : MonoBehaviour
    {
        [Inject] private BoardHighlightMaterials _boardHighlightMaterials;
        private BoardPosition _boardPosition;
        private MeshRenderer _meshRenderer;

        [Inject]
        private void Initialize(SignalBus signalBus)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _boardPosition = GetComponent<BoardPosition>();
            signalBus.Subscribe<HighlightSignal>(UpdateSelection);
        }

        private void UpdateSelection(HighlightSignal signal)
        {
            var selection = signal.SelectionData.FirstOrDefault(i => i.Position == _boardPosition.GetBoardPosition());

            if (selection == null)
            {
                _meshRenderer.enabled = false;
                return;
            }

            _meshRenderer.enabled = true;
            _meshRenderer.material = _boardHighlightMaterials.GetMaterial(selection.Type);
        }
    }
}
using System.Linq;
using DIFramework;
using UnityEngine;
using Zenject;

namespace BoardLogic
{
    public class BoardSelectionVisual : MonoBehaviour
    {
        [Inject] private BoardSelectionMaterials _boardSelectionMaterials;
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
            _meshRenderer.material = _boardSelectionMaterials.GetMaterial(selection.Type);
        }
    }
}
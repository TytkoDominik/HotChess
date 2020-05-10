using System;
using UnityEngine;

namespace Flow.Highlight
{
    public class BoardHighlightMaterials : MonoBehaviour
    {
        [SerializeField] private Material _move;
        [SerializeField] private Material _attack;
        [SerializeField] private Material _select;

        public Material GetMaterial(BoardHighlightType type)
        {
            switch (type)
            {
                case BoardHighlightType.Selected:
                    return _select;
                case BoardHighlightType.Move:
                    return _move;
                case BoardHighlightType.Attack:
                    return _attack;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}


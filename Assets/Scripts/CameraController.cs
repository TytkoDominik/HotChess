using System;
using BoardLogic;
using DG.Tweening;
using DIFramework;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class CameraController : MonoBehaviour
    {
        [Inject] private ActivePlayer _activePlayer;
        [SerializeField] private Transform _white;
        [SerializeField] private Transform _black;
        [SerializeField] private Transform _menu;
        [SerializeField] private float _transitionTime;

        [Inject]
        private void Initialize(SignalBus signalBus)
        {
            signalBus.Subscribe<ChangePlayerSignal>(UpdateCameraPosition);
            signalBus.Subscribe<StartGameSignal>(() => UpdateCameraPosition(_white));
            signalBus.Subscribe<EndGameSignal>(() => UpdateCameraPosition(_menu));
        }

        private void UpdateCameraPosition(ChangePlayerSignal signal)
        {
            switch (_activePlayer.GetActivePlayerColor())
            {
                case PieceColor.Black:
                    UpdateCameraPosition(_black);
                    break;
                case PieceColor.White:
                    UpdateCameraPosition(_white);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateCameraPosition(Transform target)
        {
            transform.DOMove(target.position, _transitionTime);
            transform.DORotate(target.rotation.eulerAngles, _transitionTime);
        }
    }
}
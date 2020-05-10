using DG.Tweening;
using DIFramework;
using GameLogic.Board;
using GameLogic.PlayerData;
using UnityEngine;
using Zenject;

namespace Flow.Camera
{
    public class CameraMover : MonoBehaviour
    {
        [Inject] private ActivePlayer _activePlayer;
        [SerializeField] private Transform _white;
        [SerializeField] private Transform _black;
        [SerializeField] private Transform _menu;
        [SerializeField] private float _transitionTime;
        [SerializeField] private AnimationCurve _middle;
        [SerializeField] private AnimationCurve _finish;
    
        [Inject]
        private void Initialize(SignalBus signalBus)
        {
            signalBus.Subscribe<ChangePlayerSignal>(UpdateCameraPosition);
            signalBus.Subscribe<StartGameSignal>(() => UpdateCameraPosition(_white));
            signalBus.Subscribe<EndGameSignal>(() => UpdateCameraPosition(_menu));
        }

        private void UpdateCameraPosition(ChangePlayerSignal signal)
        {
            UpdateCameraToPlayerPosition(_activePlayer.GetActivePlayerColor());
        }

        private void UpdateCameraPosition(Transform target)
        {
            transform.DOMove(target.position, _transitionTime);
            transform.DORotate(target.rotation.eulerAngles, _transitionTime);
        }

        private void UpdateCameraToPlayerPosition(PieceColor color)
        {
            var target = color == PieceColor.White ? _white : _black;
            var finishAngle = target.rotation.eulerAngles.y;
            var middleAngle = color == PieceColor.White ? 270 : 90;
            
            transform.DOMove(target.position, _transitionTime);
            transform.DORotate(new Vector3(0, middleAngle, 0), _transitionTime/2).SetEase(_middle).OnComplete(() =>
            {
                transform.DORotate(new Vector3(0, finishAngle, 0), _transitionTime/2).SetEase(_finish);
            });

        }
    }
}
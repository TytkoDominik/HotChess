using UnityEngine.PlayerLoop;
using Zenject;

namespace DIFramework
{
    public class SignalsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            
            Container.DeclareSignal<BoardMouseEnterSignal>();
            Container.DeclareSignal<BoardMouseExitSignal>();
            Container.DeclareSignal<BoardMouseDownSignal>();
            Container.DeclareSignal<ChangePlayerSignal>();
            Container.DeclareSignal<PerformMoveSignal>();
            Container.DeclareSignal<HighlightSignal>();
            
            Container.DeclareSignal<PromotionTypeChosenSignal>();
            Container.DeclareSignal<OpenPromotionWindowSignal>();
            Container.DeclareSignal<PromotionSignal>();
            Container.DeclareSignal<CreateBoardSignal>();
            Container.DeclareSignal<StartGameSignal>();
            Container.DeclareSignal<EndGameSignal>();

            Container.DeclareSignal<UpdatePlayerLabelsSignal>();
        }
    }
}
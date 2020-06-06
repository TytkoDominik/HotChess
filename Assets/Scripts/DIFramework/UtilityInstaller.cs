using Flow.GameFlow;
using Flow.Highlight;
using GameDesire.Rest;
using GameLogic.GameRules;
using GameLogic.PlayerData;
using UserInterface;
using Zenject;

namespace DIFramework
{
    public class UtilityInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameState>().ToSelf().FromNew().AsSingle().NonLazy();
            Container.Bind<BoardInteractionStrategyController>().ToSelf().FromNew().AsSingle().NonLazy();
            Container.Bind<HighlightController>().ToSelf().FromNew().AsSingle().NonLazy();
            Container.Bind<ChessFlowController>().ToSelf().FromNew().AsSingle().NonLazy();
            Container.Bind<PromotionBehaviour>().ToSelf().FromNew().AsSingle().NonLazy();
            Container.Bind<ActivePlayer>().ToSelf().FromNew().AsSingle().NonLazy();
            Container.Bind<ChessRulesCore>().ToSelf().FromNew().AsSingle().NonLazy();
        }
    }
}
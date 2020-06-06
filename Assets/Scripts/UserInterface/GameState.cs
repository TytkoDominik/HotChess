using DIFramework;
using GameDesire.Rest;
using GameLogic.Board;
using Zenject;

namespace UserInterface
{
    public class GameState
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private AuthRequestSender _authRequestSender;
        public PlayerData WhitePlayer;
        public PlayerData BlackPlayer;

        [Inject]
        private void Initialize()
        {
            _signalBus.Subscribe<EndGameSignal>(UpdateStats);
            _signalBus.Subscribe<StartGameSignal>(CreateCurrentGameStats);
        }

        private void CreateCurrentGameStats()
        {
            WhitePlayer.CurrentGameStats = new CurrentGameStats{Color = PieceColor.White, MovesPerformed = 0, Won = false};
            BlackPlayer.CurrentGameStats = new CurrentGameStats{Color = PieceColor.Black, MovesPerformed = 0, Won = false};
        }

        public GameState()
        {
            WhitePlayer = new PlayerData();
            BlackPlayer = new PlayerData();
        }

        public void Switch()
        {
            var temp = WhitePlayer;
            WhitePlayer = BlackPlayer;
            BlackPlayer = temp;
            _signalBus.Fire<UpdatePlayerLabelsSignal>();
        }

        public void UpdateStats()
        {
            WhitePlayer.UpdateStats(_authRequestSender, true);
            BlackPlayer.UpdateStats(_authRequestSender, true);
            _signalBus.Fire<UpdatePlayerLabelsSignal>();
        }

        public PlayerData Player(PieceColor color)
        {
            return color == PieceColor.White ? WhitePlayer : BlackPlayer;
        }
    }
}
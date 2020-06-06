using System;
using DIFramework;
using Extensions;
using GameDesire.Rest;
using GameLogic.Board;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UserInterface
{
    public class PlayerLoginController : MonoBehaviour
    {
        [Inject] private GameState _gameState;
        [Inject] private SignalBus _signalBus;
        [Inject] private AuthRequestSender _authRequestSender;
        [SerializeField] private PieceColor _color;
        [SerializeField] private Text _loginText;
        [SerializeField] private InputField _login;
        [SerializeField] private InputField _password;
        [SerializeField] private InputField _repeatPassword;
        [SerializeField] private Text _gamesWon;
        [SerializeField] private Text _gamesPlayed;
        [SerializeField] private Text _movesPerformed;
        [SerializeField] private Text _gamesPlayedAsWhite;
        [SerializeField] private Text _gamesPlayedAsBlack;
        [SerializeField] private GameObject _guest;
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _menuHud;
        [SerializeField] private GameObject _gameHud;
        [SerializeField] private GameObject _error;
        [SerializeField] private Text _errorText;
        
        [Inject]
        private void Initialize()
        {
            _signalBus.Subscribe<UpdatePlayerLabelsSignal>(UpdatePlayerLabels);
            _signalBus.Subscribe<EndGameSignal>(ShowHUD);
            _signalBus.Subscribe<StartGameSignal>(HideHUD);
        }

        private void HideHUD()
        {
            _menuHud.SetActive(false);
            _gameHud.SetActive(true);
        }

        private void ShowHUD()
        {
            _menuHud.SetActive(true);
            _gameHud.SetActive(false);
        }
        
        private void UpdatePlayerLabels()
        {
            var data = _color == PieceColor.White ? _gameState.WhitePlayer : _gameState.BlackPlayer;
            _loginText.text = data.Login;
            var stats = _gameState.Player(_color).PlayerStats;

            _guest.SetActive(data.ID == -1);
            _player.SetActive(data.ID != -1);

            if (stats.GamesPlayed == 0)
            {
                _gamesPlayed.text = "0";
                _gamesWon.text  = String.Format("{0} ({1:0.00}%)", 0, 0);
                _gamesPlayedAsWhite.text  = String.Format("White: {0} ({1:0.00}%)", 0, 0);
                _gamesPlayedAsBlack.text  = String.Format("Black: {0} ({1:0.00}%)", 0, 0);
                _movesPerformed.text = "0";
            }
            else
            {
                _gamesPlayed.text = stats.GamesPlayed.ToString();
                _gamesWon.text = String.Format("{0} ({1:0.00}%)", stats.GamesWon, (float)stats.GamesWon/stats.GamesPlayed * 100);
                _gamesPlayedAsWhite.text = String.Format("White: {0} ({1:0.00}%)", stats.PlayedAsWhite, (float)stats.PlayedAsWhite/stats.GamesPlayed * 100);
                _gamesPlayedAsBlack.text  = String.Format("Black: {0} ({1:0.00}%)", stats.GamesPlayed - stats.PlayedAsWhite,
                    (float)(stats.GamesPlayed - stats.PlayedAsWhite)/stats.GamesPlayed * 100);
                _movesPerformed.text = stats.MovesPerformed.ToString();
            }
        }

        public void LoginPlayer()
        {
            _authRequestSender.SendLoginRequest(_login.text, _password.text, ProcessLoginMessage);
        }

        public void AddPlayer()
        {
            if (_password.text == _repeatPassword.text)
            {
                _authRequestSender.SendAddPlayerRequest(_login.text, _password.text, ProcessAddPlayerMessage);
            }
        }

        private void ProcessLoginMessage(string message)
        {
            int id;
            
            if (Int32.TryParse(message, out id))
            {
                LoginSucceeded(id);
            }
            else
            {
                ShowError(message);
                LoginFailed();
            }
        }

        private void LoginSucceeded(int id)
        {
            var player = new PlayerData(_login.text, id);
            player.GetStats(_authRequestSender);
                
            if (_color == PieceColor.White)
            {
                _gameState.WhitePlayer = player;
            }
            else
            {
                _gameState.BlackPlayer = player;
            }
                
            _signalBus.Fire<UpdatePlayerLabelsSignal>();
            _player.SetActive(true);
            _guest.SetActive(false);
        }
        
        private void LoginFailed()
        {
            Debug.LogError("Login failed");
        }

        private void ProcessAddPlayerMessage(string message)
        {
            int id;
            
            if (Int32.TryParse(message, out id))
            {
                AddPlayerSucceeded(id);
            }
            else
            {
                ShowError(message);
                AddPlayerFailed();
            }
        }

        private void AddPlayerSucceeded(int id)
        {
            var player = _color == PieceColor.White ? _gameState.WhitePlayer : _gameState.BlackPlayer;
            player = new PlayerData(_login.text, id);
            player.PlayerStats = new PlayerStats();
            player.UpdateStats(_authRequestSender, false);
            LoginPlayer();
        }
        
        private void AddPlayerFailed()
        {
            Debug.LogError("Adding player failed");
        }

        public void Logout()
        {
            if (_color == PieceColor.White)
            {
                _gameState.WhitePlayer = new PlayerData();
            }
            else
            {
                _gameState.BlackPlayer = new PlayerData();
            }
            
            _player.SetActive(false);
            _guest.SetActive(true);
            UpdatePlayerLabels();
        }

        public void Concede()
        {
            _signalBus.Fire(new EndGameSignal(_color.Opposite()));
        }

        private void ShowError(string ex)
        {
            _error.SetActive(true);
            _errorText.text = ex;
        }
    }
}
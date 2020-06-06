using System;
using GameDesire.Rest;
using GameLogic.Board;
using Newtonsoft.Json;
using UnityEngine;

namespace UserInterface
{
    public class PlayerData
    {
        public string Login;
        public int ID;
        public PlayerStats PlayerStats;
        public CurrentGameStats CurrentGameStats;

        public PlayerData()
        {
            Login = "Guest";
            ID = -1;
            PlayerStats = new PlayerStats();
            CurrentGameStats = new CurrentGameStats();
        }

        public PlayerData(string login, int id)
        {
            Login = login;
            ID = id;
            PlayerStats = new PlayerStats();
            CurrentGameStats = new CurrentGameStats();
        }

        public void UpdateStats(AuthRequestSender authRequestSender, bool afterGameUpdate)
        {
            if (ID == -1)
            {
                return;
            }

            if (afterGameUpdate)
            {
                PlayerStats.GamesPlayed += 1;
            }
            
            if (CurrentGameStats.Won)
            {
                PlayerStats.GamesWon++;
            }

            if (CurrentGameStats.Color == PieceColor.White)
            {
                PlayerStats.PlayedAsWhite++;
            }

            PlayerStats.MovesPerformed += CurrentGameStats.MovesPerformed;
            
            authRequestSender.SendUpdateStatsRequest(ID, JsonConvert.SerializeObject(PlayerStats));
        }

        public void GetStats(AuthRequestSender authRequestSender)
        {
            authRequestSender.SendStatsRequest(ID, SetStats);
        }

        private void SetStats(string stats)
        {
            Debug.Log(stats);
            PlayerStats = JsonConvert.DeserializeObject<PlayerStats>(stats.Remove(0, 10));
        }
    }

    [Serializable]
    public class PlayerStats
    {
        public int GamesPlayed;
        public int GamesWon;
        public int PlayedAsWhite;
        public int MovesPerformed;
    }

    public class CurrentGameStats
    {
        public int MovesPerformed;
        public PieceColor Color;
        public bool Won;
    }
}
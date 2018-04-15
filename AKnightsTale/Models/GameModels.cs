using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AKnightsTale.Models
{

    //Models added to ApplicationDbContext in IdentityModel.cs

    public class ScoreModel
    {
        [Key]
        public int ID { get; set; }
        public string Username { get; set; }
        public int Score { get; set; }
    }

    public class GameStateModel
    {
        [Key]
        public int ID { get; set; }
        public string Username { get; set; }
        public int Lives { get; set; }
        public int Checkpoint { get; set; }
        public int GemCount { get; set; }
        public int CoinCount { get; set; }
        public float Time { get; set; }
        public string DateTime { get; set; }
        public int Heart1 { get; set; }
        public int Heart2 { get; set; }
        public int Heart3 { get; set; }
        public int Coin1 { get; set; }
        public int Coin2 { get; set; }
        public int Coin3 { get; set; }
        public int Coin4 { get; set; }
        public int Coin5 { get; set; }
        public int Coin6 { get; set; }
        public int Coin7 { get; set; }
        public int Coin8 { get; set; }
        public int Coin9 { get; set; }
        public int Coin10 { get; set; }
        public int Coin11 { get; set; }
        public int Coin12 { get; set; }
        public int Gem1 { get; set; }
        public int Gem2 { get; set; }
        public int Gem3 { get; set; }
    }

    public class SendScoreModel
    {
        public string Username { get; set; }
        public bool Valid { get; set; }
        public int Score { get; set; }
    }

    public class UserModel
    {
        public int Email { get; set; }
        public string Password { get; set; }
    }

    public class DeleteGameStateForm
    {
        public int ID;
    }

    public class AnalysisViewModel
    {
        public int Checkpoint0Count { get; set; }
        public int Checkpoint1Count { get; set; }
        public int Checkpoint2Count { get; set; }
        public int Checkpoint3Count { get; set; }
        public int Checkpoint4Count { get; set; }
        public int Downloads { get; set; }
    }
}
// -----------------------------------------------------------------------------
//  <copyright file="GameOverEventArgs.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Data
{
    using System;

    public class GameOverEventArgs : EventArgs
    {
        public GameOverEventArgs(string winnerName, string winCondition, int team0Score, int team1Score)
        {
            WinnerName = winnerName;
            WinCondition = winCondition;
            Team0Score = team0Score;
            Team1Score = team1Score;
        }

        public string WinnerName { get; private set; }
        
        public string WinCondition { get; private set; }
        
        public int Team0Score { get; private set; }

        public int Team1Score { get; private set; }
    }
}

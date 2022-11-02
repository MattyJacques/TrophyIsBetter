﻿using System;
using TrophyIsBetter.Models;

namespace TrophyIsBetter.Interfaces
{
  public interface IGameModel
  {
    /// <summary>
    /// Unlock a trophy in the game
    /// </summary>
    void UnlockTrophy(Trophy trophy, DateTime timestamp);

    /// <summary>
    /// Save the game data to file
    /// </summary>
    void Save();

  } // IGameModel
} // TrophyIsBetter.Interfaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TrophyIsBetter.Interfaces;
using TrophyParser.PS4;

namespace TrophyIsBetter.Models
{
  internal class PS4GameList : IGameListModel
  {
    #region Private Members

    private PS4TrophyDB _trophyDB;
    private List<IGameModel> _games = new List<IGameModel>();

    #endregion Private Members
    #region Constructors

    public PS4GameList(string path)
    {
      _trophyDB = new PS4TrophyDB(path);
    } // Constructor

    #endregion Constructors
    #region Public Methods

    public void ImportGames(string pathToGames) => throw new NotSupportedException();

    public bool ImportGame(PS4TrophyList game) => _trophyDB.ImportGame(game);

    public bool RemoveGame(IGameModel game) => _trophyDB.RemoveGame(game.NpCommID ?? game.Name);

    /// <summary>
    /// Load all of the games in the application data directory
    /// </summary>
    public List<IGameModel> LoadGames()
    {
      var result = _trophyDB.GetGames();

      _games.Clear();

      foreach (var game in result)
      {
        PS4Game ps4Game = new PS4Game(game);
        _games.Add(ps4Game);

        Debug.WriteLine($"Loaded {game.Name}({game.NpCommID})");
      }

      return _games;
    } // LoadGames

    #endregion Public Methods
  } // PS4GameList
} // TrophyIsBetter.Models

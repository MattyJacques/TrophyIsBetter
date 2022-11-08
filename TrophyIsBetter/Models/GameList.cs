using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TrophyIsBetter.Interfaces;

namespace TrophyIsBetter.Models
{
  internal class GameList : IGameListModel
  {
    #region Private Const Members

#if DEBUG
    private static readonly string APP_DATA_DIR_NAME = "TrophyIsBetter - Debug";
#else
    private static readonly string APP_DATA_DIR_NAME = "TrophyIsBetter";
#endif


    private static readonly Regex TROPHY_FOLDER_REGEX = new Regex("NPWR[\\d]{5}_00$");
    private static readonly string APP_DATA_PATH =
      Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                   APP_DATA_DIR_NAME);
    private static readonly string TROPHY_DATA_PATH = Path.Combine(APP_DATA_PATH, "Trophies");
    private static readonly string BACKUP_DATA_PATH = Path.Combine(APP_DATA_PATH, "Backup");

    #endregion
    #region Private Members

    private string _gameDirectory;
    private string _tempGameDirectory;

    private List<Game> _games = new List<Game>();

    #endregion Private Members
    #region Public Methods

    /// <summary>
    /// Import trophy folders from the given directory
    /// </summary>
    public void ImportGames(string pathToGames)
    {
      foreach (string dir in GetDirectories(pathToGames))
      {
        string path = CopyWithBackup(dir);
        Utility.PfdTool.DecryptTrophyData(path);

        _games.Add(new Game(path));
      }
    } // ImportGames

    /// <summary>
    /// Load all of the games in the application data directory
    /// </summary>
    public List<Game> LoadGames()
    {
      if (Directory.Exists(TROPHY_DATA_PATH))
      {
        var trophyFolders = from dir
                            in Directory.EnumerateDirectories(TROPHY_DATA_PATH, "*",
                               SearchOption.AllDirectories)
                            where TROPHY_FOLDER_REGEX.IsMatch(dir)
                            select dir;

        _games.Clear();

        foreach (string folder in trophyFolders)
        {
          _games.Add(new Game(folder));
        }
      }

      return _games;
    } // LoadGames

    /// <summary>
    /// Close the files that have been opened
    /// </summary>
    public void CloseFiles()
    {
      if (!string.IsNullOrEmpty(_tempGameDirectory))
      {
        foreach (Game list in _games)
        {
          Utility.PfdTool.EncryptTrophyData(list.Path);
        }

        Utility.File.CopyDirectory(_tempGameDirectory, _gameDirectory, true);
        Utility.File.DeleteDirectory(_tempGameDirectory);

        _gameDirectory = null;
        _tempGameDirectory = null;
      }
    } // CloseFiles

    #endregion Public Methods
    #region Private Methods

    /// <summary>
    /// Get the platform of the trophy folder
    /// </summary>
    public static string GetPlatform(string directory)
    {
      if (File.Exists(Path.Combine(directory, "TROPCONF.SFM")))
      {
        return "PS3";
      }
      else
      {
        return "Vita";
      }
    } // GetPlatform

    /// <summary>
    /// Get a list of trophy directories from the given path
    /// </summary>
    public static IEnumerable<string> GetDirectories(string rootPath)
    {
      var result = from dir
                   in Directory.EnumerateDirectories(rootPath, "*",
                      SearchOption.AllDirectories)
                   where TROPHY_FOLDER_REGEX.IsMatch(dir)
                   select dir;

      if (!result.Any() && TROPHY_FOLDER_REGEX.IsMatch(rootPath))
      {
        result = new List<string>()
        {
          rootPath
        };
      }

      return result;
    } // GetDirectories

    /// <summary>
    /// Copy the directory the application data directory with a backup
    /// </summary>
    public static string CopyWithBackup(string directory)
    {
      string destination = Path.Combine(GetPlatform(directory), Path.GetFileName(directory));
      string fullDataPath = Path.Combine(TROPHY_DATA_PATH, destination);
      string fullBackupPath = Path.Combine(BACKUP_DATA_PATH, destination);

      Debug.WriteLine($"Copying trophy folder to {fullDataPath}");

      Utility.File.CopyDirectory(directory, fullDataPath, false);
      Utility.File.CopyDirectory(directory, fullBackupPath, false);

      return fullDataPath;
    } // CopyWithBackup

    #endregion Private Methods
  } // GameList
} // TrophyIsBetter.Models

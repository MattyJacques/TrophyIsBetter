using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using TrophyParser.Models;
using TrophyParser.Vita;

namespace TrophyParser.PS4
{
  public class PS4TrophyDB
  {
    #region Private Members

    private readonly SqliteConnection _connection;
    private readonly string _path;

    #endregion Private Members
    #region Constructors

    public PS4TrophyDB(string path)
    {
      SQLitePCL.Batteries.Init();

      _path = path;
      _connection = new SqliteConnection(new SqliteConnectionStringBuilder()
      {
        DataSource = _path
      }.ConnectionString);
    } // Constructor

    #endregion Constructors
    #region Public Methods

    public List<PS4TrophyList> GetGames()
    {
      List<PS4TrophyList> games = new List<PS4TrophyList>();

      SqliteDataReader reader = ExecuteQuery("SELECT * FROM tbl_trophy_title");
      while (reader.Read())
      {
        PS4TrophyList list = new PS4TrophyList(_path,
                                               reader.GetString(3),
                                               reader.GetString(42),
                                               reader.GetInt32(32) == 1,
                                               false,
                                               reader.GetInt32(31),
                                               reader.GetInt32(14),
                                               reader.GetDateTime(22));
        games.Add(list);
      }

      return games;
    } // GetGames

    public List<Trophy> GetTrophies(string npCommID)
    {
      List<Trophy> trophies = new List<Trophy>();

      SqliteDataReader reader =
        ExecuteQuery("SELECT * FROM tbl_trophy_flag WHERE trophy_title_id = '" + npCommID + "'");
      while (reader.Read())
      {
        Trophy trophy = new Trophy(
          reader.GetInt32(4),
          reader.GetInt32(12) == 1 ? "yes" : "no",
          GetTrophyType(reader.GetInt32(11)),
          -1,
          reader.GetString(13),
          reader.GetString(14),
          reader.GetInt32(5) + 1
        );

        DateTime timestamp = reader.GetDateTime(9);
        if (timestamp != DateTime.MinValue)
        {
          trophy.Timestamp = new Timestamp
          {
            Time = timestamp,
            IsSynced = false
          };
        }

        trophies.Add(trophy);
      }

      return trophies;
    } // GetTrophies

    public void ChangeTimestamp(string npCommID,
                                int id,
                                DateTime timeUnlocked,
                                DateTime timeLastUnlocked,
                                DateTime timeLastUpdate)
    {
      string timeuc = timeUnlocked.ToString("yyyy-MM-ddTHH:mm:ss.ff");
      ExecuteNonQuery($@"UPDATE tbl_trophy_flag
        SET time_unlocked = '{timeUnlocked:yyyy-MM-ddTHH:mm:ss}.00Z',
            time_unlocked_uc = '{timeUnlocked:yyyy-MM-ddTHH:mm:ss.ff}Z'
        WHERE trophy_title_id = '{npCommID}' AND trophyid = {id}");

      ExecuteNonQuery($@"UPDATE tbl_trophy_title
        SET time_last_unlocked = '{timeLastUnlocked:yyyy-MM-ddTHH:mm:ss}.00Z',
            time_last_update = '{timeLastUpdate:yyyy-MM-ddTHH:mm:ss}.00Z',
            time_last_update_uc = '{timeLastUpdate:yyyy-MM-ddTHH:mm:ss.ff}Z'
        WHERE trophy_title_id = '{npCommID}'");

      ExecuteNonQuery($@"UPDATE tbl_trophy_title_entry
        SET time_last_update = '{timeLastUpdate:yyyy-MM-ddTHH:mm:ss}.00Z',
            time_last_update_uc = '{timeLastUpdate:yyyy-MM-ddTHH:mm:ss.ff}Z'
        WHERE trophy_title_id = '{npCommID}'");
    } // ChangeTimestamp

    #endregion Public Methods
    #region Private Methods
    #region Data Parsing

    private char GetTrophyType(int grade)
    {
      char result;

      switch (grade)
      {
        case 1:
          result = 'P';
          break;
        case 2:
          result = 'G';
          break;
        case 3:
          result = 'S';
          break;
        case 4:
          result = 'B';
          break;
        default:
          result = 'Z';
          break;
      }

      return result;
    } // GetTrophyType

    #endregion Data Parsing
    #region Database

    /// <summary>
    /// Execute a query command
    /// </summary>
    /// <param name="query">Text of the query to execute</param>
    /// <returns>SqliteDataReader containing the query results</returns>
    private SqliteDataReader ExecuteQuery(string query)
    {
      SqliteDataReader reader = null;

      if (Open())
      {
        SqliteCommand command = _connection.CreateCommand();
        command.CommandText = query;
        reader = command.ExecuteReader();
      }

      return reader;
    } // ExecuteQuery

    /// <summary>
    /// Execute a non-query command
    /// </summary>
    /// <param name="commandText">Text of the command to execute</param>
    /// <param name="shouldClose">Should the connection be closed afterwards</param>
    /// <returns>Number of rows affected</returns>
    private bool ExecuteNonQuery(string commandText, bool shouldClose = true)
    {
      int rowsAffected = 0;

      if (Open())
      {
        SqliteCommand command = _connection.CreateCommand();
        command.CommandText = commandText;
        rowsAffected = command.ExecuteNonQuery();

        if (shouldClose)
        {
          Close();
        }
      }

      return rowsAffected > 0;
    } // ExecuteNonQuery

    /// <summary>
    /// Check if the connection is open, if it isn't open it
    /// </summary>
    /// <returns>Whether the connection is/has been opened</returns>
    private bool Open()
    {
      if (!IsOpen())
      {
        _connection.Open();
      }

      return IsOpen();
    } // Open

    private bool Close()
    {
      if (IsOpen())
      {
        _connection.Close();
      }

      return !IsOpen();
    } // Close

    /// <summary>
    /// Return whether the database connection is open or not
    /// </summary>
    /// <returns>True if database connection is open</returns>
    private bool IsOpen()
    {
      return _connection.State == System.Data.ConnectionState.Open;
    } // IsOpen

    #endregion Database
    #endregion Private Methods
  } // PS4TrophyDB
} // TrophyParser.PS3

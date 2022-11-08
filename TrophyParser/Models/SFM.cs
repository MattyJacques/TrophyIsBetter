using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TrophyParser.Models;

namespace TrophyParser
{
  public class SFM
  {
    #region Private Members

    protected string _confVersion;
    protected string _npCommID;
    protected string _trophySetVersion;
    protected string _parentalLevel;
    protected string _titleName;
    protected string _titleDetail;
    protected List<Trophy> _trophies = new List<Trophy>();

    #endregion Private Members
    #region Public Properties

    public string TitleName => _titleName;
    public string NpCommID => _npCommID;
    public bool HasPlatinum => _trophies[0].Rank.Equals("P");
    public int TrophyCount => _trophies.Count;
    public Trophy this[int index] => _trophies[index];

    #endregion Public Properties
    #region Public Methods

    /// <summary>
    /// Print data in the console for debugging
    /// </summary>
    public void PrintState()
    {
      Console.WriteLine("----- TROPCONF Data -----\n");

      Console.WriteLine("Version: {0}", _confVersion);
      Console.WriteLine("npCommunicationID: {0}", _npCommID);
      Console.WriteLine("Trophy Set Version: {0}", _trophySetVersion);
      Console.WriteLine("Parental Level: {0}", _parentalLevel);
      Console.WriteLine("Title Name: {0}", _titleName);
      Console.WriteLine("Title Detail: {0}", _titleDetail);

      Console.WriteLine("\nTrophies");
      foreach (Trophy t in _trophies)
      {
        Console.WriteLine(t);
      }
    } // PrintState

    #endregion Public Methods
    #region Protected Methods

    /// <summary>
    /// Parse the SFM files data
    /// </summary>
    protected void Parse(byte[] data)
    {
      XmlDocument xmlDoc = new XmlDocument();
      xmlDoc.LoadXml(Encoding.UTF8.GetString(data).Trim('\0'));

      _confVersion = xmlDoc?.DocumentElement?.Attributes["version"]?.Value;
      _npCommID = xmlDoc?.GetElementsByTagName("npcommid")[0]?.InnerText;
      _trophySetVersion = xmlDoc?.GetElementsByTagName("trophyset-version")[0]?.InnerText;
      _parentalLevel = xmlDoc?.GetElementsByTagName("parental-level")[0]?.InnerText;
      _titleName = xmlDoc?.GetElementsByTagName("title-name")[0]?.InnerText;
      _titleDetail = xmlDoc?.GetElementsByTagName("title-detail")[0]?.InnerText;

      ParseTrophies(xmlDoc);
    } // Parse

    #endregion Protected Methods
    #region Private Methods

    /// <summary>
    /// Parse the trophy data in the SFM file
    /// </summary>
    private void ParseTrophies(XmlDocument xmlDoc)
    {
      XmlNodeList trophiesXML = xmlDoc?.GetElementsByTagName("trophy");
      List<Trophy> trophyList = new List<Trophy>();
      if (trophiesXML != null)
      {
        foreach (XmlNode trophy in trophiesXML)
        {
          _ = int.TryParse(trophy?.Attributes?["id"]?.Value, out int id);
          _ = int.TryParse(trophy?.Attributes?["pid"]?.Value, out int pid);
          _ = int.TryParse(trophy?.Attributes?["gid"]?.Value, out int gid);

          Trophy item = new Trophy(
            id,
            trophy?.Attributes?["hidden"]?.Value,
            trophy?.Attributes?["ttype"]?.Value,
            pid,
            trophy?["name"]?.InnerText,
            trophy?["detail"]?.InnerText,
            gid
            );

          _trophies.Add(item);
        }
      }
    } // ParseTrophies

    #endregion Private Methods
  } // SFM
} // TrophyParser

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using static TrophyParser.Structs;

namespace TrophyParser
{
  // Class to extract general trophy data about the title
  public class TROPCONF
  {
    #region Const Members

    private const string TROPCONF_FILE_NAME = "TROPCONF.SFM";
    private const int START_BYTE = 0x40;

    #endregion
    #region Public Members

    public readonly string Name;
    public readonly string NpCommId;

    public bool HasPlatinum { get { return _trophies[0].Rank.Equals("P"); } }
    public int TrophyCount { get { return _trophies.Count; } }

    #endregion
    #region Private Members

    private readonly string _version;
    private readonly string _trophySetVersion;
    private readonly string _parentalLevel;
    private readonly string _titleDetail;
    private readonly List<Trophy> _trophies;

    #endregion
    #region Public Constructors

    public TROPCONF(string path)
    {
      byte[] data = File.ReadAllBytes(Utility.File.GetFullPath(path, TROPCONF_FILE_NAME));
      data = data.SubArray(START_BYTE, data.Length - START_BYTE);

      XmlDocument xmlDoc = new XmlDocument();
      xmlDoc.LoadXml(Encoding.UTF8.GetString(data).Trim('\0'));

      _version = xmlDoc?.DocumentElement?.Attributes["version"]?.Value;
      NpCommId = xmlDoc?.GetElementsByTagName("npcommid")[0]?.InnerText;
      _trophySetVersion = xmlDoc?.GetElementsByTagName("trophyset-version")[0]?.InnerText;
      _parentalLevel = xmlDoc?.GetElementsByTagName("parental-level")[0]?.InnerText;
      Name = xmlDoc?.GetElementsByTagName("title-name")[0]?.InnerText;
      _titleDetail = xmlDoc?.GetElementsByTagName("title-detail")[0]?.InnerText;

      _trophies = ParseTrophies(xmlDoc);
    } // Constructor

    #endregion
    #region Public Methods

    public void PrintState()
    {
      Console.WriteLine("----- TROPCONF Data -----\n");

      Console.WriteLine("Version: {0}", _version);
      Console.WriteLine("npCommunicationID: {0}", NpCommId);
      Console.WriteLine("Trophy Set Version: {0}", _trophySetVersion);
      Console.WriteLine("Parental Level: {0}", _parentalLevel);
      Console.WriteLine("Title Name: {0}", Name);
      Console.WriteLine("Title Detail: {0}", _titleDetail);

      Console.WriteLine("\nTrophies");
      foreach (Trophy t in _trophies)
      {
        Console.WriteLine(t);
      }
    } // PrintState

    #endregion
    #region Private Methods

    private static List<Trophy> ParseTrophies(XmlDocument xmlDoc)
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

          trophyList.Add(item);
        }
      }

      return trophyList;
    } // ParseTrophies

    #endregion
  } // TROPCONF
} // TrophyParser

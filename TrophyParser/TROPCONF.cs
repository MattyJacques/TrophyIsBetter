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

    public List<Trophy> Trophies;

    #endregion
    #region Private Members

    private readonly string? _version;
    private readonly string? _npcommid;
    private readonly string? _trophyset_version;
    private readonly string? _parental_level;
    private readonly string? _title_name;
    private readonly string? _title_detail;

    #endregion
    #region Public Constructors

    public TROPCONF(string path)
    {
      byte[] fileData = File.ReadAllBytes(GetFullPath(path));
      ArraySegment<byte> data = new(fileData, START_BYTE, fileData.Length - START_BYTE);

      XmlDocument xmlDoc = new();
      xmlDoc.LoadXml(Encoding.UTF8.GetString(data).Trim('\0'));

      _version = xmlDoc?.DocumentElement?.Attributes["version"]?.Value;
      _npcommid = xmlDoc?.GetElementsByTagName("npcommid")[0]?.InnerText;
      _trophyset_version = xmlDoc?.GetElementsByTagName("trophyset-version")[0]?.InnerText;
      _parental_level = xmlDoc?.GetElementsByTagName("parental-level")[0]?.InnerText;
      _title_name = xmlDoc?.GetElementsByTagName("title-name")[0]?.InnerText;
      _title_detail = xmlDoc?.GetElementsByTagName("title-detail")[0]?.InnerText;

      Trophies = ParseTrophies(xmlDoc);
    } // Constructor

    #endregion
    #region Public Methods

    public void PrintState()
    {
      Console.WriteLine("----- TROPCONF Data -----\n");

      Console.WriteLine("Version: {0}", _version);
      Console.WriteLine("npCommunicationID: {0}", _npcommid);
      Console.WriteLine("Trophy Set Version: {0}", _trophyset_version);
      Console.WriteLine("Parental Level: {0}", _parental_level);
      Console.WriteLine("Title Name: {0}", _title_name);
      Console.WriteLine("Title Detail: {0}", _title_detail);

      Console.WriteLine("\nTrophies");
      foreach (Trophy t in Trophies)
      {
        Console.WriteLine(t);
      }
    } // PrintState

    #endregion
    #region Private Methods

    private static string GetFullPath(string directory)
    {
      if (directory == null || directory.Trim() == string.Empty)
        throw new Exception("Path cannot be null!");

      string filePath = Path.Combine(directory, TROPCONF_FILE_NAME);
      if (!File.Exists(filePath))
        throw new FileNotFoundException("File not found", TROPCONF_FILE_NAME);

      return filePath;
    } // GetFullPath

    private static List<Trophy> ParseTrophies(XmlDocument? xmlDoc)
    {
      XmlNodeList? trophiesXML = xmlDoc?.GetElementsByTagName("trophy");
      List<Trophy> trophyList = new();
      if (trophiesXML != null)
      {
        foreach (XmlNode trophy in trophiesXML)
        {
          _ = int.TryParse(trophy?.Attributes?["id"]?.Value, out int id);
          _ = int.TryParse(trophy?.Attributes?["pid"]?.Value, out int pid);
          _ = int.TryParse(trophy?.Attributes?["gid"]?.Value, out int gid);

          Trophy item = new(
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

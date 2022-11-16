using System.IO;

namespace TrophyParser.PS3
{
  // Class to extract general trophy data about the title
  internal class TROPCONF : SFM
  {
    #region Const Members

    private const string TROPCONF_FILE_NAME = "TROPCONF.SFM";
    private const int START_BYTE = 0x40;

    #endregion Const Members
    #region Constructors

    internal TROPCONF(string path)
    {
      byte[] data = File.ReadAllBytes(Utility.File.GetFullPath(path, TROPCONF_FILE_NAME));
      Parse(data.SubArray(START_BYTE, data.Length - START_BYTE));
    } // Constructor

    #endregion Constructors
  } // TROPCONF
} // TrophyParser

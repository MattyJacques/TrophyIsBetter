using System;
using System.IO;

namespace TrophyParser.Vita
{
  public class TROP : SFM
  {
    #region Const Members

    private const string TROP_FILE_NAME = "TROP.SFM";

    #endregion Const Members
    #region Constructors

    public TROP(string path)
    {
      FileStream reader = null;
      if (!File.Exists(Path.Combine(path, "TROP.SFM")))
        throw new Exception($"Cannot find {path}/TROP.SFM.");

      try
      {
        byte[] data = File.ReadAllBytes(Utility.File.GetFullPath(path, TROP_FILE_NAME));
        Parse(data);
      }
      catch (IOException)
      {
        throw new Exception("Cannot Open TROPCONF.SFM.");
      }
    } // Constructor

    #endregion Constructors
  } // TROP
} // TrophyParser.Vita

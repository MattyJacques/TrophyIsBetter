using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
  public class PfdTool
  {
    #region Public Static Members

    public static void DecryptTrophyData(string directory)
    {
      RunTool(" -d \"" + directory + "\" TROPTRNS.DAT");
    } // DecryptTrophies

    public static void EncryptTrophyData(string directory, string profile)
    {
      // resign param.sfo
      if (profile != "Default Profile")
      {
        profile = "profiles\\" + profile;
        var br = new BinaryReader(new FileStream(profile, FileMode.Open));
        br.BaseStream.Position = 0xC;
        br.BaseStream.Position = br.ReadInt32();
        var profileId = br.ReadBytes(0x10);
        br.Close();
        var bw = new BinaryWriter(new FileStream(directory + "\\PARAM.SFO", FileMode.Open));
        bw.BaseStream.Position = 0x274;
        bw.Write(profileId);
        bw.Close();
      }

      RunTool(" -u \"" + directory + "\""); // Update PFD
      RunTool(" -e \"" + directory + "\" TROPTRNS.DAT"); // Encrypt trophy data
    } // EncryptTrophies

    #endregion
    #region Private Members

    private static void RunTool(string command)
    {
      System.Diagnostics.Process proc = GetProcess(command);
      proc.Start();
      proc.WaitForExit();
    } // RunTool

    private static System.Diagnostics.Process GetProcess(string command)
    {
      System.Diagnostics.ProcessStartInfo startInfo = GetProcessStartInfo(command);
      return new System.Diagnostics.Process
      {
        StartInfo = startInfo
      };
    } // GetProcess

    private static System.Diagnostics.ProcessStartInfo GetProcessStartInfo(string command)
    {
      return new System.Diagnostics.ProcessStartInfo("pfdtool\\pfdtool.exe", command)
      {
        WorkingDirectory = "pfdtool",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
      };
    } // GetProcessStartInfo

    #endregion
  }
}

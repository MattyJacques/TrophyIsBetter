namespace TrophyParser
{
  internal class Program
  {
    static void Main(string[] args)
    {
      string tempPath = Utility.File.CopyDirToTemp("C:\\Users\\Matty\\Desktop\\trophy\\PS3\\NPWR00071_00");
      //string tempPath = Utility.File.CopyDirToTemp("C:\\Users\\Matty\\Desktop\\NPWR03907_00");
      Utility.PfdTool.DecryptTrophyData(tempPath);
      TROPCONF conf = new TROPCONF(tempPath);
      TROPUSR usr = new TROPUSR(tempPath);
      TROPTRNS trns = new TROPTRNS(tempPath);

      conf.PrintState();
      usr.PrintState();
      trns.PrintState();
      Utility.File.DeleteDirectory(tempPath);

    }
  }
}

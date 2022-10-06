using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrophyParser
{
  internal class Program
  {
    static void Main(string[] args)
    {
      string tempPath = Utility.File.CopyTrophyDirToTemp("C:\\Users\\Matty\\Desktop\\NPWR03907_00");
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

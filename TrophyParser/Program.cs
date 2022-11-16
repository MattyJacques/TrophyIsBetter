using TrophyParser.PS3;
using TrophyParser.Vita;

namespace TrophyParser
{
    internal class Program
    {
        static void PrintPS3()
        {
            string tempPath = Utility.File.CopyDirToTemp("C:\\Users\\Matty\\Desktop\\trophy\\PS3\\NPWR03907_00");

            Utility.PfdTool.DecryptTrophyData(tempPath);
            TROPCONF conf = new TROPCONF(tempPath);
            TROPUSR usr = new TROPUSR(tempPath);
            TROPTRNS trns = new TROPTRNS(tempPath);

            conf.PrintState();
            usr.PrintState();
            trns.PrintState();
            Utility.File.DeleteDirectory(tempPath);
        } // PrintPS3

        static void PrintVita()
        {
            TROP conf = new TROP("C:\\Users\\Matty\\Desktop\\trophy\\vita\\conf\\NPWR03128_00");
            TRPTITLE title = new TRPTITLE("C:\\Users\\Matty\\Desktop\\trophy\\vita\\data\\NPWR03128_00");
            TRPTRANS trans = new TRPTRANS("C:\\Users\\Matty\\Desktop\\trophy\\vita\\data\\NPWR03128_00", conf.TrophyCount);

            conf.PrintState();
            title.PrintState();
            trans.PrintState();
        } // PrintVita

        static void Main(string[] args)
        {
            //PrintPS3();
            //PrintVita();
        } // Main
    } // Program
} // TrophyParser

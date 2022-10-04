// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using TrophyParser;

string tempPath = Utility.File.CopyTrophyDirToTemp("C:\\Users\\Matty\\Desktop\\NPWR03907_00");
Utility.PfdTool.DecryptTrophyData(tempPath);
TROPCONF conf = new(tempPath);

conf.PrintState();
Utility.File.DeleteDirectory(tempPath);

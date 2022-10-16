using System;
using System.IO;

namespace Utility
{
  public class File
  {
    #region Public Methods

    public static string GetFullPath(string directory, string fileName)
    {
      if (directory == null || directory.Trim() == string.Empty)
        throw new Exception("Path cannot be null!");

      string filePath = Path.Combine(directory, fileName);
      if (!System.IO.File.Exists(filePath))
        throw new FileNotFoundException("File not found", filePath);

      return filePath;
    } // GetFullPath

    public static string CopyDirToTemp(string trophyDir)
    {
      DirectoryInfo dir = new DirectoryInfo(trophyDir);
      string pathTemp = Path.Combine(GetTempDirectory(), dir.Name);
      CopyDirectory(trophyDir, pathTemp, true);
      return pathTemp;
    } // CopyTrophyDirToTemp

    public static void CopyDirectory(string source, string target, bool overwrite)
    {
      DirectoryInfo dir = new DirectoryInfo(source);
      if (!dir.Exists)
      {
        throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + source);
      }

      // If the destination directory doesn't exist, create it.
      Directory.CreateDirectory(target);

      // Get the files in the directory and copy them to the new location.
      FileInfo[] files = dir.GetFiles();
      foreach (FileInfo file in files)
      {
        string tempPath = Path.Combine(target, file.Name);
        file.CopyTo(tempPath, overwrite);
      }

      DirectoryInfo[] dirs = dir.GetDirectories();
      foreach (DirectoryInfo subDir in dirs)
      {
        string tempPath = Path.Combine(target, subDir.Name);
        CopyDirectory(subDir.FullName, tempPath, overwrite);
      }
    } // DirectoryCopy

    public static void DeleteDirectory(string path)
    {
      if (Directory.Exists(path))
      {
        Directory.Delete(path, true);
      }
    } // DeleteDirectory

    #endregion Public Methods
    #region Private Methods

    private static string GetTempDirectory()
    {
      string tempDirectory;
      do
      {
        tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
      } while (Directory.Exists(tempDirectory));
      Directory.CreateDirectory(tempDirectory);
      return tempDirectory;
    } // GetTemporaryDirectory

    #endregion Private Methods
  } // File
} // Utility

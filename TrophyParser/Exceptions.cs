using System;

namespace TrophyParser
{
  class InvalidFileException : Exception
  {
    public InvalidFileException(string fileName) : base(string.Format("Not a valid {0}.", fileName)) { }
  }

  public class AlreadyEarnedException : Exception
  {
    public AlreadyEarnedException(string message) : base(message) { }
    public AlreadyEarnedException() : base("Trophy already earned.") { }
  }

  public class SyncTimeException : Exception
  {
    private DateTime psnSyncTime = new DateTime(0);
    public DateTime PsnSyncTime
    {
      get { return psnSyncTime; }
    }

    public SyncTimeException(string message, DateTime psnSyncTime) : base(message)
    {
      this.psnSyncTime = psnSyncTime;
    }
    public SyncTimeException(DateTime psnSyncTime) : base(string.Format("The last trophy synchronized with PSN has the following date: {0:dd/MM/yyyy HH:mm:ss}. Select a date greater than this.", psnSyncTime)) { }
  }

  public class TrophyNotFound : Exception
  {
    public TrophyNotFound(string message) : base(message) { }
    public TrophyNotFound() : base("Trophy ID not found.") { }
  }
}

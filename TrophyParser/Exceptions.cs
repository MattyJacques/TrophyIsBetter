using System;

namespace TrophyParser
{
  class InvalidFileException : Exception
  {
    public InvalidFileException(string fileName) : base(string.Format("Not a valid {0}.", fileName)) { }
  } // InvalidFileException

  public class AlreadyEarnedException : Exception
  {
    public AlreadyEarnedException(string message) : base(message) { }
    public AlreadyEarnedException() : base("Trophy already earned.") { }
  } // AlreadyEarnedException

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
    public SyncTimeException(DateTime psnSyncTime)
      : base(string.Format("The last trophy synchronized with PSN has the following date:" +
        " {0:dd/MM/yyyy HH:mm:ss}. Select a date greater than this.", psnSyncTime)) { }
  } // SyncTimeException

  public class AlreadySyncedException : Exception
  {
    public AlreadySyncedException(int id)
      : base($"Trophy {id} is already synchronized. It can't be modified.") { }
  } // AlreadySyncedException

  public class AlreadyLockedException : Exception
  {
    public AlreadyLockedException(int id)
      : base($"Trophy {id} is already locked.") { }
  } // AlreadySyncedException

  public class TrophyNotFound : Exception
  {
    public TrophyNotFound(string message) : base(message) { }
    public TrophyNotFound() : base("Trophy ID not found.") { }
  } // TrophyNotFound
} // TrophyParser

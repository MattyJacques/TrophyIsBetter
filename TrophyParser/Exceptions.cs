using System;

namespace TrophyParser
{
  class InvalidFileException : Exception
  {
    internal InvalidFileException(string fileName) : base(string.Format("Not a valid {0}.", fileName)) { }
  } // InvalidFileException

  internal class AlreadyEarnedException : Exception
  {
    internal AlreadyEarnedException(string message) : base(message) { }
    internal AlreadyEarnedException() : base("Trophy already earned.") { }
  } // AlreadyEarnedException

  internal class SyncTimeException : Exception
  {
    private DateTime psnSyncTime = new DateTime(0);
    internal DateTime PsnSyncTime
    {
      get { return psnSyncTime; }
    }

    internal SyncTimeException(string message, DateTime psnSyncTime) : base(message)
    {
      this.psnSyncTime = psnSyncTime;
    }
    internal SyncTimeException(DateTime psnSyncTime)
      : base(string.Format("The last trophy synchronized with PSN has the following date:" +
        " {0:dd/MM/yyyy HH:mm:ss}. Select a date greater than this.", psnSyncTime)) { }
  } // SyncTimeException

  internal class AlreadySyncedException : Exception
  {
    internal AlreadySyncedException(int id)
      : base($"Trophy {id} is already synchronized. It can't be modified.") { }
  } // AlreadySyncedException

  internal class AlreadyLockedException : Exception
  {
    internal AlreadyLockedException(int id)
      : base($"Trophy {id} is already locked.") { }
  } // AlreadySyncedException

  internal class TrophyNotFound : Exception
  {
    internal TrophyNotFound(int id) : base($"Trophy {id} not found.") { }
  } // TrophyNotFound
} // TrophyParser

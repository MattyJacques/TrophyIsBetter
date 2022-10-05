using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrophyParser
{
  class InvalidTrophyFileException : Exception
  {
    public InvalidTrophyFileException(string fileName) : base(string.Format("Not a valid {0}.", fileName)) { }
  }
}

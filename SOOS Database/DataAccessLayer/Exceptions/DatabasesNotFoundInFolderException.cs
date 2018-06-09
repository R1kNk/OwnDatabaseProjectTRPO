using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Exceptions
{
    /// <summary>
    /// Exception which handles situation if folder for databases doesn't exist
    /// </summary>
    public class DatabasesNotFoundInFolderException : Exception
    {
        public DatabasesNotFoundInFolderException()
        { }
        public DatabasesNotFoundInFolderException(string message)
            : base(message)
        { }
        public DatabasesNotFoundInFolderException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Security.Cryptography;

namespace SecurityLayer.Modules
{
    internal class DecryptionModule
    {
        /// <summary>
        /// Decrypt bytes to database instance
        /// </summary>
        /// <param name="_dataToDeCrypt"></param>
        /// <returns></returns>
        static public DataLayer.DataBaseInstance DecryptDataBase(byte[] _dataToDeCrypt)
        {
            byte[] _dbInstanceBytes = SharedCryptingMethods.DecryptDatabaseObjectShellArrayToDatabaseBytes(_dataToDeCrypt);
            DataLayer.DataBaseInstance dbObject = SharedCryptingMethods.ByteArrayToDatabaseObject(_dbInstanceBytes);
            return dbObject;

        }

    }
}

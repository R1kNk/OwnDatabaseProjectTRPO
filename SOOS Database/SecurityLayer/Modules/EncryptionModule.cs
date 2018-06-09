using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SecurityLayer.Modules;

namespace SecurityLayer.Modules
{
    internal class EncryptionModule
    {
        /// <summary>
        /// Encrypt db instane to byte array
        /// </summary>
        /// <param name="_dataToCrypt"></param>
        /// <returns></returns>
        static public MemoryStream EncryptDataBase(DataLayer.DataBaseInstance _dataToCrypt)
        {
            byte[] _outputData = SharedCryptingMethods.DatabaseObjectToByteArray(_dataToCrypt);
            byte[] DataBaseObjectInShell = SharedCryptingMethods.EncryptDatabaseBytesToDatabaseObjectShellArray(_outputData);
            return new MemoryStream(DataBaseObjectInShell);
        }
    }
}

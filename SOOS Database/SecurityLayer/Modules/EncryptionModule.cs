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
        static public MemoryStream EncryptDataBase(DataLayer.DataBaseInstance _dataToCrypt)
        {
            byte[] _outputData = SharedCryptingMethods.DatabaseObjectToByteArray(_dataToCrypt);
            byte[] DataBaseObjectInShell = SharedCryptingMethods.EncryptDatabaseBytesToDatabaseObjectShellArray(_outputData);
            return new MemoryStream(DataBaseObjectInShell);
        }
    }
}

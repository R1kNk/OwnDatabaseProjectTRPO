using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;

namespace SecurityLayer.Modules
{
    [Serializable]
    internal class DataBaseObjectShell
    {
        string cryptKey;
        byte[] dataBaseInstanceArray;
        byte[] iv;

        public DataBaseObjectShell(string key, byte[] dbArray, byte[] iv_key)
        {
            cryptKey = key;
            dataBaseInstanceArray = dbArray;
            iv = iv_key;
        }

        public string CryptKey { get => cryptKey; private set => cryptKey = value; }
        public byte[] DataBaseInstanceArray { get => dataBaseInstanceArray; private set => dataBaseInstanceArray = value; }
        public byte[] IV { get => iv; private set => iv = value; }
    }
}

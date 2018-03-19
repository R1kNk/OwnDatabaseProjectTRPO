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
    internal class SharedCryptingMethods
    {
        static internal byte[] DatabaseObjectToByteArray(DataLayer.DataBaseInstance _obj)
        {
            if (_obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, _obj);
                return ms.ToArray();
            }
        }
        static internal DataLayer.DataBaseInstance ByteArrayToDatabaseObject(byte[] dbObjectArray)
        {
            MemoryStream memStream = new MemoryStream(dbObjectArray);
            BinaryFormatter formatter = new BinaryFormatter();

            return (DataLayer.DataBaseInstance)formatter.Deserialize(memStream);
        }
        //
        static internal byte[] DatabaseObjectShellToByteArray(DataBaseObjectShell _obj)
        {
            if (_obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, _obj);
                return ms.ToArray();
            }
        }
        static internal DataBaseObjectShell ByteArrayToDatabaseObjectShell(byte[] dbObjectArray)
        {
            MemoryStream memStream = new MemoryStream(dbObjectArray);
            BinaryFormatter formatter = new BinaryFormatter();

            return (DataBaseObjectShell)formatter.Deserialize(memStream);
        }
        //
        static internal string xorString(string input, string key)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
                sb.Append((char)(input[i] ^ key[(i % key.Length)]));
            String result = sb.ToString();

            return result;
        }
        static internal byte[] GetKeyBytes(string key)
        {
            byte[] passBytes = new UTF8Encoding().GetBytes(key);

            return ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(passBytes);
        }

        static internal string SecretCryptKeyFormula(string key)
        {
            return xorString(key, "hardbass");
        }

        static internal byte[] EncryptDatabaseBytesToDatabaseObjectShellArray(byte[] dbInstanceBytes)
        {
            using (var myAes = Aes.Create())
            {
                Random rand = new Random();
                string password = DateTime.Now.ToString() + DateTime.Now.Millisecond + DateTime.Now.Ticks + rand.Next(Int32.MinValue, Int32.MaxValue).ToString();
                string secretPassword = SecretCryptKeyFormula(password);
                myAes.Key = GetKeyBytes(password);
                myAes.GenerateIV();
                byte[] encryptedDbInstance = AES.encryptStream(dbInstanceBytes, myAes.Key, myAes.IV);
                return DatabaseObjectShellToByteArray(new DataBaseObjectShell(secretPassword, encryptedDbInstance, myAes.IV));
            }
        }
        static internal byte[] DecryptDatabaseObjectShellArrayToDatabaseBytes(byte[] dbShellbytes)
        {
            DataBaseObjectShell shellObject = ByteArrayToDatabaseObjectShell(dbShellbytes);
            byte[] _dbInstance;
            byte[] password = SharedCryptingMethods.GetKeyBytes(SharedCryptingMethods.SecretCryptKeyFormula(shellObject.CryptKey));
            using (var myAes = Aes.Create())
            {
                myAes.Key = password;
                myAes.IV = shellObject.IV;
               _dbInstance = AES.decryptStream(shellObject.DataBaseInstanceArray, myAes.Key, myAes.IV);
            }
            return _dbInstance;
        }
    }
}

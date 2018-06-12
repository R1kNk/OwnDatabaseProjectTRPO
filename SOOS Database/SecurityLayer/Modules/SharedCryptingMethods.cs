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
    /// <summary>
    /// Class represents shared crypting methods
    /// </summary>
    internal class SharedCryptingMethods
    {
        /// <summary>
        /// Convert database object to byte array
        /// </summary>
        /// <param name="_obj"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Convert byte array to database object
        /// </summary>
        /// <param name="dbObjectArray"></param>
        /// <returns></returns>
        static internal DataLayer.DataBaseInstance ByteArrayToDatabaseObject(byte[] dbObjectArray)
        {if (dbObjectArray == null) return null;
            MemoryStream memStream = new MemoryStream(dbObjectArray);
            BinaryFormatter formatter = new BinaryFormatter();

            return (DataLayer.DataBaseInstance)formatter.Deserialize(memStream);
        }
        //
        /// <summary>
        /// database object shell to byte array
        /// </summary>
        /// <param name="_obj"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Convert Byte array to database object shell
        /// </summary>
        /// <param name="dbObjectArray"></param>
        /// <returns></returns>
        static internal DataBaseObjectShell ByteArrayToDatabaseObjectShell(byte[] dbObjectArray)
        {
            try
            {
                MemoryStream memStream = new MemoryStream(dbObjectArray);
                BinaryFormatter formatter = new BinaryFormatter();

                return (DataBaseObjectShell)formatter.Deserialize(memStream);
            } catch(System.Runtime.Serialization.SerializationException)
            {
                return null;
            }
        }
        //
        /// <summary>
        /// Xor string 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        static internal string xorString(string input, string key)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
                sb.Append((char)(input[i] ^ key[(i % key.Length)]));
            String result = sb.ToString();

            return result;
        }
        /// <summary>
        /// Convert key to bytes
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static internal byte[] GetKeyBytes(string key)
        {
            byte[] passBytes = new UTF8Encoding().GetBytes(key);

            return ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(passBytes);
        }
        /// <summary>
        /// Secret key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static internal string SecretCryptKeyFormula(string key)
        {
            return xorString(key, "hardbass");
        }
        /// <summary>
        /// Encrypt database bytes to database object shell
        /// </summary>
        /// <param name="dbInstanceBytes"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Convert bytes of database object shell to database bytes 
        /// </summary>
        /// <param name="dbShellbytes"></param>
        /// <returns></returns>
        static internal byte[] DecryptDatabaseObjectShellArrayToDatabaseBytes(byte[] dbShellbytes)
        {
            DataBaseObjectShell shellObject = ByteArrayToDatabaseObjectShell(dbShellbytes);if (shellObject == null) return null;
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

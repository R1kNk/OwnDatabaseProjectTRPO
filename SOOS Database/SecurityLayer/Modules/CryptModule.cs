using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace SecurityLayer
{
    /// <summary>
    /// Class for converting data to bytes and bytes to data
    /// </summary>
    internal class CryptModule
    {
        /// <summary>
        /// Encrypts database to bytes
        /// </summary>
        /// <param name="_dataToCrypt"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        static public byte[] CryptDataBase(object _dataToCrypt, byte[] _key)
        {
            byte[] _outputData = ObjectToByteArray(_dataToCrypt);

            //Шифруем здесь!
            


            return _outputData;
        }
        /// <summary>
        /// Decrypt bytes to object
        /// </summary>
        /// <param name="_dataToDeCrypt"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        static public object DecryptDataBase(byte[] _dataToDeCrypt, byte[] _key)
        {
            //Дешифруем здесь!


            return (object)_dataToDeCrypt;
        }

        /// <summary>
        /// Converts object to bytes array
        /// </summary>
        /// <param name="_obj"></param>
        /// <returns></returns>
        static public byte[] ObjectToByteArray(object _obj)
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
    }
}

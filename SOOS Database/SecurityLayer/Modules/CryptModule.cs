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
    internal class CryptModule
    {
        static public byte[] CryptDataBase(object _dataToCrypt, byte[] _key)
        {
            byte[] _outputData = ObjectToByteArray(_dataToCrypt);

            //Шифруем здесь!
            


            return _outputData;
        }
        static public object DecryptDataBase(byte[] _dataToDeCrypt, byte[] _key)
        {
            //Дешифруем здесь!


            return (object)_dataToDeCrypt;
        }

        //Преобразуем объект в байты
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

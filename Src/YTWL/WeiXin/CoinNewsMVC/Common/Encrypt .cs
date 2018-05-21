using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public class Encrypt 
    {
        public static string md5(string str)
        {
            byte[] result = Encoding.Default.GetBytes(str);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "").ToLower();
        }

        #region 把对象序列化为字节数组SerializeObject_ToByte
        /// <summary>
        /// 把对象序列化为字节数组
        /// </summary>
        public static byte[] SerializeObject_ToByte(object obj)
        {
            if (obj == null)
                return null;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, bytes.Length);
            ms.Close();
            return bytes;
        }
        #endregion

        #region 把字节数组反序列化成对象DeserializeObject_FromByte
        /// <summary>
        /// 把字节数组反序列化成对象
        /// </summary>
        public static object DeserializeObject_FromByte(byte[] bytes)
        {
            object obj = null;
            if (bytes == null)
                return obj;
            System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
            ms.Position = 0;
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            obj = formatter.Deserialize(ms
    );
            ms.Close();
            return obj;
        }
        #endregion

    }
}

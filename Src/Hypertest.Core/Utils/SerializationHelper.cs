using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Hypertest.Core.Utils
{
    public static class SerializationHelper
    {
        public static byte[] SerializeToBinary<T>(T obj, IEnumerable<Type> extraTypes)
        {
            if (obj == null)
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                DataContractSerializer dcs = new DataContractSerializer(typeof(T), extraTypes);
                dcs.WriteObject(ms, obj);
                return ms.ToArray();
            }
        }

        public static T DeserializeFromBinary<T>(byte[] data, IEnumerable<Type> extraTypes)
        {
            if (data.Length == 0)
                return default(T);

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);
                ms.Seek(0, 0);

                DataContractSerializer dcs = new DataContractSerializer(typeof(T), extraTypes);
                return (T)dcs.ReadObject(ms);
            }
        }
    }
}

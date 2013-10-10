#region License

// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

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
                DataContractSerializer dcs = new DataContractSerializer(typeof (T), extraTypes);
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

                DataContractSerializer dcs = new DataContractSerializer(typeof (T), extraTypes);
                return (T) dcs.ReadObject(ms);
            }
        }
    }
}
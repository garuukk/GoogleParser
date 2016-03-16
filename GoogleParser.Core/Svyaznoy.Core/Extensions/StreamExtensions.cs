using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Svyaznoy.Core
{
    public static class StreamExtensions
    {
        public static byte[] ReadAllBytes(this Stream stream)
        {
            if (stream==null)
            {
                return null;
            }
            stream.Position = 0;
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}

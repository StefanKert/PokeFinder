using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PokeFinder.Misc
{
    public static class MiscExtensions
    {
        public static byte[] ReadAsBytes(this Stream input) {
            byte[] buffer = new byte[16*1024];

            using (MemoryStream ms = new MemoryStream()) {
                int read;

                while ((read = input.Read(buffer, 0, buffer.Length)) > 0) {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        public static async Task<string> ReadAsUnzippedStringAsync(this HttpContent content) {
            var bytes = await content.ReadAsByteArrayAsync();
            var result = Encoding.UTF8.GetString(bytes);
            return result;
        }


        public static string UnZipStr(byte[] input)
        {
            using (MemoryStream inputStream = new MemoryStream(input))
            {
                using (var gzip = new DeflateStream(inputStream, CompressionMode.Decompress))
                {
                    return Encoding.UTF8.GetString(gzip.ReadAsBytes());
                }
            }
        }

        public static void AddSorted<T>(this IList<T> list, T item, Func<T, T, bool> compareItemToList) {
            int i = 0;
            while (i < list.Count && compareItemToList(item, list[i]))
                i++;

            list.Insert(i, item);
        }
    }
}
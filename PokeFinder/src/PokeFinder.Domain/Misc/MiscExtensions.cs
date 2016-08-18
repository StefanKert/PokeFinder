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
        public static byte[] ReadAsBytes(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];

            using (MemoryStream ms = new MemoryStream())
            {
                int read;

                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        public static async Task<string> ReadAsUnzippedStringAsync(this HttpContent content) {
            var bytes = await content.ReadAsByteArrayAsync();
            return UnZipStr(bytes.Skip(2).ToArray());
        }


        public static string UnZipStr(byte[] input)
        {
            using (MemoryStream inputStream = new MemoryStream(input))
            {
                using (DeflateStream gzip = new DeflateStream(inputStream, CompressionMode.Decompress))
                {
                    return Encoding.UTF8.GetString(gzip.ReadAsBytes());
                }
            }
        }
    }
}
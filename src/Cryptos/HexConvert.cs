using System.Text;

namespace SecureConnectionString.Cryptos
{
    public class HexConvert
    {
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public HexConvert()
        {
            Encoding = Encoding.UTF8;
        }

        public static HexConvert Instance
        {
            get
            {
                return new HexConvert();
            }
        }

        public static HexConvert Create()
        {
            return new HexConvert();
        }

        /// <summary>
        /// 字串轉Hex字串
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Encode(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            byte[] bytes = Encoding.GetBytes(text);
            string hexString = ToHexString(bytes);
            return hexString;
        }

        /// <summary>
        /// Hex字串轉字串
        /// </summary>
        /// <param name="bytesString"></param>
        /// <returns></returns>
        public string Decode(string bytesString)
        {
            if (bytesString == null)
            {
                throw new ArgumentNullException(nameof(bytesString));
            }

            try
            {
                byte[] bytes = FromHexString(bytesString);
                return Encoding.GetString(bytes);
            }
            catch
            {
                return bytesString;
            }
        }

        /// <summary>
        /// Bytes轉Hex字串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string ToHexString(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Hex字串轉Bytes
        /// </summary>
        /// <param name="bytesString"></param>
        /// <returns></returns>
        public byte[] FromHexString(string bytesString)
        {
            if (bytesString == null)
            {
                throw new ArgumentNullException(nameof(bytesString));
            }

            int length = bytesString.Length / 2;
            var bytes = new byte[length];

            for (int i = 0; i < length; ++i)
            {
                bytes[i] = Convert.ToByte(bytesString.Substring(i * 2, 2), 16);
            }

            return bytes;
        }
    }
}

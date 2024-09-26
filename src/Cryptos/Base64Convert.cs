using System.Text;

namespace SecureConnectionString.Cryptos
{
    public class Base64Convert
    {
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public bool UrlEncodeFlag { get; set; } = false;

        public Base64Convert()
        {
            UrlEncodeFlag = false;
            Encoding = Encoding.UTF8;
        }

        public static Base64Convert Create(bool urlEncodeFlag = false)
        {
            return new Base64Convert()
            {
                UrlEncodeFlag = urlEncodeFlag
            };
        }


        /// <summary>
        /// 字串轉Base64字串
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
            return ToBase64String(bytes);
        }

        /// <summary>
        /// Bytes轉Base64字串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string ToBase64String(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            string base64String = Convert.ToBase64String(bytes);

            if (UrlEncodeFlag)
            {
                return UrlEncode(base64String);
            }

            return base64String;
        }


        /// <summary>
        /// Base64字串轉字串
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public string Decode(string base64String)
        {
            if (base64String == null)
            {
                throw new ArgumentNullException(nameof(base64String));
            }

            if (UrlEncodeFlag)
            {
                base64String = UrlDecode(base64String);
            }

            try
            {
                byte[] bytes = FromBase64String(base64String);
                return Encoding.GetString(bytes);
            }
            catch
            {
                return base64String;
            }
        }

        /// <summary>
        /// Base64字串轉Bytes
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public byte[] FromBase64String(string base64String)
        {
            return Convert.FromBase64String(base64String);
        }



        /// <summary>
        /// Base64 字串 URL Encode
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        protected string UrlEncode(string base64String)
        {
            if (base64String == null)
            {
                throw new ArgumentNullException(nameof(base64String));
            }

            return base64String.Replace("=", "")
                               .Replace("/", "_")
                               .Replace("+", "-");
        }
        /// <summary>
        /// Base64 字串 URL Encode
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected string UrlEncode(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            string base64String = Convert.ToBase64String(bytes);
            return UrlEncode(base64String);
        }

        /// <summary>
        /// Base64字串 URL Encode
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        protected string UrlDecode(string base64String)
        {
            if (base64String == null)
            {
                throw new ArgumentNullException(nameof(base64String));
            }

            return base64String.PadRight(base64String.Length + (4 - base64String.Length % 4) % 4, '=')
                               .Replace("_", "/")
                               .Replace("-", "+");
        }
    }
}

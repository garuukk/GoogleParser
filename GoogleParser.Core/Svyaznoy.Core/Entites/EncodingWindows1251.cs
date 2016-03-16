namespace Svyaznoy.Core.Entites
{
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public class EncodingWindows1251 : Encoding
    {
        private Encoding encoding;

        /// <summary>
        /// 
        /// </summary>
        public EncodingWindows1251() : base (1251)
        {
            encoding = GetEncoding(1251);
        }

        public override int GetByteCount(char[] chars, int index, int count)
        {
            return encoding.GetByteCount(chars, index, count);
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return encoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return encoding.GetCharCount(bytes, index, count);
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            return encoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        }

        public override int GetMaxByteCount(int charCount)
        {
            return encoding.GetMaxByteCount(charCount);
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return encoding.GetMaxCharCount(byteCount);
        }

        public override string BodyName
        {
            get
            {
                return "windows-1251";
            }
        }
    }
}

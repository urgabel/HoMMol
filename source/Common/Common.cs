using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMMol_core
{
    /// <summary>Shared variables and methods in the whole project</summary>
    public static class Common
    {
        /// <summary>enc can be used from everywhere
        /// <para>"GB18030" = Page Code 54936: Simplified Chinese</para>
        /// </summary>
        public static Encoding enc = Encoding.GetEncoding(54936);

        /// <summary>Parse a 32 bytes array to string</summary>
        /// <returns>a string without '\0' chars at the end</returns>
        public static String Bytes32ToString(Byte[] b)
        {
            return enc.GetString(b).TrimEnd('\0');
        }

        /// <summary>Parse a string to a 32 bytes array</summary>
        /// <returns>a 32 bytes lenght string, adding '\0' chars at the end</returns>
        public static Byte[] StringToBytes32(String s)
        {
            Byte[] eb;
            eb = enc.GetBytes(s);
            Byte[] b = new Byte[32];
            Array.Clear(b, 0, 0);
            Buffer.BlockCopy(eb, 0, b, 0, eb.Length);
            return b;
        }

        /// <summary>Text line separators</summary>
        public static String[] lineSplit = { "\r\n", "\n" };

        /// <summary>Split a string by \r\n or \n</summary>
        /// <param name="s">Multiline string to split</param>
        /// <returns>Array of strings</returns>
        public static String[] SplitLines(String s)
        {
            return s.Split(lineSplit, StringSplitOptions.None);
        }
    }
}

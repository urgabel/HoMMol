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
    }
    /// <summary>A 4 components color value (ARGB)</summary>
    public class D3DColor
    {
        #region Properties
        /// <summary>Alpha channel value, for transparency.</summary>
        public Byte Alpha;
        /// <summary>Red channel value.</summary>
        public Byte Red;
        /// <summary>Green Channel value.</summary>
        public Byte Green;
        /// <summary>Blue Channel value.</summary>
        public Byte Blue;
        #endregion

        #region Constructor
        /// <summary>New default instance</summary>
        public D3DColor()
        {
            Alpha = 0xFF;
            Red = 0xFF;
            Green = 0xFF;
            Blue = 0xFF;
        }
        /// <summary>New instance from provided fields</summary>
        public D3DColor(Byte a, Byte r, Byte g, Byte b)
        {
            Alpha = a;
            Red = r;
            Green = g;
            Blue = b;
        }
        /// <summary>New instance from a 32 bits value, as stored in Matr dbc file</summary>
        public D3DColor(UInt32 c)
        {
            Alpha = (Byte)(c >> 24);
            Red = (Byte)((c & 0x00FF0000) >> 16);
            Green = (Byte)((c & 0x0000FF00) >> 8);
            Blue = (Byte)(c & 0x0000FF00);
        }
        /* 
         * Not added constructor from string to avoid exceptions from 
         * constructors in common classes. When reading an ini file, 
         * parse the string where using this class.
        */
        #endregion

        #region PublicMethods
        /// <summary>Get the color in a Byte array format</summary>
        /// <returns>a 4 bytes buffer array, as stored in Matr dbc file</returns>
        public Byte[] ToBytes()
        {
            Byte[] b = new Byte[4];
            b[0] = Alpha;
            b[1] = Red;
            b[2] = Green;
            b[3] = Blue;
            return b;
        }
        /// <summary>Get the color in UInt32 format</summary>
        /// <returns>a 32 bits value, as stored in Matr dbc file</returns>
        public UInt32 ToUInt32()
        {
            //return (UInt32)(Alpha * 0x1000000 + Red * 0x10000 + Green * 0x100 + Blue);
            return BitConverter.ToUInt32(this.ToBytes(), 0);
        }
        /// <summary>Get the color in string format</summary>
        /// <returns>a 8 chars lenght string, as stored in Matr ini file</returns>
        public override string ToString()
        {
            //return this.ToUInt32().ToString("X8");
            return BitConverter.ToString(this.ToBytes()).Replace("-", string.Empty);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMMol_core.Graphics
{
    /// <summary>Alpha Blending Mode with Direct 3D
    /// It sets the way how to calculate mixed color from image and background
    /// final color = (image color * Asb) + (background color * Adb)
    /// Note that it could be different operators, but usually it is '+'
    /// <remarks>Used with Mesh and Texture for 
    /// Asb and Adb (Alpha Source/Destination Blend)</remarks>
    /// </summary>
    public enum D3DBLEND
    {
        /// <summary>Disable this color (multiply * 0)</summary>
        D3DBLEND_ZERO = 1,

        /// <summary>Enable this color (multiply * 1)</summary>
        D3DBLEND_ONE = 2,

        /// <summary>Image color (image RGBA)</summary>
        D3DBLEND_SRCCOLOR = 3,

        /// <summary>Inverse image color (image RGBA * -1)</summary>
        D3DBLEND_INVSRCCOLOR = 4,

        /// <summary>Image alpha channel (image A)</summary>
        D3DBLEND_SRCALPHA = 5,

        /// <summary>Inverse image alpha channel (image A * -1)</summary>
        D3DBLEND_INVSRCALPHA = 6,

        /// <summary>Background alpha channel (background A)</summary>
        D3DBLEND_DESTALPHA = 7,

        /// <summary>Inverse background alpha channel (background A * -1)</summary>
        D3DBLEND_INVDESTALPHA = 8,

        /// <summary>Background color (background RGBA)</summary>
        D3DBLEND_DESTCOLOR = 9,

        /// <summary>Inverse background color (background RGBA * -1)</summary>
        D3DBLEND_INVDESTCOLOR = 10,
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
         * Not added constructors from string or byte array to avoid 
         * exceptions from constructors in common classes. When reading 
         * an ini file, parse the string where using this class.
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
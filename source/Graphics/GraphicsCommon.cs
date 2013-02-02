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

    /// <summary>Mix options when using more than one texture
    /// <remarks>D3DTOP_* comes from Direct3D</remarks></summary>
    // http://msdn.microsoft.com/en-us/library/windows/desktop/bb172616%28v=vs.85%29.aspx
    public enum MIX_OPT
    {
        /// <summary>D3DTOP_SELECTARG1
        /// Texture stage 1st color unmodified; depending on D3DTSS_COLOROP 
        /// texture-stage state affects the color or the alpha.</summary>
        TEXMIX_NONE = 2,

        /// <summary>D3DTOP_MODULATE, Multiply the components of the 
        /// arguments.</summary>
        TEXMIX_MODULATE = 4,

        /// <summary>D3DTOP_MODULATE, Multiply the components of the 
        /// arguments.</summary>
        TEXMIX_DARK_MAP = 4,

        /// <summary>D3DTOP_MODULATE2X, Multiply the components of the 
        /// arguments, and shift the products to the left 1 bit 
        /// (effectively multiplying them by 2) for brightening.</summary>
        TEXMIX_MODULATE2X = 5,

        /// <summary>D3DTOP_MODULATE4X, Multiply the components of the 
        /// arguments, and shift the products to the left 2 bits 
        /// (effectively multiplying them by 4) for brightening.</summary>
        TEXMIX_MODULATE4X = 6,

        /// <summary>D3DTOP_ADD, Add the components of the arguments.</summary>
        TEXMIX_ADD = 7,

        /// <summary>D3DTOP_ADDSIGNED, Add the components of the arguments 
        /// with a - 0.5 bias, making the effective range of values 
        /// from - 0.5 through 0.5.</summary>
        TEXMIX_ADDSIGNED = 8,

        /// <summary>D3DTOP_ADDSIGNED2X, Add the components of the arguments 
        /// with a - 0.5 bias, and shift the products to the left 1 bit.</summary>
        TEXMIX_ADDSIGNED2X = 9,

        /// <summary>D3DTOP_SUBTRACT, Subtract the components of the second 
        /// argument from those of the first argument.</summary>
        TEXMIX_SUBTRACT = 10,

        /// <summary>D3DTOP_ADDSMOOTH, Add the first and second arguments; 
        /// then subtract their product from the sum.</summary>
        TEXMIX_ADDSMOOTH = 11,

        /// <summary>D3DTOP_BLENDTEXTUREALPHA, Linearly blend this texture 
        /// stage, using the alpha from this stage's texture.</summary>
        TEXMIX_DECAL_MAP = 13,

        /// <summary>D3DTOP_BLENDFACTORALPHA, Linearly blend this texture 
        /// stage, using a scalar alpha set with the D3DRS_TEXTUREFACTOR 
        /// render state.</summary>
        TEXMIX_BLEND_FACTOR_ALPHA = 14,

        /// <summary>D3DTOP_BLENDTEXTUREALPHAPM, Linearly blend a texture 
        /// stage that uses a premultiplied alpha.</summary>
        TEXMIX_BLEND_PRE_ALPHA = 15,

        /// <summary>D3DTOP_BLENDCURRENTALPHA, Linearly blend this texture 
        /// stage, using the alpha taken from the previous texture stage.</summary>
        TEXMIX_BLEND_CUR_ALPHA = 16,

        /// <summary>Reflection map</summary>
        TEXMIX_REFLECT = 17, 

        /// <summary>Use Alpha from 1st texture?</summary>
        TEXMIX_REF_BASEALPHA = 18,

        /// <summary>Rendering silhouette</summary>
        TEXMIX_SILHOUETTE = 19,

        /// <summary>Cartoon Crochet</summary>
        TEXMIX_CARTOON_SILHOUETTE = 20,

        /// <summary>Cartoon tone shadow</summary>
        TEXMIX_CARTOON_SHADOW = 21,

        /// <summary>Cartoon Crochet + Cartoon tone shadow</summary>
        TEXMIX_CARTOON_FULLSTYLE = 22,

        /// <summary>Mobile reflection map</summary>
        TEXMIX_FLUID_REFLECT = 23,

        /// <summary>RGB from 1st texture, Alpha from 2nd texture</summary>
        TEXMIX_USE_FIRST_RGB_SECOND_ALPHA = 25,

        /// <summary>RGB from 1st texture, Alpha from 2nd and 3rd textures 
        /// multiplying values</summary>
        TEXMIX_USE_FIRST_RGB_MODULATE_SECOND_ALPHA = 26,

        /// <summary>Alpha value changes between the two textures mixed to 
        /// produce a gradient effect</summary>
        TEXMIX_BLEND_CLIENT_ALPHA = 27,

        /// <summary>Normal map</summary>
        TEXMIX_DOT3_NORMALMAP = 28,

        /// <summary>Spherical map</summary>
        TEXMIX_ENV_SPHEREMAP = 29,

        /// <summary>Projective Shadow</summary>
        TEXMIX_ENV_PROJECTIVESHADOW = 30,

        /// <summary>Shadow Map</summary>
        TEXMIX_ENV_SHADOWMAP = 31,
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
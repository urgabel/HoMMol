using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMMol_core.Graphics
{
    /// <summary>Handles an Effect Part data structure, as a single frame in 
    /// <para>a 3D Effect animation.</para>
    /// <remarks>Used by Effect class</remarks>
    /// <example>
    /// Example of an effect part in a Effe ini file:
    /// EffectId0=50974
    /// TextureId0=50974
    /// Scale0=51
    /// ASB0=5
    /// ADB0=2
    /// ZBuffer0=1
    /// </example>
    /// </summary>
    //Scale and ZBuffer are not present in ini files when default values:
    //  ZBuffer = 0,  Scale = 100
    public class EffectPart
    {
        #region Constants
        /// <summary>MAGIC_TYPE_TXT</summary>
        /// <value>Binary files header, in text format</value>
        public const String MAGIC_TYPE_TXT = "EffP";

        /// <summary>MAGIC_TYPE_BIN</summary>
        /// <value>Binary files header, in binary format</value>
        public const UInt32 MAGIC_TYPE_BIN = 0x50666645; // "EffP"
        #endregion

        #region Properties
        /// <summary>Effect Id, in 3DEffectObj.ini</summary>
        /// <value>Effect Id, in 3DEffectObj.ini</value>
        public UInt32 EffectId;

        /// <summary>Texture Id, in 3DTexture.ini</summary>
        /// <value>Texture Id, in 3DTexture.ini</value>
        public UInt32 TextureId;

        /// <summary>Optional Scale, 1 (100%) means no scaling</summary>
        /// <remarks>If 1 (100%), it is not shown in ini files</remarks>
        /// <value>Optional Scale, 1 (100%) means no scaling</value>
        public float Scale;        // By default is 1 (100%)

        /// <summary>Asb</summary>
        /// <value>Alfa Source Blend
        /// <seealso cref="D3DBLEND">check possible options</seealso></value>
        public D3DBLEND Asb;        // Alfa Source Blend. Usually 5.

        /// <summary>Adb</summary>
        /// <value>Alfa Destination Blend
        /// <seealso cref="D3DBLEND">check possible options</seealso></value>
        public D3DBLEND Adb;        // Alfa Destination Blend. Usually 6.

        /// <summary>Rendering occupy space, optional, default 0. 
        /// With other 3D objects produce realistic 3D crossover.</summary>
        /// <remarks>If 0, it is not shown in ini files</remarks>
        /// <value>Rendering occupy space, with other 3D objects produce 
        /// realistic 3D crossover, optional, default 0.</value>
        public Boolean ZBuffer;    // Internally is a boolean

        /// <summary>Billboard type</summary>
        /// <remarks>If 0, it is not shown in ini files</remarks>
        /// <value>Billboard type</value>
        public Boolean Billboard;
        #endregion

        #region Constructor
        /// <summary>New default empty Effect Part</summary>
        public EffectPart()
        {
            EffectId = 0;
            TextureId = 0;
            Scale = 1;
            Asb = D3DBLEND.D3DBLEND_SRCALPHA;
            Adb = D3DBLEND.D3DBLEND_INVSRCALPHA;
            ZBuffer = false;
            Billboard = false;
        }

        /// <summary>New Effect Part from provided fields, all fields 
        /// but Billboard is optional</summary>
        /// <param name="E">Effect Id, in 3DEffectObj.ini</param>
        /// <param name="T">Texture Id, in 3DTexture.ini</param>
        /// <param name="S">Scale, 1 (100%) means no scaling</param>
        /// <param name="As">Asb, Alpha Source Blending mode
        /// <seealso cref="D3DBLEND">check possible options</seealso></param>
        /// <param name="Ad">Adb, Alpha Destination Blending mode
        /// <seealso cref="D3DBLEND">check possible options</seealso></param>
        /// <param name="Z">Rendering occupy space, default false(0)</param>
        /// <param name="B">Optional, Billboard type, default false (0)</param>
        public EffectPart(UInt32 E, UInt32 T, float S, D3DBLEND As, 
            D3DBLEND Ad, Boolean Z, Boolean B = false)
        {
            EffectId = E;
            TextureId = T;
            Scale = S;
            Asb = As;
            Adb = Ad;
            ZBuffer = Z;
            Billboard = B;
        }

        /// <summary>New Effect Part from provided fields, all fields 
        /// except Scale (1 -> 100% by default), and Billboard is optional 
        /// as default = false (0)</summary>
        /// <param name="E">Effect Id, in 3DEffectObj.ini</param>
        /// <param name="T">Texture Id, in 3DTexture.ini</param>
        /// <param name="As">Asb, Alpha Source Blending mode
        /// <seealso cref="D3DBLEND">check possible options</seealso></param>
        /// <param name="Ad">Adb, Alpha Destination Blending mode
        /// <seealso cref="D3DBLEND">check possible options</seealso></param>
        /// <param name="Z">Rendering occupy space, default false(0)</param>
        /// <param name="B">Optional, Billboard type, default false (0)</param>
        public EffectPart(UInt32 E, UInt32 T, D3DBLEND As,
            D3DBLEND Ad, Boolean Z, Boolean B = false)
        {
            new EffectPart(E, T, 1, As, Ad, Z, B);
        }

        /// <summary>New Effect Part from provided fields, all fields 
        /// except ZBuffer as false by default (0), and Billboard
        /// is optional, as default = false (0)</summary>
        /// <param name="E">Effect Id, in 3DEffectObj.ini</param>
        /// <param name="T">Texture Id, in 3DTexture.ini</param>
        /// <param name="S">Scale, 1 (100%) means no scaling</param>
        /// <param name="As">Asb, Alpha Source Blending mode
        /// <seealso cref="D3DBLEND">check possible options</seealso></param>
        /// <param name="Ad">Adb, Alpha Destination Blending mode
        /// <seealso cref="D3DBLEND">check possible options</seealso></param>
        /// <param name="B">Optional, Billboard type, default false (0)</param>
        public EffectPart(UInt32 E, UInt32 T, float S, D3DBLEND As,
            D3DBLEND Ad, Boolean B = false)
        {
            new EffectPart(E, T, S, As, Ad, false, B);
        }

        /// <summary>New Effect Part from provided fields, all fields 
        /// except Scale (1 -> 100% by default) and ZBuffer as false (0), 
        /// and Billboard is optional, as default = false (0)</summary>
        /// <param name="E">Effect Id, in 3DEffectObj.ini</param>
        /// <param name="T">Texture Id, in 3DTexture.ini</param>
        /// <param name="As">Asb, Alpha Source Blending mode
        /// <seealso cref="D3DBLEND">check possible options</seealso></param>
        /// <param name="Ad">Adb, Alpha Destination Blending mode
        /// <seealso cref="D3DBLEND">check possible options</seealso></param>
        /// <param name="B">Optional, Billboard type, default false (0)</param>
        public EffectPart(UInt32 E, UInt32 T, D3DBLEND As,
            D3DBLEND Ad,  Boolean B = false)
        {
            new EffectPart(E, T, 1, As, Ad, false, B);
        }

        /// <summary>New Effect Part from byte array, as in dbc Effe file</summary>
        /// <param name="buffer">16 bytes of data read from binary dbc file</param>
        public EffectPart(Byte[] buffer)
        {
            // Effe dbc binary files always uses fixed length structure
            if (buffer.Length < 16)
            {
                new EffectPart();
            }
            else
            {
                EffectId = BitConverter.ToUInt32(buffer, 0);
                TextureId = BitConverter.ToUInt32(buffer, 4);
                Scale = BitConverter.ToSingle(buffer, 8);
                Asb = (D3DBLEND)buffer[12];
                Adb = (D3DBLEND)buffer[13];
                ZBuffer = (buffer[14] != 0);
                Billboard = (buffer[15] != 0);
            }
        }

        /// <summary>New Effect Part from multiline string</summary>
        /// <param name="str">Lines separated by \r\n, like with this.ToString()</param>
        public EffectPart(String str)
        {
            String[] st = Common.SplitLines(str);
            new ModelPart(st);
        }

        /// <summary>New Effect Part from string array, as in ini files
        /// <remarks>Does not check if correct format</remarks></summary>
        /// <param name="str">Array of strings, one for each line</param>
        public EffectPart(String[] str)
        {
            ReadFromArray(str, 0);
        }
        #endregion

        #region PublicMethods
        /// <summary>Type of the class</summary>
        /// <returns>Returns "EffP"</returns>
        public String Type()
        {
            return MAGIC_TYPE_TXT;
        }

        /// <summary>Check if provided instance has the same values</summary>
        /// <returns>Return true if same values</returns>
        public Boolean Equals(EffectPart E)
        {
            if ((E.EffectId != this.EffectId)) return false;
            if ((E.TextureId != this.TextureId)) return false;
            if ((E.Scale != this.Scale)) return false;
            if ((E.Asb != this.Asb)) return false;
            if ((E.Adb != this.Adb)) return false;
            if ((E.ZBuffer != this.ZBuffer)) return false;
            if ((E.Billboard != this.Billboard)) return false;
            return true;
        }

        /// <summary>Parse to String without index</summary>
        /// <returns>Return a string ready to save to an ini file</returns>
        // Needed to override in case of optional index not provided
        public override String ToString()
        {
            return ToString(0);
        }

        /// <summary>Parse to a String with an index in each field</summary>
        /// <remarks>Scale, ZBuffer and Billboard are skipped when 
        /// default values</remarks>
        /// <param name="i">Optional, index, inside an Effect structure</param>
        /// <returns>Returns a string ready to save to an ini file</returns>
        public String ToString(UInt32 i = 0xFFFFFFFF)
        {
            String s = "";
            String index = "";
            if (i != 0xFFFFFFFF) 
                index = i.ToString();
            s += "EffectId" + index + "=" + EffectId.ToString() + "\r\n";
            s += "TextureId" + index + "=" + TextureId.ToString() + "\r\n";
            if (Scale != 1) 
                s += "Scale" + index + "=" + (100 * Scale).ToString() + "\r\n";
            s += "ASB" + index + "=" + Asb.ToString() + "\r\n";
            s += "ADB" + index + "=" + Adb.ToString() + "\r\n";
            if (ZBuffer) 
                s += "ZBuffer" + index + "=1\r\n";
            if (Billboard) 
                s += "Billboard" + index + "=1";
            return s;
        }

        /// <summary>Parse to array of bytes</summary>
        /// <returns>Return an array of 16 bytes ready to save to a dbc file</returns>
        public Byte[] ToBytes()
        {
            Byte[] b = new Byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(EffectId), 0, b, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(TextureId), 0, b, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Scale / 100), 0, b, 8, 4);
            b[12] = (byte)Asb;
            b[13] = (byte)Adb;
            b[14] = (ZBuffer ? (byte)1 : (byte)0);
            b[15] = (Billboard ? (byte)1: (byte)0);
            return b;
        }

        /// <summary>Read Effect Part from string array, as in ini files
        /// <remarks>Does not check if correct format</remarks></summary>
        /// <param name="str">Array of strings, one for each line</param>
        /// <param name="i">Optional, Array index to start reading from
        /// by default is zero</param>
        /// <returns>next index to read</returns>
        public int ReadFromArray(String[] str, int i = 0)
        {
            int j = str.Length;
            Scale = 100;
            ZBuffer = false;
            Billboard = false;
            if (i < j && str[i].StartsWith("EffectId"))
            {
                EffectId = UInt32.Parse(str[i].Split('=')[1]);
                i++;
            }
            if (i < j && str[i].StartsWith("TextureId"))
            {
                TextureId = UInt32.Parse(str[i].Split('=')[1]);
                i++;
            }
            if (i < j && str[i].StartsWith("Scale"))
            {
                Scale = float.Parse(str[i].Split('=')[1]) / 100;
                i++;
            }
            if (i < j && str[i].StartsWith("ASB"))
            {
                Asb = (D3DBLEND)Byte.Parse(str[i].Split('=')[1]);
                i++;
            }
            if (i < j && str[i].StartsWith("ADB"))
            {
                Adb = (D3DBLEND)Byte.Parse(str[i].Split('=')[1]);
                i++;
            }
            if (i < j && str[i].StartsWith("ZBuffer"))
            {
                ZBuffer = (Byte.Parse(str[i].Split('=')[1]) != 0);
                i++;
            }
            if (i < j && str[i].StartsWith("Billboard"))
            {
                Billboard = (Byte.Parse(str[i].Split('=')[1]) != 0);
                i++;
            }
            return i;
        }
        #endregion
    }
}

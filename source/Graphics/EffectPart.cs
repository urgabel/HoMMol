using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMMol_core.Graphics
{
    /// <summary>Handles an Effect Part data structure
    /// <remarks>only handles a part, not a full entry in Effe files</remarks>
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

        /// <summary>Optional Scale, 100 means no scaling</summary>
        /// <remarks>If 100%, it is not shown in ini files</remarks>
        /// <value>Optional Scale, 100 means no scaling</value>
        public UInt32 Scale;

        /// <summary>Asb</summary>
        /// <value>Alfa Source Blend
        /// <seealso cref="D3DBLEND">check possible options</seealso></value>
        public Byte Asb;        // Alfa Source Blend. Usually 5.

        /// <summary>Adb</summary>
        /// <value>Alfa Destination Blend
        /// <seealso cref="D3DBLEND">check possible options</seealso></value>
        public Byte Adb;        // Alfa Destination Blend. Usually 6.

        /// <summary>Rendering occupy space, optional, default 0. 
        /// With other 3D objects produce realistic 3D crossover.</summary>
        /// <remarks>If 0, it is not shown in ini files</remarks>
        /// <value>Rendering occupy space, with other 3D objects produce 
        /// realistic 3D crossover, optional, default 0.</value>
        public UInt16 ZBuffer;
        #endregion

        #region Constructor
        /// <summary>New default empty Effect Part</summary>
        public EffectPart()
        {
            EffectId = 0;
            TextureId = 0;
            Scale = 100;
            Asb = 5;
            Adb = 6;
            ZBuffer = 0;
        }

        /// <summary>New Effect Part from provided fields, 
        /// all fields, but ZBuffer is optional, default = 0</summary>
        /// <param name="E">Effect Id, in 3DEffectObj.ini</param>
        /// <param name="T">Texture Id, in 3DTexture.ini</param>
        /// <param name="S">Scale, 100 means no scaling</param>
        /// <param name="As">Asb, Alpha Source Blending mode
        /// <seealso cref="D3DBLEND">check possible options</seealso></param>
        /// <param name="Ad">Adb, Alpha Destination Blending mode
        /// <seealso cref="D3DBLEND">check possible options</seealso></param>
        /// <param name="Z">Optional, Rendering occupy space, default 0</param>
        public EffectPart(UInt32 E, UInt32 T, UInt32 S,
            Byte As, Byte Ad, UInt16 Z = 0)
        {
            EffectId = E;
            TextureId = T;
            Scale = S;
            Asb = As;
            Adb = Ad;
            ZBuffer = Z;
        }

        /// <summary>New Effect Part from provided fields, with default 
        /// scale = 100%, and ZBuffer optional, default = 0</summary>
        /// <param name="E">Effect Id, in 3DEffectObj.ini</param>
        /// <param name="T">Texture Id, in 3DTexture.ini</param>
        /// <param name="As">Asb, Alpha Source Blending mode
        /// <seealso cref="D3DBLEND">check possible options</seealso></param>
        /// <param name="Ad">Adb, Alpha Destination Blending mode
        /// <seealso cref="D3DBLEND">check possible options</seealso></param>
        /// <param name="Z">Optional, Rendering occupy space, default 0</param>
        public EffectPart(UInt32 E, UInt32 T, Byte As, Byte Ad, UInt16 Z = 0)
        {
            new EffectPart(E, T, 100, As, Ad, Z);
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
                Scale = BitConverter.ToUInt32(buffer, 8);
                Asb = buffer[12];
                Adb = buffer[13];
                ZBuffer = BitConverter.ToUInt16(buffer, 14);
            }
        }

        /// <summary>New Effect Part from multiline string</summary>
        /// <param name="str">7 Lines separated by \r\n, like with this.ToString()</param>
        /// <exception cref="ArgumentOutOfRangeException">if less than 7
        /// strings in the array (inherited)</exception>
        public EffectPart(String str)
        {
            String[] st = Common.SplitLines(str);
            new ModelPart(st);
        }

        /// <summary>New Effect Part from string array, as in ini files
        /// <remarks>Does not check if correct format</remarks></summary>
        /// <param name="str">Array of 7 strings, one for each line</param>
        /// <exception cref="ArgumentOutOfRangeException">if less than 7
        /// strings in the array</exception>
        public EffectPart(String[] str)
        {
            if (str.Length < 7)
                throw new ArgumentOutOfRangeException("str", "there are too few lines; expected 7, found " + str.Length.ToString());
            EffectId = UInt32.Parse(str[0].Split('=')[1]);
            TextureId = UInt32.Parse(str[1].Split('=')[1]);
            Scale = UInt32.Parse(str[2].Split('=')[1]);
            Asb = Byte.Parse(str[3].Split('=')[1]);
            Adb = Byte.Parse(str[4].Split('=')[1]);
            ZBuffer = UInt16.Parse(str[5].Split('=')[1]);
        }
        #endregion

        #region PublicMethods
        /// <summary>Type of the class</summary>
        /// <returns>Return "EffP"</returns>
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
        /// <remarks>Scale and ZBuffer are skipped when default values</remarks>
        /// <param name="i">Optional, index, inside an Effect structure</param>
        /// <returns>Return a string ready to save to an ini file</returns>
        public String ToString(UInt32 i = 0)
        {
            String s = "";
            String index = i.ToString();     // By default is 0
            s += "EffectId" + index + "=" + EffectId + "\r\n";
            s += "TextureId" + index + "=" + TextureId + "\r\n";
            if (Scale != 100) s += "Scale" + index + "=" + Scale + "\r\n";
            s += "ASB" + index + "=" + Asb + "\r\n";
            s += "ADB" + index + "=" + Adb + "\r\n";
            if (ZBuffer != 0) s += "ZBuffer" + index + "=" + ZBuffer + "\r\n";
            return s;
        }

        /// <summary>Parse to array of bytes</summary>
        /// <returns>Return an array of 16 bytes ready to save to a dbc file</returns>
        public Byte[] ToBytes()
        {
            Byte[] b = new Byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(EffectId), 0, b, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(TextureId), 0, b, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Scale), 0, b, 8, 4);
            b[12] = Asb;
            b[13] = Adb;
            Buffer.BlockCopy(BitConverter.GetBytes(ZBuffer), 0, b, 14, 2);
            return b;
        }
        #endregion
    }
}

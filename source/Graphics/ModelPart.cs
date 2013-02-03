using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMMol_core.Graphics
{
    /// <summary>Handles a Model Part data structure
    /// <remarks>only handles a part, not a full entry in Mesh files</remarks>
    /// </summary>
    //Example of a model part in a Mesh ini file:
    //Mesh0=1001000000
    //Texture0=1001000000
    //MixTex0=0
    //MixOpt0=0
    //Asb0=5
    //Adb0=6
    //Material0=ÈËÀàÆ¤·ô
    public class ModelPart
    {
        #region Constants
        /// <summary>MAGIC_TYPE_TXT</summary>
        /// <value>Binary files header, in text format</value>
        public const String MAGIC_TYPE_TXT = "MESH";

        /// <summary>MAGIC_TYPE_BIN</summary>
        /// <value>Binary files header, in binary format</value>
        public const UInt32 MAGIC_TYPE_BIN = 0x4853454D; // "MESH"

        /// <summary>MAX_MESH_PARTS</summary>
        /// <value>Any Role Part can have upto 4 meshes at most</value>
        // Maybe this should be in Graphics.Common
        public const int MAX_MESH_PARTS = 4;
        #endregion

        #region Properties
        /// <summary>Mesh</summary>
        /// <value>Mesh Id, in 3DObj.ini</value>
        public UInt32 Mesh;     // Mesh Id

        /// <summary>Texture</summary>
        /// <value>Texture Id, in 3DTexture.ini</value>
        public UInt32 Texture;  // Texture Id

        /// <summary>MixTex</summary>
        /// <value>Secondary Texture Id to mix, in 3DTexture.ini, 
        /// when using more than one</value>
        public UInt32 MixTex;   // Texture Id to mix, when using more than one

        /// <summary>MixOpt</summary>
        /// <value>Mixing options, 
        /// <seealso cref="MIX_OPT">check possible Mix options</seealso></value>
        public Byte MixOpt;     // Texture Mixing options. Usually 0 for 1st texture, 17 (0x11) for 2nd texture

        /// <summary>Asb</summary>
        /// <value>Alfa Source Blend
        /// <seealso cref="D3DBLEND">check possible options</seealso></value>
        public Byte Asb;        // Alfa Source Blend. Usually 5.

        /// <summary>Adb</summary>
        /// <value>Alfa Destination Blend
        /// <seealso cref="D3DBLEND">check possible options</seealso></value>
        public Byte Adb;        // Alfa Destination Blend. Usually 6.

        /// <summary>Material</summary>
        /// <remarks>If reading binary Mesh file, it is needed to check 
        /// Materials.ini to pass here the name (chinese encoded)</remarks>
        /// <value>Material Id name, in Material.ini</value>
        public String Material; // Usually 1="ÈËÀàÆ¤·ô" ("human skin" in chinese) 
                                //    for 1st texture, 0="default" for 2nd texture
                                //    If reading binary Mesh file, it is needed
                                //    to check Materials.ini to pass here the name
        #endregion

        #region Constructor
        /// <summary>New default empty Model Part</summary>
        public ModelPart()
        {
            Mesh = 0;
            Texture = 0;
            MixTex = 0;
            MixOpt = 0;
            Asb = 5;
            Adb = 6;
            Material = "default";
        }

        /// <summary>New Model Part from provided fields</summary>
        /// <param name="M">Mesh Id, in 3DObj.ini</param>
        /// <param name="T">Texture Id, in 3DTexture.ini</param>
        /// <param name="MT">MixTex, 2nd texture to mix with</param>
        /// <param name="MO">MixOpt, mixing options
        /// <seealso cref="MIX_OPT">check possible Mix options</seealso></param>
        /// <param name="As">Asb, Alpha Source Blending mode
        /// <seealso cref="D3DBLEND">check possible options</seealso></param>
        /// <param name="Ad">Adb, Alpha Destination Blending mode
        /// <seealso cref="D3DBLEND">check possible options</seealso></param>
        /// <param name="Mat">Material Id name, in Material.ini</param>
        public ModelPart(UInt32 M, UInt32 T, UInt32 MT, Byte MO, 
            Byte As, Byte Ad, String Mat)
        {
            Mesh = M;
            Texture = T;
            MixTex = MT;
            MixOpt = MO;
            Asb = As;
            Adb = Ad;
            Material = Mat;
        }

        /// <summary>New Model Part from byte array, as in dbc Mesh file
        /// <remarks>For material it will be used "Material##", check 
        /// <see cref="ModelPart(Byte[], String)"/>
        /// </remarks></summary>
        /// <param name="buffer">Data read from binary dbc file</param>
        public ModelPart(Byte[] buffer)
        {
            // Mesh dbc binary files always uses fixed length structure
            if (buffer.Length < 16)
            {
                new ModelPart();
            }
            else
            {
                new ModelPart(buffer, "Material" + buffer[15].ToString());
            }
        }

        /// <summary>New Model Part from byte array, as in dbc Mesh file</summary>
        /// <param name="buffer">Data read from binary dbc file</param>
        /// <param name="material">Material Id name, in Material.ini</param>
        public ModelPart(Byte[] buffer, String material)
        {
            // Mesh dbc binary files always uses fixed length structure
            if (buffer.Length < 16)
            {
                new ModelPart();
            }
            else
            {
                Mesh = BitConverter.ToUInt32(buffer, 0);
                Texture = BitConverter.ToUInt32(buffer, 4);
                MixTex = BitConverter.ToUInt32(buffer, 8);
                MixOpt = buffer[12];
                Asb = buffer[13];
                Adb = buffer[14];
                Material = material;
            }
        }

        /// <summary>New Model Part from multiline string</summary>
        /// <param name="str">Lines separated by \r\n, like with this.ToString()</param>
        /// <exception cref="ArgumentOutOfRangeException">if less than 7
        /// strings in the array (inherited)</exception>
        public ModelPart(String str)
        {
            String[] st = Common.SplitLines(str);
            new ModelPart(st);
        }

        /// <summary>New Model Part from string array, as in ini files
        /// <remarks>Does not check if correct format</remarks></summary>
        /// <param name="str">Array of strings, one for each line</param>
        /// <exception cref="ArgumentOutOfRangeException">if less than 7
        /// strings in the array</exception>
        public ModelPart(String[] str)
        {
            if (str.Length < 7)
                throw new ArgumentOutOfRangeException("str", "there are too few lines; expected 7, found " + str.Length.ToString());
            Mesh = UInt32.Parse(str[0].Split('=')[1]);
            Texture = UInt32.Parse(str[1].Split('=')[1]);
            MixTex = UInt32.Parse(str[2].Split('=')[1]);
            MixOpt= Byte.Parse(str[3].Split('=')[1]);
            Asb = Byte.Parse(str[4].Split('=')[1]);
            Adb = Byte.Parse(str[5].Split('=')[1]);
            Material = str[6].Split('=')[1];
        }
        #endregion

        #region PublicMethods
        /// <summary>Type of the class</summary>
        /// <returns>Return "MESH"</returns>
        public String Type()
        {
            return MAGIC_TYPE_TXT;
        }

        /// <summary>Check if provided instance has the same values</summary>
        /// <param name="M">Model to compare with</param>
        /// <returns>Return true if same values</returns>
        public Boolean Equals(ModelPart M)
        {
            if ((M.Mesh != this.Mesh)) return false;
            if ((M.Texture != this.Texture)) return false;
            if ((M.MixTex != this.MixTex)) return false;
            if ((M.MixOpt != this.MixOpt)) return false;
            if ((M.Asb != this.Asb)) return false;
            if ((M.Adb != this.Adb)) return false;
            if ((M.Material != this.Material)) return false;
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
        /// <param name="i">Optional, index, inside an Model structure</param>
        /// <returns>Return a string ready to save to an ini file</returns>
        public String ToString(UInt32 i = 0)
        {
            String s = "";
            String index = i.ToString();     // By default is 0
            s += "Mesh" + index + "=" + Mesh + "\r\n";
            s += "Texture" + index + "=" + Texture + "\r\n";
            s += "MixTex" + index + "=" + MixTex + "\r\n";
            s += "MixOpt" + index + "=" + MixOpt + "\r\n";
            s += "Asb" + index + "=" + Asb + "\r\n";
            s += "Adb" + index + "=" + Adb + "\r\n";
            s += "Material" + index + "=" + Material;
            return s;
        }

        /// <summary>Parse to array of bytes
        /// <remarks>This class does not read Material.ini file, so this 
        /// method check if Material Id name is in the form "Material##" 
        /// to use the number, or a zero otherwise. 
        /// <see cref="ToBytes(Byte)"/></remarks>
        /// </summary>
        /// <returns>Return an array of 16 bytes ready to save to a dbc file</returns>
        public Byte[] ToBytes()
        {
            Byte b;
            if (Byte.TryParse(Material.Substring(9, Material.Length - 9), out b))
            {
                return this.ToBytes(b);
            }
            return this.ToBytes(0);
        }

        /// <summary>Parse to array of bytes</summary>
        /// <param name="material">Material Id order number, in Material.ini</param>
        /// <returns>Return an array of 16 bytes ready to save to a dbc file</returns>
        public Byte[] ToBytes(Byte material)
        {
            Byte[] b = new Byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(Mesh), 0, b, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Texture), 0, b, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(MixTex), 0, b, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(MixOpt), 0, b, 12, 1);
            Buffer.BlockCopy(BitConverter.GetBytes(Asb), 0, b, 13, 1);
            Buffer.BlockCopy(BitConverter.GetBytes(Adb), 0, b, 14, 1);
            b[15] = material;
            return b;
        }
        #endregion
    }
}

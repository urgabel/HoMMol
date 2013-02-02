using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HoMMol_core.Ini;

namespace HoMMol_core.Graphics
{
    /// <summary>Handles a Model data structure
    /// <remarks>Used by MeshFile class, using ModelPart class</remarks>
    /// </summary>
    //Example of a model in a Mesh ini file:
    //[1000000]
    //Part=1
    //Mesh0=1001000000
    //Texture0=1001000000
    //MixTex0=0
    //MixOpt0=0
    //Asb0=5
    //Adb0=6
    //Material0=ÈËÀàÆ¤·ô
    class Model
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
        /// <summary>Model Id number</summary>
        public UInt32 Id = 0;

        /// <summary>Amount of parts</summary>
        public UInt32 Amount = 0;      // Max should be 4

        /// <summary>Stores model parts</summary>
        public List <ModelPart> Data = null;

        /// <summary>Comment lines in ini file</summary>
        String Comments = null;
        #endregion

        #region Constructor
        /// <summary>Default Constructor: New Model empty instance
        /// <remarks>Does not handle the file, just the data</remarks></summary>
        public Model()
        {
            Data = new List <ModelPart>();
        }

        /// <summary>New Model from byte array, as in dbc Mesh file
        /// <remarks>For material it will be used "Material##", check 
        /// <see cref="Model(UInt32, UInt32, Byte[], String[])"/>
        /// </remarks></summary>
        /// <param name="Id">Model Id Number</param>
        /// <param name="Amount">Amount of parts</param>
        /// <param name="buffer">Data read from binary dbc file</param>
        public Model(UInt32 Id, UInt32 Amount, Byte[] buffer)
        {
            // Mesh dbc binary files always uses fixed length structure
            if (buffer.Length < 16 * Amount)
            {
                new Model();
            }
            else
            {
                String[] materials = new String[Amount];
                for (int i = 0; i < Amount; i++)
                    materials[i] = "Material" + buffer[i * 16 + 15].ToString();
                new Model(Id, Amount, buffer, materials);
            }
        }

        /// <summary>New Model from byte array, as in dbc Mesh file</summary>
        /// <param name="Id">Model Id Number</param>
        /// <param name="Amount">Amount of parts</param>
        /// <param name="buffer">Data read from binary dbc file</param>
        /// <param name="materials">Array of material Id names (all parts), 
        /// in Material.ini</param>
        public Model(UInt32 Id, UInt32 Amount, Byte[] buffer, String[] materials)
        {
            // Mesh dbc binary files always uses fixed length structure
            if (buffer.Length < 16 * Amount)
            {
                new Model();
            }
            else
            {
                Data = new List<ModelPart>();
                if (materials.Length < 16)
                {
                    for (int i = 0; i < Amount; i++)
                        materials[i] = "Material" + buffer[i * 16 + 15].ToString();
                }
                Byte[] b = new Byte[16];
                for (int i = 0; i < Amount; i++)
                {
                    Buffer.BlockCopy(buffer, 1 * 16, b, 0, 16);
                    Data.Add(new ModelPart(b, materials[i]));
                }
            }
        }

        /// <summary>New Model from multiline string</summary>
        /// <param name="str">Lines separated by \r\n, like with this.ToString()</param>
        /// <exception cref="ArgumentOutOfRangeException">if Amount cannot fit 
        /// in an unsigned integer (should be 4 or less) (inherited)</exception>
        /// <exception cref="ArgumentOutOfRangeException">if not enough
        /// strings in the array for all Model Parts (inherited)</exception>
        public Model(String str)
        {
            String[] st = Common.SplitLines(str);
            new Model(st);
        }

        /// <summary>New Model from string array, as in ini files
        /// <remarks>Does not check if correct format</remarks></summary>
        /// <param name="str">Array of strings, one for each line</param>
        /// <exception cref="ArgumentOutOfRangeException">if Amount cannot fit 
        /// in an unsigned integer (should be 4 or less)</exception>
        /// <exception cref="ArgumentOutOfRangeException">if not enough
        /// strings in the array for all Model Parts</exception>
        public Model(String[] str)
        {
            // Check here if there are comments
            int lineNumber = 0;
            PARSE_COMMENTS_STATE commentsState = PARSE_COMMENTS_STATE.NotCommentLine;
            while (lineNumber < str.Length
                && (commentsState = IniCommon.ParseLineComments(str[lineNumber], commentsState)) >= 0)
            {
                // Store comments
                if (!String.IsNullOrEmpty(Comments))
                    Comments += "\r\n" + str[lineNumber];
                else
                    Comments = str[lineNumber];
                lineNumber++;
            }
            Id = UInt32.Parse(str[lineNumber].Substring(1, str[0].Length - 2));
            lineNumber++;
            Amount = UInt32.Parse(str[lineNumber].Split('=')[1]);
            lineNumber++;
            if (Amount > 0x0FFFFFFF)
                throw new ArgumentOutOfRangeException("Amount", "cannot fit in a signed integer, while should be 4 or lower");
            // Check if remaining lines are enough for the amount of parts
            if (str.Length - lineNumber < Amount * 7)
                throw new ArgumentOutOfRangeException("str", "there are too few lines to read Model Parts, expected " + (7*Amount).ToString() + ", while remaining " + (str.Length - lineNumber).ToString());
            Data = new List<ModelPart>();
            for (int i = 0; i < Amount; i++)
            {
                Data.Add(new ModelPart(str[lineNumber]));
                lineNumber++;
            }
        }
        #endregion

        #region PublicMethods
        /// <summary>Add comments for this Model</summary>
        /// <param name="s">String like in ini comment lines</param>
        public void AddComments(String s)
        {
            if (String.IsNullOrEmpty(Comments))
                Comments = s;
            else
                Comments += "\r\n" + s;
        }

        /// <summary>Parse to array of bytes</summary>
        /// <returns>Return an array of bytes ready to save to a dbc file</returns>
        public Byte[] ToBytes()
        {
            Byte[] b = new Byte[8 + 16 * Data.Count];
            Buffer.BlockCopy(BitConverter.GetBytes(Id), 0, b, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Amount), 0, b, 4, 4);
            int pos = 8;
            foreach (ModelPart mp in Data)
            {
                Buffer.BlockCopy(mp.ToBytes(), 0, b, pos, 16);
                pos += 16;
            }
            return b;
        }

        /// <summary>Parse to String</summary>
        /// <returns>Return a string ready to save to an ini file</returns>
        public override String ToString()
        {
            String s = "";
            if (!String.IsNullOrEmpty(Comments))
                s += Comments + "\r\n";
            s += "[" + Id.ToString() + "]" + "\r\n";
            s += "Part=" + Amount.ToString() + "\r\n";
            for (int i = 0; i < Amount; i++)
                s += Data[i].ToString() + "\r\n";
            return s;
        }
        #endregion
    }
}

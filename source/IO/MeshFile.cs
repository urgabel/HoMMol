using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HoMMol_core.Graphics;
using HoMMol_core.Ini;

namespace HoMMol_core.IO
{
    /// <summary>Handles Models collections, in dbc Mesh files
    /// <para>Files using this class (either in dbc or ini format):</para>
    /// <para> armet.dbc  armor.dbc  Earrings.dbc  hair.dbc   head.dbc</para>
    /// <para> misc.dbc   mount.dbc  necklace.dbc  skirt.dbc  weapon.dbc</para>
    /// <remarks>Only handles data and a FileStream, use dbcFile to 
    /// handle the file. Also need to receive materials name from 
    /// Material.ini, when reading binary files</remarks></summary>
    class MeshFile
    {
        #region Constants
        /// <summary>Magyc Type Header in dbc binary files, in String format</summary>
        public const String MAGIC_TYPE_TXT = "MESH";

        /// <summary>Magyc Type Header in dbc binary files, in UInt32 format</summary>
        public const UInt32 MAGIC_TYPE_BIN = 0x4853454D; // "MESH"
        #endregion

        #region Properties
        /// <summary>Stores all Models</summary>
        public Dictionary<UInt32, Model> Data = null;

        /// <summary>Amount of Models</summary>
        public UInt32 Amount = 0;

        /// <summary>Stores initial comments, if any
        /// <remarks>Reading an ini file is hard to distinguish between 
        /// initial comments and first model's comments</remarks></summary>
        public String InitialComments = null;
        #endregion

        #region Constructor
        /// <summary>Default Constructor: New MeshFile empty instance
        /// <remarks>Does not handle the file, just the data</remarks></summary>
        public MeshFile()
        {
            Data = new Dictionary<UInt32,Model>();
        }
        #endregion

        #region PublicMethods
        /// <summary>Add initial comments for this collection</summary>
        /// <param name="s">String like in ini comment lines</param>
        public void AddComments(String s)
        {
            if (String.IsNullOrEmpty(InitialComments))
                InitialComments = s;
            else
                InitialComments += "\r\n" + s;
        }

        /// <summary>Load a Mesh file from a filestream
        /// <remarks>Use first the constructor</remarks>
        /// </summary>
        /// <param name="amount">Amount of Models</param>
        /// <param name="fs">FileStream to read from</param>
        /// <param name="mode">BIN or TXT, for binary dbc or text ini</param>
        /// <returns>False if error loading the data</returns>
        /// <exception cref="Exception">If cannot find the header</exception>
        public Boolean Load(UInt32 amount, FileStream fs, dbcFile.Mode mode)
        {
            Boolean result = false;
            if (mode == dbcFile.Mode.TXT)
                if (LoadTxt(amount, fs) != 0) result = true;
            else
                if (LoadBin(amount, fs) != 0) result = true;
            return result;
        }

        /// <summary>Save a Mesh file to a filestream</summary>
        /// <param name="fs">FileStream to write to</param>
        /// <param name="mode">BIN or TXT, for binary dbc or text ini</param>
        /// <returns>False if error saving the data</returns>
        public Boolean Save(FileStream fs, dbcFile.Mode mode)
        {
            Boolean result = false;
            if (mode == dbcFile.Mode.TXT)
                if (SaveTxt(fs) != 0) result = true;
            else
                if (SaveBin(fs) != 0) result = true;
            return result;
        }

        /// <summary>Load a Mesh file from a text filestream
        /// <remarks>Use first the constructor</remarks>
        /// </summary>
        /// <param name="amount">Amount of Models</param>
        /// <param name="fs">FileStream to read from</param>
        /// <returns>Amount of models read</returns>
        public UInt32 LoadTxt(UInt32 amount, FileStream fs)
        {
            UInt32 modelsRead = 0;      // Only for debug
            Int32 lineCount = 0;        // Only for debug
            String commentLines = null;
            String line = null;
            List<String> comments = new List<String>();
            List<String> currentModel = new List<String>();
            PARSE_COMMENTS_STATE commentsState = PARSE_COMMENTS_STATE.NotCommentLine;
            // Simplified Chinese encoding
            using (StreamReader sr = new StreamReader(fs, Common.enc, false)) lock (Data)
            {
                while ((line = sr.ReadLine()) != null)
                {
                    lineCount++;
                    // Check if comments or empty line
                    if ((commentsState = IniCommon.ParseLineComments(line, commentsState)) >= 0)
                    {
                        // Add to comments if not inside a model (or skip)
                        if (currentModel.Count == 0)
                        {
                            comments.Add(line);
                        }
                    }
                    // Check if starting to read a new model
                    else if (line.StartsWith("["))
                    {
                        // If we already have a model read
                        if (currentModel.Count != 0)
                        {
                            // Join with "\r\n" all comment lines until 
                            //   last not empty, as last empty was just a
                            //   separator between models
                            // TODO: improve this method
                            commentLines = Common.ConcatenateAllUntilLastNotEmpty(comments);
                            // current model + comments -> add to collection
                            Model m = new Model(currentModel.ToArray());
                            m.AddComments(commentLines);
                            Data.Add(m.Id, m);
                            modelsRead++;
                            // reset comments
                            comments.Clear();
                        }
                        // new model
                        currentModel.Clear();
                        // add line to model
                        currentModel.Add(line);
                    }
                    else
                    {
                        // Add data to the current model
                        currentModel.Add(line);
                    }
                }
                // EOF: Check if pending to save current model
                if (currentModel.Count != 0)
                {
                    Model m = new Model(currentModel.ToArray());
                    m.AddComments(commentLines);
                    Data.Add(m.Id, m);
                    modelsRead++;
                }
            }
            return modelsRead;
        }

        /// <summary>Load a Mesh file from a binary filestream
        /// <remarks>Use first the constructor. After data read, 
        /// it is needed to read materials names from Material.ini</remarks>
        /// </summary>
        /// <param name="amount">Amount of Models</param>
        /// <param name="fs">FileStream to read from</param>
        /// <returns>Amount of bytes read</returns>
        public UInt32 LoadBin(UInt32 amount, FileStream fs)
        {
            UInt32 bytesRead = 0;
            fs.Seek(0, SeekOrigin.Begin);
            using (BinaryReader br = new BinaryReader(fs)) lock (Data)
            {
                UInt32 id, partsAmount;
                Byte[] head = new Byte[8];
                // Skip Magic Header and amount as it is already known
                br.Read(head, 0, 8);
                bytesRead += 8;
                // Read every Model
                for (int i = 0; i < Amount; i++)
                {
                    // Read ID and Model Parts amount
                    br.Read(head, 0, 8);
                    bytesRead += 8;
                    id = BitConverter.ToUInt32(head, 0);
                    partsAmount = BitConverter.ToUInt32(head, 4);
                    // Read every part in this Model
                    Byte[] modelParts = new Byte[16 * partsAmount];
                    br.Read(modelParts, 0, modelParts.Length);
                    // Add the model to the collection
                    Data.Add(id, new Model(id, partsAmount, modelParts));
                }
            }
            return bytesRead;
        }

        /// <summary>Save a Mesh file to a text filestream</summary>
        /// <param name="fs">FileStream to write to</param>
        /// <returns>Amount of models written</returns>
        public UInt32 SaveTxt(FileStream fs)
        {
            UInt32 modelsWritten = 0;    // Only for debug
            // Simplified Chinese encoding
            using (StreamWriter sw = new StreamWriter(fs, Common.enc)) lock (Data)
            {
                if (!String.IsNullOrEmpty(InitialComments))
                    sw.WriteLine(InitialComments);
                foreach (KeyValuePair<UInt32, Model> m in Data)
                {
                    sw.WriteLine(m.Value.ToString());
                    sw.WriteLine();
                    modelsWritten++;
                }
                // Add an extra blank line at the end of the file
                sw.WriteLine();
            }
            return modelsWritten;
        }

        /// <summary>Save a Mesh file to a binary filestream</summary>
        /// <remarks> Before save data, it is needed to read Material.ini
        /// and provide the material order number in the list for this
        /// material name</remarks>
        /// <param name="fs">FileStream to write to</param>
        /// <returns>Amount of bytes written</returns>
        public UInt32 SaveBin(FileStream fs)
        {
            UInt32 bytesWritten = 0;
            using (BinaryWriter bw = new BinaryWriter(fs)) lock (Data)
            {
                bw.Write(MAGIC_TYPE_BIN);
                bytesWritten += 4;
                bw.Write(Amount);
                bytesWritten += 4;
                foreach (KeyValuePair<UInt32, Model> m in Data)
                {
                    bw.Write(m.Key);
                    bytesWritten += 4;
                    bw.Write(m.Value.Amount);
                    bytesWritten += 4;
                    bw.Write(m.Value.ToBytes());
                    bytesWritten += 16 * m.Value.Amount;
                }
            }
            return bytesWritten;
        }
        #endregion
    }
}

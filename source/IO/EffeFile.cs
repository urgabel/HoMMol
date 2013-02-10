using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HoMMol_core.Graphics;
using HoMMol_core.Ini;

namespace HoMMol_core.IO
{
    /// <summary>Handles Effects collections, in dbc Effe files
    /// <para>3D Effect animations. Each animation has a sequence of effects</para>
    /// <para>Files using this class (either in dbc or ini format):</para>
    /// <para> 3DEffect.dbc</para>
    /// <remarks>Only handles data and a FileStream, use dbcFile to 
    /// handle the file.</remarks></summary>
    class EffeFile
    {
        #region Constants
        /// <summary>Magyc Type Header in dbc binary files, in String format</summary>
        public const String MAGIC_TYPE_TXT = "EFFE";

        /// <summary>Magyc Type Header in dbc binary files, in UInt32 format</summary>
        public const UInt32 MAGIC_TYPE_BIN = 0x45464645; // "EFFE"
        #endregion

        #region Properties
        /// <summary>Stores all Effects</summary>
        public Dictionary<String, Effect> Data = null;

        /// <summary>Amount of Effects</summary>
        public UInt32 Amount = 0;

        /// <summary>Stores initial comments, if any
        /// <remarks>Reading an ini file is hard to distinguish between 
        /// initial comments and first effect's comments</remarks></summary>
        public String InitialComments = null;
        #endregion

        #region Constructor
        /// <summary>Default Constructor: New EffeFile empty instance
        /// <remarks>Does not handle the file, just the data</remarks></summary>
        public EffeFile()
        {
            Data = new Dictionary<String, Effect>();
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

        /// <summary>Load an Effe file from a filestream
        /// <remarks>Use first the constructor</remarks>
        /// </summary>
        /// <param name="amount">Amount of Effects</param>
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

        /// <summary>Save an Effect file to a filestream</summary>
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

        /// <summary>Load an Effe file from a text filestream
        /// <remarks>Use first the constructor</remarks>
        /// </summary>
        /// <param name="amount">Amount of Effects</param>
        /// <param name="fs">FileStream to read from</param>
        /// <returns>Amount of models read</returns>
        public UInt32 LoadTxt(UInt32 amount, FileStream fs)
        {
            UInt32 effectsRead = 0;      // Only for debug
            Int32 lineCount = 0;        // Only for debug
            String commentLines = null;
            String line = null;
            List<String> comments = new List<String>();
            List<String> currentEffect = new List<String>();
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
                            // Add to comments if not inside an effect (or skip)
                            if (currentEffect.Count == 0)
                            {
                                comments.Add(line);
                            }
                        }
                        // Check if starting to read a new effect
                        else if (line.StartsWith("["))
                        {
                            // If we already have an effect read
                            if (currentEffect.Count != 0)
                            {
                                // Join with "\r\n" all comment lines until 
                                //   last not empty, as last empty was just a
                                //   separator between effects
                                // TODO: improve this method
                                commentLines = Common.ConcatenateAllUntilLastNotEmpty(comments);
                                // current effect + comments -> add to collection
                                Effect e = new Effect(currentEffect.ToArray());
                                e.AddComments(commentLines);
                                Data.Add(e.AniTitle, e);
                                effectsRead++;
                                // reset comments
                                comments.Clear();
                            }
                            // new effect
                            currentEffect.Clear();
                            // add line to effect
                            currentEffect.Add(line);
                        }
                        else
                        {
                            // Add data to the current effect
                            currentEffect.Add(line);
                        }
                    }
                    // EOF: Check if pending to save current effect
                    if (currentEffect.Count != 0)
                    {
                        Effect e = new Effect(currentEffect.ToArray());
                        e.AddComments(commentLines);
                        Data.Add(e.AniTitle, e);
                        effectsRead++;
                    }
                }
            return effectsRead;
        }

        /// <summary>Load an Effe file from a binary filestream
        /// <remarks>Use first the constructor.</remarks>
        /// </summary>
        /// <param name="amount">Amount of Effects</param>
        /// <param name="fs">FileStream to read from</param>
        /// <returns>Amount of bytes read</returns>
        public UInt32 LoadBin(UInt32 amount, FileStream fs)
        {
            UInt32 bytesRead = 0;
            fs.Seek(0, SeekOrigin.Begin);
            using (BinaryReader br = new BinaryReader(fs)) lock (Data)
            {
                UInt16 frameAmount;
                Byte[] b = new Byte[34];
                // Skip Magic Header and amount as it must be already known
                br.Read(b, 0, 8);
                bytesRead += 8;
                // Read every Effect
                for (int i = 0; i < Amount; i++)
                {
                    // Read AniTitle and Effect Parts amount
                    br.Read(b, 0, 34);
                    bytesRead += 34;
                    frameAmount = BitConverter.ToUInt16(b, 32);
                    Byte[] fullEffect = new Byte[16 * frameAmount + 34];
                    Buffer.BlockCopy(b, 0, fullEffect, 0, 34);
                    // Read all frames in this Effect
                    br.Read(fullEffect, 34, fullEffect.Length);
                    bytesRead += (uint)16 * frameAmount;
                    // Add the effect to the collection
                    Effect e = new Effect(fullEffect);
                    Data.Add(e.AniTitle, e);
                }
            }
            return bytesRead;
        }

        /// <summary>Save an Effe file to a text filestream</summary>
        /// <param name="fs">FileStream to write to</param>
        /// <returns>Amount of models written</returns>
        public UInt32 SaveTxt(FileStream fs)
        {
            UInt32 effectsWritten = 0;    // Only for debug
            // Simplified Chinese encoding
            using (StreamWriter sw = new StreamWriter(fs, Common.enc)) lock (Data)
                {
                    if (!String.IsNullOrEmpty(InitialComments))
                        sw.WriteLine(InitialComments);
                    foreach (KeyValuePair<String, Effect> e in Data)
                    {
                        sw.WriteLine(e.Value.ToString());
                        sw.WriteLine();
                        effectsWritten++;
                    }
                    // Add an extra blank line at the end of the file
                    sw.WriteLine();
                }
            return effectsWritten;
        }

        /// <summary>Save an Effe file to a binary filestream</summary>
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
                    foreach (KeyValuePair<String, Effect> e in Data)
                    {
                        bw.Write(e.Value.ToBytes());
                        bytesWritten += (uint)16 * e.Value.Amount + 34;
                    }
                }
            return bytesWritten;
        }
        #endregion
    }
}

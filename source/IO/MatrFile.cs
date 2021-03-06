﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HoMMol_core.Graphics;
using HoMMol_core.Ini;

namespace HoMMol_core.IO
{
    /// <summary>Materials list collection
    /// <remarks>Only handles data and a FileStream, 
    /// use dbcFile to handle the file</remarks></summary>
    class MatrFile
    {
        #region Constants
        /// <summary>Magyc Type Header in dbc binary files, in String format</summary>
        public const String MAGIC_TYPE_TXT = "MATR";

        /// <summary>Magyc Type Header in dbc binary files, in UInt32 format</summary>
        public const UInt32 MAGIC_TYPE_BIN = 0x5254414D; // "MATR"
        #endregion

        #region Properties
        /// <summary>Stores all entries</summary>
        public Dictionary<String, Matr> Data = null;

        /// <summary>Amount of data entries</summary>
        public UInt32 Amount = 0;

        /// <summary>Stores initial comments, if any</summary>
        public String InitialComments = null;
        #endregion

        #region Constructor
        /// <summary>Default Constructor: New MatrFile empty instance
        /// <remarks>Does not handle the file, just the data</remarks></summary>
        public MatrFile()
        {
            Data = new Dictionary<String, Matr>();
        }
        #endregion

        #region PublicMethods
        /// <summary>Load a Matr file from a filestream
        /// <remarks>Use first the constructor</remarks>
        /// </summary>
        /// <param name="amount">Amount of data entries</param>
        /// <param name="fs">FileStream to read from</param>
        /// <param name="mode">BIN or TXT, for binary dbc or text ini</param>
        /// <returns>False if error loading the data</returns>
        /// <exception cref="Exception">If cannot find the header</exception>
        public Boolean Load(UInt32 amount, FileStream fs, dbcFile.Mode mode)
        {
            Boolean result = false;
            fs.Seek(0, SeekOrigin.Begin);
            switch (mode)
            {
                case dbcFile.Mode.TXT:
                    {
                        Boolean headerIsSet = false;
                        PARSE_COMMENTS_STATE commentsState = PARSE_COMMENTS_STATE.NotCommentLine;
                        Int32 lineCount = 0;
                        // Simplified Chinese encoding
                        using (StreamReader sr = new StreamReader(fs, Common.enc, false)) lock (Data)
                        {
                            String line = null;
                            Boolean hasComments = false;
                            String Comments = String.Empty;
                            while ((line = sr.ReadLine()) != null)
                            {
                                lineCount++;
                                // Check if comments
                                if ((commentsState = IniCommon.ParseLineComments(line, commentsState)) >= 0)
                                {
                                    // Store comments to add after created a new Matr
                                    if (hasComments)
                                        Comments += "\r\n" + line;
                                    else
                                    {
                                        hasComments = true;
                                        Comments = line;
                                    }
                                }
                                else
                                {
                                    line = line.Trim();     // Remove spaces from start and end of line
                                    if (!headerIsSet)
                                    {
                                        if (line.StartsWith("Material=")
                                            && UInt32.TryParse(line.Substring(9), out Amount))
                                            headerIsSet = true;
                                        else
                                            throw new Exception("Cannot find the header; expected: Material=##");
                                    }
                                    else try
                                    {
                                        Matr m = new Matr(line);
                                        if (hasComments) m.AddComments(Comments);
                                        Data.Add(m.IdName, m);
                                        hasComments = false;
                                        Comments = String.Empty;
                                    }
                                    catch
                                    {
                                        // Skip lines with incorrect format
                                        //Trace.Warning("Skiped line " + lineCount + " with incorrect format.");
                                    }
                                }
                            }
                        }
                        break;
                    }
                case dbcFile.Mode.BIN:
                default:
                    {
                        using (BinaryReader br = new BinaryReader(fs)) lock (Data)
                        {
                            UInt32 bytesRead = 0;   // Only for debug
                            Byte[] buffer = new Byte[52];
                            br.Read(buffer, 0, 8);  // Skip Magic Header and amount
                                                    //   as it is already known
                            bytesRead += 8;
                            // Read all data, 52 bytes each
                            for (int i = 0; i < amount; i++)
                            {
                                br.Read(buffer, 0, 52);
                                Matr m = new Matr(buffer);
                                Data.Add(m.IdName, m);
                            }
                        }
                        break;
                    }
            }
            result = true;
            return result;
        }

        /// <summary>Save a Matr file to a filestream</summary>
        /// <param name="fs">FileStream to write to</param>
        /// <param name="mode">BIN or TXT, for binary dbc or text ini</param>
        /// <returns>False if error saving the data</returns>
        public Boolean Save(FileStream fs, dbcFile.Mode mode)
        {
            Boolean result = true;
            fs.Seek(0, SeekOrigin.Begin);
            switch (mode)
            {
                case dbcFile.Mode.TXT:
                    {
                        UInt32 lineCount = 0;    // Only for debug
                        // Simplified Chinese encoding
                        using (StreamWriter sw = new StreamWriter(fs, Common.enc)) lock (Data)
                        {
                            sw.WriteLine("Material=" + Amount);
                            lineCount++;
                            foreach (KeyValuePair<String, Matr> m in Data)
                            {
                                sw.WriteLine(m.ToString());
                                lineCount++;
                            }
                        }
                        break;
                    }
                case dbcFile.Mode.BIN:
                default:
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs)) lock (Data)
                        {
                            UInt32 bytesWritten = 0;   // Only for debug
                            bw.Write(MAGIC_TYPE_BIN);
                            bytesWritten += 4;
                            bw.Write(Amount);
                            bytesWritten += 4;
                            foreach (KeyValuePair<String, Matr> m in Data)
                            {
                                bw.Write(m.Value.ToBytes());
                                bytesWritten += 52;
                            }
                        }
                        break;
                    }
            }
            return result;
        }
        #endregion
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMMol_core.IO
{
    /// <summary>Dbc files are DataBase files stored in Client side
    ///   It can be: Mesh, Rsdb (32 or 64 bits), Effe, Simo, 
    ///   SimX, Emoi, Matr, Ropt
    /// Mesh -> 3D Wire
    /// Rsdb -> Resource-Strings DataBase
    /// Effe -> 3D Effects
    /// Simo -> 3D Simple Object
    /// SimX -> 3D Simple Object eXtension
    /// Emoi -> Emotion Icon
    /// Matr -> Material
    /// Ropt -> Role Parts
    /// <remarks>//TODO: Add new constructors to create and edit files</remarks>
    /// </summary>
    class dbcFile
    {
        #region Constants
        /// <summary>Mode to handle dbc files, in read or write operations</summary>
        public enum Mode { BIN, TXT }

        /// <summary>Magyc Type Header in dbc binary files, in String format</summary>
        public static String[] DBC_HEADERS_TXT = 
            {
                "MESH", "RSDB", "EFFE", "SIMO", 
                "SIMX", "EMOI", "MATR", "ROPT"
            };

        /// <summary>Magyc Type Header in dbc binary files, in UInt32 format</summary>
        public static UInt32[] DBC_HEADERS_BIN =
            {
                0x4853454D,     // "MESH"
                0x42445352,     // "RSDB"
                0x45464645,     // "EFFE"
                0x4F4D4953,     // "SIMO"
                0x584D4953,     // "SIMX"
                0x494F4D45,     // "EMOI"
                0x5254414D,     // "MATR"
                0x54504F52      // "ROPT"
            };

        #endregion

        #region Properties
        /// <summary>Amount of data entries</summary>
        public UInt32 Amount = 0;

        private UInt32 _DbcMagicType = 0;   // Dbc Magic type in binary form
        private Boolean _IsBinary = true;   // True if Dbc is in binary form
        private String _DbcType = null;     // Type of Dbc (MATR, MESH...)
        private String _FileName = null;    // Dbc Filename
        private FileStream _fs = null;      // Source Filestream
        private long _fs_size = 0;          // Size of the source
        private FileStream _dfs = null;     // Destination Filestream
        private StreamReader _sr = null;    // For text read operations
        private StreamWriter _sw = null;    // For text write operations
        private MatrFile _Matr = null;      // Stores all data
        #endregion

        #region Constructor
        //TODO: Add new constructors to create and edit files
        /// <summary>Open a dbc file to read data</summary>
        /// <param name="fileName">Full path to the file</param>
        /// <exception cref="Exception">If unsuported type of DBC file</exception>
        /// <exception cref="FileNotFoundException">If the file does not exists</exception>
        public dbcFile(String fileName)
        {
            if (File.Exists(fileName))
            {
                _fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                // Check if empty file, if txt or bin, and Dbc type
                ReadHeader();
                if (_DbcType == "Unsupported")
                {
                    throw new Exception("Unsupported type of DBC file.");
                }
                _FileName = fileName;
                // Initialize the data store
                InitDataStore();        // Can throw NotImplementedException 
                                        //    if cannot handle dbc type 
                // Read entries
                if (_IsBinary) Load(Mode.BIN);
                else Load(Mode.TXT);
            }
            else
            {
                // If the file does not exists
                throw new FileNotFoundException(fileName + " does not exists");
            }
        }
        
        #endregion

        #region PublicMethods
        /// <summary>Close any opened file
        /// <remarks>Keeps the data</remarks>
        /// </summary>
        public void Close()
        {
            if (_sw != null)
            {
                _sw.Flush();
                _sw.Close();
                _sw.Dispose();
                _sw = null;
            }
            if (_sr != null)
            {
                _sr.Close();
                _sr.Dispose();
                _sr = null;
            }
            if (_dfs != null)
            {
                _dfs.Flush();
                _dfs.Close();
                _dfs.Dispose();
                _dfs = null;
            }
            if (_fs != null)
            {
                _fs.Close();
                _fs.Dispose();
                _fs_size = 0;
                _fs = null;
            }
        }
        
        /// <summary>Check if a dbc instance is in binary format
        /// <remarks>Use first the constructor</remarks>
        /// </summary>
        /// <returns>False if text format (.ini)</returns>
        public Boolean IsBinary()
        {
            return _IsBinary;
        }

        /// <summary>Check if a dbc instance has no data
        /// <remarks>Use first the constructor</remarks>
        /// </summary>
        /// <returns>False if amount not 0</returns>
        public Boolean IsEmpty()
        {
            return (Amount == 0);
        }

        /// <summary>Check the type of a dbc instance
        /// <remarks>Use first the constructor</remarks>
        /// </summary>
        /// <returns>A string with Magic Type</returns>
        public String DbcType()
        {
            return _DbcType;
        }

        /// <summary>Load a dbc file
        /// <remarks>Use first the constructor</remarks>
        /// </summary>
        /// <param name="m">Mode can be Mode.BIN or MODE.TXT</param>
        /// <returns>False if error loading the file</returns>
        // TODO: Update on supported types
        //       Now: MATR
        public Boolean Load(Mode m)
        {
            String result = "";
            long pos = 0;       // For debug only
            try
            {
                Byte[] buffer = new Byte[16];
                // Start after Magic Type Binary Header and Amount
                _fs.Seek(8, SeekOrigin.Begin);
                pos = 8;
                switch (_DbcType)
                {
                    case "MATR":
                        {
                            if (! _Matr.Load(Amount, _fs, m))
                                throw new Exception("Error loading data from  " + _FileName);
                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException("Cannot handle " + _DbcType);
                        }
                }
            }
            catch (Exception e)
            {
                pos = _fs.Position;
                if ((result == "")) result = "The file cannot be read.";
                else result = result + "\nError at pos " + pos.ToString();
                result = result + "\n" + e.Message;
                return false;
            }
            return true;
        }

        /// <summary>Save to a dbc file</summary>
        /// <param name="fileName">Full path to the file</param>
        /// <param name="m">Mode can be Mode.BIN or MODE.TXT</param>
        /// <returns>False if error saving the file</returns>
        // TODO: Update on supported types
        //       Now: MATR
        public Boolean Save(String fileName, Mode m)
        {
            String result = "";     // For debug
            long pos = 0;
            _dfs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            try
            {
                switch (_DbcType)
                {
                    case "MATR":
                        {
                            if (! _Matr.Save(_dfs, m))
                                throw new Exception("Error saving data to  " + _FileName);
                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException("Cannot handle " + _DbcType);
                        }
                }
            }
            catch (Exception e)
            {
                pos = _fs.Position;
                if ((result == "")) result = "The file cannot be written.";
                else result = result + "\nError at pos " + pos.ToString();
                result = result + "\n" + e.Message;
                return false;
            }
            return true;
        }
        #endregion

        #region PrivateMethods
        // Detect if dbc file is in binary or text format
        // Check filename extension
        // It could be safer by reading first bytes of data
        private Boolean CheckIfBinary()
        {
            String ext = Path.GetExtension(_FileName);
            if (ext == ".ini" | ext == ".txt")
                return false;
            return true;
        }

        // Detect type of binary dbc file
        // Read first bytes and compare to expected data
        // TODO: Update on supported types
        //       Now: MATR
        private String CheckDbcBinType()
        {
            // Read the Magic Type Header and get the Amount of data
            Byte[] buffer = new Byte[8];
            _fs.Seek(0, SeekOrigin.Begin);
            _fs.Read(buffer, 0, 8);
            
            // Read DBC Header
            _DbcMagicType = BitConverter.ToUInt32(buffer, 0);
            if (!DBC_HEADERS_BIN.Contains(_DbcMagicType)) 
                _DbcType = "Unsupported";
            else 
                _DbcType = Common.enc.GetString(BitConverter.GetBytes(_DbcMagicType));
            
            // Read Amount
            Amount = BitConverter.ToUInt32(buffer, 4);

            // For RSDB binary mode, detect if it is 32 or 64 bits
            if (_DbcType == "RSDB")
            {
                if (CheckId64bits()) _DbcType += "64";
                else _DbcType += "32";
            }

            // TODO: Remove next line when all types implemented
            if (_DbcType != "MATR") _DbcType = "Unsupported";
            
            return _DbcType;
        }

        // Detect type of text dbc file
        // It would be a lot easier to check the filename, 
        //    but safer not to do so
        // Read first lines and compare to expected data
        // TODO: Revise if there are data in same line after a long Comment
        //        "[...] */DataHere".
        // TODO: Handle Bad Formats
        // TODO: Handle SimX, surely like Mesh
        // TODO: Update on supported types
        //       Now: MATR
        private String CheckDbcTxtType()
        {
            // Text dbc files includes amount only for Matr type
            // Skip initial comments and empty lines
            // Read line
            // Check first char

            // If "/" -> Comment (Check if "//" or "/*")
            // If ";" -> Comment
            // If "[" -> Effe, Ropt, Simo, Simx, Mesh
            // If "M" -> Matr (Check if Material=)
            // If number -> Emoi, Rsdb
            // Otherwise Unsupported

            _fs.Seek(0, SeekOrigin.Begin);
            _sr = new StreamReader(_fs, Common.enc);
            Int32 lineCount = 0;
            long testNumber = 0;
            String line;
            String firstChar;
            Boolean continueReading = true;
            Boolean longComment = false;

            // Read Header
            while (continueReading && (line = _sr.ReadLine()) != null)
            {
                lineCount++;
                if (!String.IsNullOrEmpty(line.Trim()))     // Skip empty lines
                {
                    if (longComment)
                    {
                        // Check here if finish long comment "*/"
                        // If there are data in same line after "*/" it
                        //   should be an unsupported format
                        if (line.Length > 1 && line.Substring(line.Length - 2) == "*/")
                        {
                            longComment = false;
                        }
                    }
                    else
                    {
                        firstChar = line.Substring(0, 1);
                        switch (firstChar)
                        {
                            case "[":
                                {       // Effe, Ropt, Simo, Simx, Mesh
                                    if (line.Length < 3 
                                        || line.Substring(line.Length - 1) != "]" 
                                        || !long.TryParse(line.Substring(1, line.Length - 2), out testNumber))
                                    {
                                        _DbcType = "BadFormatBracket";
                                    }
                                    else
                                    {
                                        String line2;
                                        if ((line2 = _sr.ReadLine()) != null)
                                        {
                                            if (line2.Length > 5 && line2.Substring(0, 5) == "Part=")
                                            {
                                                // Check if Mesh or Simx
                                                _DbcType = "MESH";
                                            }
                                            else if (line2.Length > 6 && line2.Substring(0, 6) == "Count=")
                                            {
                                                _DbcType = "ROPT";
                                            }
                                            else if (line2.Length > 7 && line2.Substring(0, 7) == "Amount=")
                                            {
                                                _DbcType = "EFFE";
                                            }
                                            else if (line2.Length > 11 && line2.Substring(0, 11) == "PartAmount=")
                                            {
                                                _DbcType = "SIMO";
                                            }
                                            else
                                            {
                                                _DbcType = "Unsupported";
                                            }
                                        }
                                        else
                                        {
                                            // End of file
                                            _DbcType = "BadFormatBracketEOF";
                                        }
                                        _DbcType = "";
                                    }
                                    continueReading = false;
                                    break;
                                }
                            case "M":
                                {       // Matr (Check if "Material=")
                                    if (line.Length > 9 && line.Substring(0, 9) == "Material=")
                                    {
                                        // Check amount;
                                        if (UInt32.TryParse(line.Split('=')[1], out Amount))
                                        {
                                            _DbcType = "MATR";
                                        }
                                        else
                                        {
                                            _DbcType = "BadFormatMatr";
                                        }
                                    }
                                    else
                                    {
                                        _DbcType = "Unsupported";
                                    }
                                    continueReading = false;
                                    break;
                                }
                            case "0":
                            case "1":
                            case "2":
                            case "3":
                            case "4":
                            case "5":
                            case "6":
                            case "7":
                            case "8":
                            case "9":
                                {       // Emoi, Rsdb
                                    if (line.Contains('='))
                                    {
                                        // Check if Rsdb
                                        // Some Rsdb ini files, like ActionSound.ini uses nnn.nnnn.nnn
                                        if (long.TryParse(line.Split('=')[0].Replace(".", String.Empty), out testNumber))
                                        {
                                            _DbcType = "RSDB";
                                        }
                                        else
                                        {
                                            _DbcType = "BadFormatRsdb";
                                        }
                                    }
                                    else
                                    {
                                        // Check if Emoi
                                        if (line.Contains(' '))
                                        {
                                            String[] s = line.Split(' ');
                                            if (s.Length == 2 && long.TryParse(s[0], out testNumber))
                                            {
                                                _DbcType = "EMOI";
                                            }
                                            else
                                            {
                                                _DbcType = "BadFormatEmoi";
                                            }
                                        }
                                        else
                                        {
                                            _DbcType = "BadFormatNumber";
                                        }
                                    }
                                    continueReading = false;
                                    break;
                                }
                            case ";":
                                {       // Comment
                                    // Nothing to do, skip comment and continue reading lines
                                    break;
                                }
                            case "/":
                                {       // Comment. Check if "//" or "/*"
                                    if (line.Length == 1)
                                    {
                                        _DbcType = "Unsupported";
                                        continueReading = false;
                                    }
                                    else
                                    {
                                        firstChar = line.Substring(0, 2);
                                        if (firstChar == "//")
                                        {
                                            // Nothing to do, skip comment and continue reading lines
                                        }
                                        else if (firstChar == "/*")
                                        {
                                            // If there are data in same line after "*/" it
                                            //   should be an unsupported format
                                            // Check if line ends with "*/"
                                            if (line.Substring(line.Length - 2) == "*/")
                                            {
                                                // Nothing to do, skip comment and continue reading lines
                                            }
                                            else
                                            {
                                                longComment = true;
                                            }
                                        }
                                    }
                                    break;
                                }
                            default:
                                {       // Unsupported
                                    _DbcType = "Unsupported";
                                    continueReading = false;
                                    break;
                                }
                        }
                    }
                }
            }
            
            // Check if end of file without a type
            if (_DbcType == null) _DbcType = "Undefined";

            // Check if Bad Format
            if (_DbcType.Length > 8 && _DbcType.Substring(0, 9) == "BadFormat")
            {
                // TODO: Handle here Bad Formats
                _DbcType = "Unsupported";
            }

            // If RSDB, check if 32 or 64 bits Ids
            if (_DbcType == "RSDB")
            {
                if (CheckId64bits()) _DbcType += "64";
                else _DbcType += "32";
            }

            // TODO: Remove next line when all types implemented
            if (_DbcType != "MATR") _DbcType = "Unsupported";

            return _DbcType;
        }

        // Read first bytes and perform several checks
        private void ReadHeader()
        {
            // Get file length
            _fs_size = _fs.Length;
            // Check if txt or bin
            _IsBinary = CheckIfBinary();
            // Check type of dbc
            if (_fs_size == 0)
                _DbcType = "Undefined";
            if (_IsBinary & _fs_size < 8)
                _DbcType = "Undefined";
            if (_DbcType == null)
            {
                if (_IsBinary)
                    CheckDbcBinType();
                else
                    CheckDbcTxtType();
            }
        }

        // In case of RSDB, detect if it is in 32 or 64 bits mode
        private Boolean CheckId64bits()
        {
            // It would be easier to check filename, 
            //    but safer not to do so with binary files
            if (_IsBinary)
            {
                Byte[] buffer = new Byte[12];
                _fs.Seek(0, SeekOrigin.Begin);
                _fs.Read(buffer, 0, 12);
                if (((BitConverter.ToUInt64(buffer, 4) - 8) / Amount) == 12)
                {
                    // RSDB 64 bits
                    return true;
                }
                return false;
            }
            else
            {
                // With text files it is needed to check filename
                //    while finding a better method
                if (_FileName.Contains("motion"))
                {
                    // RSDB 64 bits
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        
        // Initialize the appropriate Data Store, depending on _DbcType
        // Can throw NotImplementedException "Cannot handle dbc type"
        // TODO: Update on supported types
        //       Now: MATR
        private void InitDataStore()
        {
            switch (_DbcType)
            {
                case "MATR":
                    {
                        _Matr = new MatrFile();
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException("Cannot handle " + _DbcType);
                    }
            }
        }
        #endregion

    }
}

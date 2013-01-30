using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMMol_core.source.IO
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
        private UInt32 _DbcMagicType = 0;
        private Boolean _IsBinary = true;
        private String _DbcType = null;
        private String _FileName = null;
        private FileStream _fs = null;
        private FileStream _dfs = null;
        private StreamReader _sr = null;
        private StreamWriter _sw = null;
        private MatrFile _Matr = null;
        #endregion

        #region Constructor
        //TODO: Add new constructors to create and edit files
        /// <summary>Open a dbc file to read data</summary>
        /// <param name="fileName">Full path to the file</param>
        public dbcFile(String fileName)
        {
            if (File.Exists(fileName))
            {
                // Check if txt or bin
                // Check type of dbc
                _fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                ReadHeader();
                if (_DbcType == "Unsupported")
                {
                    throw new Exception("Unsupported type of DBC file.");
                }
                _FileName = fileName;
                InitDataStore();
                LoadDbc();
            }
            else
            {
                // If the file does not exists
                throw new Exception(fileName + " does not exists");
            }
        }
        
        #endregion

        #region PublicMethods
        // TODO: Load a binary dbc file
        /// <summary>Load a binary dbc file
        /// <remarks>Use first the constructor</remarks>
        /// </summary>
        /// <returns>False if error loading the file</returns>
        public Boolean LoadDbc()
        {
            Boolean result = true;
            return result;
        }

        // TODO: Load a text dbc file
        /// <summary>Load a text ini (dbc) file
        /// <remarks>Use first the constructor</remarks>
        /// </summary>
        /// <returns>False if error loading the file</returns>
        public Boolean LoadTxt()
        {
            Boolean result = true;
            return result;
        }

        // TODO: Save a binary dbc file
        /// <summary>Save to a dbc file</summary>
        /// <param name="fileName">Full path to the file</param>
        /// <returns>False if error saving the file</returns>
        public Boolean SaveDbc(String fileName)
        {
            Boolean result = true;
            return result;
        }

        // TODO: Save a text file
        /// <summary>Save to a text ini (dbc) file</summary>
        /// <param name="fileName">Full path to the file</param>
        /// <returns>False if error saving the file</returns>
        public Boolean SaveTxt(String fileName)
        {
            Boolean result = true;
            return result;
        }
        #endregion

        #region PrivateMethods
        // Detect the Dbc Type by reading first bytes (better than by filename extension)
        // Check if it is in text or binary mode
        // Read the Magic Type Header and the Amount of data
        // TODO: Check if file is too short or empty
        private void ReadHeader()
        {
            // To do: Check if zero bytes file
            // To do: Check if 8 bytes file (binary form)
            Byte[] buffer = new Byte[16];
            _fs.Seek(0, SeekOrigin.Begin);
            // Read DBC Header
            _fs.Read(buffer, 0, 11);
            // Detect if text or binary form and read MagicType
            if (buffer == DBC_TEXT_HEADER)
            {
                _IsBinary = false;
                _DbcMagicType = BitConverter.ToUInt32(buffer, 7);
            }
            else
            {
                _IsBinary = true;
                _DbcMagicType = BitConverter.ToUInt32(buffer, 0);
            }
            if (!DBC_HEADERS_BIN.Contains(_DbcMagicType)) _DbcType = "Unsupported";
            else _DbcType = _enc.GetString(BitConverter.GetBytes(_DbcMagicType));
            // Read Amount
            if (_IsBinary)
            {
                Amount = BitConverter.ToUInt32(buffer, 4);
                // For RSDB binary mode, detect if it is 32 or 64 bits
                if (_DbcType == "RSDB")
                {
                    if (CheckId64bits()) _DbcType += "64";
                    else _DbcType += "32";
                }
            }
            else
            {
                _fs.Seek(0, SeekOrigin.Begin);
                _sr = new StreamReader(_fs, _enc);
                _lineCount = 0;
                // Read Header
                _line = _sr.ReadLine();
                _lineCount++;
                // If RSDB text mode, detect if it is 32 or 64 bits
                if (_DbcType == "RSDB") _DbcType = _line.Split('=')[1];
                // Read Amount
                _line = _sr.ReadLine();
                _lineCount++;
                if (!UInt32.TryParse(_line.Split('=')[1], out Amount))
                {
                    // Bad text format
                    _DbcType = "BadTextFormat";
                }
            }
        }
        // In case of RSDB, detect if it is in 32 or 64 bits mode
        private Boolean CheckId64bits()
        {
            return false;       // RSDB 32 bits
        }
        // TODO: Initialize the appropriate Dictionary, depending on _DbcType
        //   or better a list (class), and later call the reader
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HoMMol_core.Ini;

namespace HoMMol_core.Graphics
{
    // Todo: Change Offsets X, Y, Z to IEEE single.
    /// <summary>Handles an Effect data structure, as a single 3D Effect 
    /// <para>animation. Each animation has a sequence of effects in a loop.</para>
    /// <remarks>Used by EffeFile class, using EffectPart class</remarks>
    /// <example>
    /// Example of an effect in an Effe ini file:
    /// [IceRay01_Atk1]
    /// Amount=1
    /// EffectId0=50974         // Effect Part
    /// TextureId0=50974        // Effect Part
    /// Scale0=51               // Effect Part
    /// ASB0=5                  // Effect Part
    /// ADB0=2                  // Effect Part
    /// ZBuffer0=1              // Effect Part
    /// Billboard0=1            // Effect Part
    /// Delay=0
    /// LoopTime=1
    /// FrameInterval=33
    /// LoopInterval=0
    /// OffsetX=0
    /// OffsetY=0
    /// OffsetZ=0
    /// Billboard=1
    /// Lev=2
    /// </example>
    /// </summary>
    class Effect
    {
        #region Constants
        /// <summary>MAGIC_TYPE_TXT</summary>
        /// <value>Binary files header, in text format</value>
        public const String MAGIC_TYPE_TXT = "EFFE";

        /// <summary>MAGIC_TYPE_BIN</summary>
        /// <value>Binary files header, in binary format</value>
        public const UInt32 MAGIC_TYPE_BIN = 0x45464645; // "EFFE"

        /// <summary>MAX_EFFECT_PARTS</summary>
        /// <value>Any Effect can be composed upto 16 effect parts at most</value>
        // Maybe this should be in Graphics.Common
        public const int MAX_EFFECT_PARTS = 16;
        #endregion

        #region Properties
        /// <summary>Effect animation title, in 3DEffect.ini</summary>
        /// <value>Effect animation title, in 3DEffect.ini</value>
        public String AniTitle;

        /// <summary>Amount of frames</summary>
        /// <value>Amount of frames</value>
        public UInt16 Amount;

        /// <summary>Array of Effects in the loop</summary>
        /// <value>Array of Effects in the loop</value>
        public List<EffectPart> Part;

        /// <summary>Delay after animation, in milliseconds</summary>
        /// <value>Delay after animation, in milliseconds</value>
        public UInt32 Delay;

        /// <summary>How much to repeat the loop, 99999999 means forever</summary>
        /// <value>How much to repeat the loop, 99999999 means forever</value>
        public UInt32 LoopTime;

        /// <summary>Frame duration in milliseconds</summary>
        /// <value>Frame duration in milliseconds</value>
        public UInt32 FrameInterval;

        /// <summary>Time between loops in milliseconds</summary>
        /// <value>Time between loops in milliseconds</value>
        public UInt32 LoopInterval;

        /// <summary>X Axis coord</summary>
        /// <value>X Axis coord</value>
        public float OffsetX;

        /// <summary>Y Axis coord</summary>
        /// <value>Y Axis coord</value>
        public float OffsetY;

        /// <summary>Z Axis coord</summary>
        /// <value>Z Axis coord</value>
        public float OffsetZ;

        /// <summary>Billboard type. Optional, by default is false (0)</summary>
        /// <value>Billboard type. Optional, by default is false (0)</value>
        public Boolean Billboard;

        /// <summary>Show colors or just light, optional, by default 
        /// is _SHOWWAY_NORMAL (0)</summary>
        /// <value>Show colors or just light, optional, by default is 
        /// _SHOWWAY_NORMAL (0)</value>
        public COLOR_ENABLE ColorEnable;

        /// <summary>Priority, optional, 1 by default</summary>
        /// <value>Priority, optional, 1 by default</value>
        public Byte Level;              // Optional, Priority. All 1, except Outskirts Materials Truck and Horse/Elephant level 2

        /// <summary>Unknown. ZeroFill? Level is int16?</summary>
        /// <value>Unknown. ZeroFill? Level is int16?</value>
        public Byte Unknown;            // ZeroFill? Level is int16?

        /// <summary>Comments in ini file</summary>
        /// <value>Comments in ini file</value>
        String Comments;
        #endregion

        #region Constructor
        /// <summary>Default Constructor: New Effect empty instance
        /// <remarks>Does not handle the file, just the data</remarks></summary>
        public Effect()
        {
            AniTitle = "";
            Amount = 0;
            Part = new List<EffectPart>(Amount);
            Delay = 0;
            LoopTime = 1;
            FrameInterval = 33;
            LoopInterval = 0;
            OffsetX = 0;
            OffsetY = 0;
            OffsetZ = 0;
            Billboard = false;
            ColorEnable = COLOR_ENABLE._SHOWWAY_NORMAL;
            Level = 1;
            Unknown = 0;
        }

        /// <summary>New Effect from provided fields, all fields 
        /// but Comments is optional and Billboard = false, 
        /// ColorEnable = _SHOWWAY_NORMAL, Level = 1, Unknown = 0</summary>
        /// <param name="AT">Effect animation title, in 3DEffect.ini</param>
        /// <param name="Am">Amount of frames</param>
        /// <param name="P">List of Effect Parts (frames)</param>
        /// <param name="D">Delay after animation, in milliseconds</param>
        /// <param name="LT">How much to repeat the loop, 99999999 means forever</param>
        /// <param name="FI">Frame duration in milliseconds</param>
        /// <param name="LI">Time between loops in milliseconds</param>
        /// <param name="X">X Axis coord</param>
        /// <param name="Y">Y Axis coord</param>
        /// <param name="Z">Z Axis coord</param>
        /// <param name="C">Optional, Comments as in ini file</param>
        // New instance from provided fields
        public Effect(String AT, UInt16 Am,
            List<EffectPart> P, UInt32 D, UInt32 LT,
            UInt32 FI, UInt32 LI, float X, float Y, float Z,
            String C = null)
        {
            new Effect(AT, Am, P, D, LT, FI, LI, X, Y, Z, 
                false, COLOR_ENABLE._SHOWWAY_NORMAL, 1, 0, C);
        }

        /// <summary>New Effect from provided fields, all fields 
        /// but Comments is optional and ColorEnable = _SHOWWAY_NORMAL, 
        /// Level = 1, Unknown = 0</summary>
        /// <param name="AT">Effect animation title, in 3DEffect.ini</param>
        /// <param name="Am">Amount of frames</param>
        /// <param name="P">List of Effect Parts (frames)</param>
        /// <param name="D">Delay after animation, in milliseconds</param>
        /// <param name="LT">How much to repeat the loop, 99999999 means forever</param>
        /// <param name="FI">Frame duration in milliseconds</param>
        /// <param name="LI">Time between loops in milliseconds</param>
        /// <param name="X">X Axis coord</param>
        /// <param name="Y">Y Axis coord</param>
        /// <param name="Z">Z Axis coord</param>
        /// <param name="B">Billboard type</param>
        /// <param name="C">Optional, Comments as in ini file</param>
        // New instance from provided fields
        public Effect(String AT, UInt16 Am,
            List<EffectPart> P, UInt32 D, UInt32 LT,
            UInt32 FI, UInt32 LI, float X, float Y, float Z, 
            Boolean B, String C = null)
        {
            new Effect(AT, Am, P, D, LT, FI, LI, X, Y, Z, 
                B, COLOR_ENABLE._SHOWWAY_NORMAL, 1, 0, C);
        }

        /// <summary>New Effect from provided fields, all fields 
        /// but Comments is optional and Billboard = false, 
        /// ColorEnable = _SHOWWAY_NORMAL, Unknown = 0</summary>
        /// <param name="AT">Effect animation title, in 3DEffect.ini</param>
        /// <param name="Am">Amount of frames</param>
        /// <param name="P">List of Effect Parts (frames)</param>
        /// <param name="D">Delay after animation, in milliseconds</param>
        /// <param name="LT">How much to repeat the loop, 99999999 means forever</param>
        /// <param name="FI">Frame duration in milliseconds</param>
        /// <param name="LI">Time between loops in milliseconds</param>
        /// <param name="X">X Axis coord</param>
        /// <param name="Y">Y Axis coord</param>
        /// <param name="Z">Z Axis coord</param>
        /// <param name="L">Priority Level</param>
        /// <param name="C">Optional, Comments as in ini file</param>
        // New instance from provided fields
        public Effect(String AT, UInt16 Am,
            List<EffectPart> P, UInt32 D, UInt32 LT,
            UInt32 FI, UInt32 LI, float X, float Y, float Z,
            Byte L, String C = null)
        {
            new Effect(AT, Am, P, D, LT, FI, LI, X, Y, Z,
                false, COLOR_ENABLE._SHOWWAY_NORMAL, L, 0, C);
        }

        /// <summary>New Effect from provided fields, all fields 
        /// but Comments is optional and Unknown = 0</summary>
        /// <param name="AT">Effect animation title, in 3DEffect.ini</param>
        /// <param name="Am">Amount of frames</param>
        /// <param name="P">List of Effect Parts (frames)</param>
        /// <param name="D">Delay after animation, in milliseconds</param>
        /// <param name="LT">How much to repeat the loop, 99999999 means forever</param>
        /// <param name="FI">Frame duration in milliseconds</param>
        /// <param name="LI">Time between loops in milliseconds</param>
        /// <param name="X">X Axis coord</param>
        /// <param name="Y">Y Axis coord</param>
        /// <param name="Z">Z Axis coord</param>
        /// <param name="B">Billboard type</param>
        /// <param name="L">Priority Level</param>
        /// <param name="C">Optional, Comments as in ini file</param>
        // New instance from provided fields
        public Effect(String AT, UInt16 Am,
            List<EffectPart> P, UInt32 D, UInt32 LT,
            UInt32 FI, UInt32 LI, float X, float Y, float Z, 
            Boolean B, Byte L, String C = null)
        {
            new Effect(AT, Am, P, D, LT, FI, LI, X, Y, Z, 
                B, COLOR_ENABLE._SHOWWAY_NORMAL, L, 0, C);
        }

        /// <summary>New Effect from provided fields, all fields 
        /// but Comments is optional</summary>
        /// <param name="AT">Effect animation title, in 3DEffect.ini</param>
        /// <param name="Am">Amount of frames</param>
        /// <param name="P">List of Effect Parts (frames)</param>
        /// <param name="D">Delay after animation, in milliseconds</param>
        /// <param name="LT">How much to repeat the loop, 99999999 means forever</param>
        /// <param name="FI">Frame duration in milliseconds</param>
        /// <param name="LI">Time between loops in milliseconds</param>
        /// <param name="X">X Axis coord</param>
        /// <param name="Y">Y Axis coord</param>
        /// <param name="Z">Z Axis coord</param>
        /// <param name="B">Billboard type</param>
        /// <param name="CE">Show colors or just light
        /// <see cref="COLOR_ENABLE"/></param>
        /// <param name="L">Priority Level</param>
        /// <param name="U">"Unknown" property</param>
        /// <param name="C">Optional, Comments as in ini file</param>
        // New instance from provided fields
        public Effect(String AT, UInt16 Am,
            List<EffectPart> P, UInt32 D, UInt32 LT,
            UInt32 FI, UInt32 LI, float X, float Y, float Z, Boolean B,
            COLOR_ENABLE CE, Byte L, Byte U, String C = null)
        {
            AniTitle = AT;
            Amount = Am;
            Part = P;
            Delay = D;
            LoopTime = LT;
            FrameInterval = FI;
            LoopInterval = LI;
            OffsetX = X;
            OffsetY = Y;
            OffsetZ = Z;
            Billboard = B;
            ColorEnable = CE;
            Level = L;
            Unknown = U;
            Comments = C;
        }

        /// <summary>New instance from byte array, as in dbc Effe binary file
        /// </summary>
        /// <param name="b">Data read from binary dbc file</param>
        public Effect(Byte[] b)
        {
            if (b.Length < 34)      // Bytes 32 and 33 contains the amount
            {
                new Effect();
            }
            else
            {
                Amount = BitConverter.ToUInt16(b, 32);
                Part = new List<EffectPart>(Amount);
                if (b.Length < 66 + 16 * Amount)
                {
                    new Effect();
                }
                else
                {
                    // Check AniTitle length
                    int c = 0;
                    while (b[c] != 0 & c < 32) c++;
                    // Read AniTitle
                    AniTitle = Common.enc.GetString(b, 0, c);
                    // Read all fixed lenght fields
                    Delay = BitConverter.ToUInt32(b, 34);
                    LoopTime = BitConverter.ToUInt32(b, 38);
                    FrameInterval = BitConverter.ToUInt32(b, 42);
                    LoopInterval = BitConverter.ToUInt32(b, 46);
                    OffsetX = BitConverter.ToSingle(b, 50);
                    OffsetY = BitConverter.ToSingle(b, 54);
                    OffsetZ = BitConverter.ToSingle(b, 58);
                    Billboard = (b[62] != 0);
                    ColorEnable = (COLOR_ENABLE)b[63];
                    Level = b[64];
                    Unknown = b[65];
                    Byte[] be = new Byte[16];
                    for (int i = 0; i < Amount; i++)
                    {
                        Buffer.BlockCopy(b, 66 + i * 16, be, 0, 16);
                        EffectPart e = new EffectPart(be);
                        Part.Add(e);
                    }
                }
            }
        }

        /// <summary>New Effect from multiline string</summary>
        /// <param name="str">Lines separated by \r\n, like with this.ToString()</param>
        public Effect(String str)
        {
            String[] st = Common.SplitLines(str);
            new Effect(st);
        }

        /// <summary>New Effect from string array, as in ini files
        /// <remarks>Does not check if correct format</remarks></summary>
        /// <param name="str">Array of strings, one for each line</param>
        public Effect(String[] str)
        {
            ReadFromArray(str, 0);
        }
        #endregion

        #region PublicMethods
        /// <summary>Type of the class</summary>
        /// <returns>Returns "EFFE"</returns>
        public String Type()
        {
            return MAGIC_TYPE_TXT;
        }

        /// <summary>Check if provided instance has the same values, 
        /// except comments</summary>
        /// <returns>Return true if same values</returns>
        public Boolean Equals(Effect E)
        {
            if (E.AniTitle != this.AniTitle) return false;
            for (int i = 0; i < Amount; i++)
            {
                if (!E.Part[i].Equals(this.Part[i])) return false;
            }
            if (E.Part != this.Part) return false;
            if (E.Delay != this.Delay) return false;
            if (E.LoopTime != this.LoopTime) return false;
            if (E.FrameInterval != this.FrameInterval) return false;
            if (E.LoopInterval != this.LoopInterval) return false;
            if (E.OffsetX != this.OffsetX) return false;
            if (E.OffsetY != this.OffsetY) return false;
            if (E.OffsetZ != this.OffsetZ) return false;
            if (E.Billboard != this.Billboard) return false;
            if (E.ColorEnable != this.ColorEnable) return false;
            if (E.Level != this.Level) return false;
            if (E.Unknown != this.Unknown) return false;
            return true;
        }

        /// <summary>Parse to a String, as in ini file
        /// <remarks>Billboard, ColorEnable and Level are skipped when 
        /// default values. "Unknown" property is always skipped</remarks></summary>
        /// <returns>Returns a string ready to save to an ini file</returns>
        // TODO: Check for "Unknown" property
        public override String ToString()
        {
            String s = "";
            if (!String.IsNullOrEmpty(Comments))
                s = Comments + "\r\n";
            s += "[" + AniTitle + "]\r\n";
            s += "Amount=" + Amount.ToString();
            for (int i = 0; i < Amount; i++)
                s += "\r\n" + Part[i].ToString((uint)i);
            s += "\r\nDelay=" + Delay.ToString() + "\r\n";
            s += "LoopTime=" + LoopTime.ToString() + "\r\n";
            s += "FrameInterval=" + FrameInterval.ToString() + "\r\n";
            s += "LoopInterval=" + LoopInterval.ToString() + "\r\n";
            s += "OffsetX=" + OffsetX.ToString() + "\r\n";
            s += "OffsetY=" + OffsetY.ToString() + "\r\n";
            s += "OffsetZ=" + OffsetZ.ToString();
            if (Billboard)
                s += "\r\nBillboard=1";
            if (ColorEnable != COLOR_ENABLE._SHOWWAY_NORMAL)
                s += "\r\nColorEnable=1";
            if (Level != 0)
                s += "\r\nLev=" + Level.ToString();
            // Check here for "Unknown" property
            return "";
        }

        /// <summary>Parse to array of bytes as in dbc Effe files
        /// <remarks>array lenght is 66 + 16 * Amount</remarks></summary>
        /// <returns>Return an array of bytes ready to save to a dbc file</returns>
        public Byte[] ToBytes()
        {
            Byte[] b = new Byte[66 + 16 * Amount];
            // Ensure the values will be zero by default, needed for AniTitle
            Array.Clear(b, 0, b.Length);
            Byte[] at = Common.enc.GetBytes(AniTitle);
            Buffer.BlockCopy(at, 0, b, 0, at.Length);
            // To do: Check if AniTitle has 32 chars
            // Buffer.BlockCopy(at, 0, b, at.Length, 32 - at.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(Amount), 0, b, 32, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(Delay), 0, b, 34, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(LoopTime), 0, b, 38, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(FrameInterval), 0, b, 42, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(LoopInterval), 0, b, 46, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(OffsetX), 0, b, 50, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(OffsetY), 0, b, 54, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(OffsetZ), 0, b, 58, 4);
            b[62] = (byte)(Billboard == false ? 0 : 1);
            b[63] = (byte)ColorEnable;
            b[64] = Level;
            b[65] = Unknown;
            int i = 0;
            foreach (EffectPart p in Part)
            {
                Buffer.BlockCopy(p.ToBytes(), 0, b, 66 + 16 * i, 16);
                i++;
            }
            return b;
        }

        /// <summary>Add comments for this Effect</summary>
        /// <param name="s">String like in ini comment lines</param>
        public void AddComments(String s)
            {
                if (String.IsNullOrEmpty(Comments))
                    Comments = s;
                else
                    Comments += "\r\n" + s;
            }

        /// <summary>Read Effect from string array, as in ini files, 
        /// optionally starting from specified index
        /// <remarks>Does not check if correct format</remarks></summary>
        /// <param name="str">Array of strings, one for each line</param>
        /// <param name="i">Optional, Array index to start reading from
        /// by default is zero</param>
        public int ReadFromArray(String[] str, int i = 0)
        {
            // Check here if there are comments
            PARSE_COMMENTS_STATE commentsState = PARSE_COMMENTS_STATE.NotCommentLine;
            while (i < str.Length
                && (commentsState = IniCommon.ParseLineComments(str[i], commentsState)) >= 0)
            {
                // Store comments
                if (!String.IsNullOrEmpty(Comments))
                    Comments += "\r\n" + str[i];
                else
                    Comments = str[i];
                i++;
            }
            // Read AniTitle and Amount
            AniTitle = str[i].Substring(1, str[0].Length - 2);
            i++;
            Amount = UInt16.Parse(str[i].Split('=')[1]);
            i++;
            if (Amount > 0x0FFF)
                throw new ArgumentOutOfRangeException("Amount", "cannot fit in a signed short, while should be 16 or lower");
            // Check if remaining lines are enough for the amount of parts
            if (str.Length - i < Amount * 4 + 7)
                throw new ArgumentOutOfRangeException("str", "there are too few lines to read Effect, expected at least " + (4 * Amount + 7).ToString() + ", while remaining " + (str.Length - i).ToString());
            Part = new List<EffectPart>(Amount);
            // Read every Part
            for (int j = 0; j < Amount; j++)
            {
                EffectPart ep = new EffectPart();
                i += ep.ReadFromArray(str, i);
                Part.Add(ep);
            }
            // Read the rest of data
            Delay = UInt32.Parse(str[i].Substring(1, str[0].Length - 2));
            i++;
            LoopTime = UInt32.Parse(str[i].Substring(1, str[0].Length - 2));
            i++;
            FrameInterval = UInt32.Parse(str[i].Substring(1, str[0].Length - 2));
            i++;
            LoopInterval = UInt32.Parse(str[i].Substring(1, str[0].Length - 2));
            i++;
            OffsetX = float.Parse(str[i].Substring(1, str[0].Length - 2));
            i++;
            OffsetY = float.Parse(str[i].Substring(1, str[0].Length - 2));
            i++;
            OffsetZ = float.Parse(str[i].Substring(1, str[0].Length - 2));
            i++;
            if (i < str.Length && str[i].StartsWith("Billboard"))
            {
                Billboard = (Byte.Parse(str[i].Split('=')[1]) != 0);
                i++;
            }
            if (i < str.Length && str[i].StartsWith("ColorEnable"))
            {
                ColorEnable = (COLOR_ENABLE)Byte.Parse(str[i].Split('=')[1]);
                i++;
            }
            if (i < str.Length && str[i].StartsWith("Lev"))
            {
                Level = Byte.Parse(str[i].Split('=')[1]);
                i++;
            }
            if (i < str.Length
                && !String.IsNullOrWhiteSpace(str[i])
                && str[i].Contains("="))
            {
                Unknown = Byte.Parse(str[i].Split('=')[1]);
                i++;
            }
            return i;
        }
        #endregion
    }
}

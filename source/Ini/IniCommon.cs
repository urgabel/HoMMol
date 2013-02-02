using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMMol_core.Ini
{
    /// <summary>Possible states when parsing lines with 
    /// <see cref="IniCommon.ParseLineComments(String, PARSE_COMMENTS_STATE)"/></summary>
    public enum PARSE_COMMENTS_STATE
    {
        /// <summary>Line is not a comment</summary>
        NotCommentLine = -1,

        /// <summary>Line is empty, just spaces or null 
        /// (consider it as a comment</summary>
        SpacesEmptyOrNull = 0,

        /// <summary>Line is a single line comment, 
        /// as starts with a semicolon ";"</summary>
        SingleLineSemicolon = 1,

        /// <summary>Line is a single line comment, 
        /// as starts with double slash "//"</summary>
        SingleLineDoubleSlash = 2,

        /// <summary>Line is a single line comment, 
        /// as starts with slash star "/*" and ends with 
        /// star slash "*/"</summary>
        SingleLineSlashStar = 3,

        /// <summary>Line is a multiline comment, 
        /// as starts with slash star "/*" and does not end 
        /// with star slash "*/"</summary>
        MultiLineSlashStarBegin = 4,

        /// <summary>Line is a multiline comment, 
        /// as the previous state is multiline comment open 
        /// with slash star "/*" and not closed with star slash "*/"</summary>
        MultiLineSlashStarContinue = 5,

        /// <summary>Line is a multiline comment, 
        /// as the previous state is multiline comment open
        /// with slash star "/*" and not closed with star slash "*/",
        /// but the multiline comment finish here, as this line ends
        /// with slash star "/*"</summary>
        MultiLineSlashStarEnd = 6
    }

    /// <summary>Shared constants and methods to handle ini files</summary>
    public static class IniCommon
    {
        /// <summary>Parse an ini line and search for comments, without taking
        /// in account state from previous line. Can use
        /// <seealso cref="ParseLineComments(String, PARSE_COMMENTS_STATE)"/> 
        /// for not first line.</summary>
        /// <param name="s">Line to parse</param>
        /// <returns>-1 if not comment line; 0 if empty, spaces or null; 
        /// higher values if comment found
        /// <see cref="PARSE_COMMENTS_STATE"/></returns>
        public static PARSE_COMMENTS_STATE ParseLineComments(String s)
        {
            return ParseLineComments(s, PARSE_COMMENTS_STATE.NotCommentLine);
        }

        /// <summary>Parse an ini line and search for comments, taking in 
        /// account state from previous line. Can use
        /// <seealso cref="ParseLineComments(String)"/> for first line.</summary>
        /// <param name="s">Line to parse</param>
        /// <param name="st">Previous state, from last line parsed</param>
        /// <returns>-1 if not comment line; 0 if empty, spaces or null; 
        /// higher values if comment found
        /// <see cref="PARSE_COMMENTS_STATE"/></returns>
        public static PARSE_COMMENTS_STATE ParseLineComments(String s, 
            PARSE_COMMENTS_STATE st)
        {
            if (st == PARSE_COMMENTS_STATE.MultiLineSlashStarBegin
                | st == PARSE_COMMENTS_STATE.MultiLineSlashStarContinue)
            {
                if (s.EndsWith("*/"))
                    return PARSE_COMMENTS_STATE.MultiLineSlashStarEnd;
                else return PARSE_COMMENTS_STATE.MultiLineSlashStarContinue;
            }
            else
            {
                if (String.IsNullOrWhiteSpace(s))
                    return PARSE_COMMENTS_STATE.SpacesEmptyOrNull;
                if (s.StartsWith(";"))
                    return PARSE_COMMENTS_STATE.SingleLineSemicolon;
                if (s.StartsWith("//"))
                    return PARSE_COMMENTS_STATE.SingleLineDoubleSlash;
                if (s.StartsWith("/*"))
                    if (s.EndsWith("*/"))
                        return PARSE_COMMENTS_STATE.SingleLineSlashStar;
                    else return PARSE_COMMENTS_STATE.MultiLineSlashStarBegin;
            }
            return PARSE_COMMENTS_STATE.NotCommentLine;
        }

        /// <summary>Parse ini lines searching for comments</summary>
        /// <param name="iniLines">List of lines to parse</param>
        /// <returns>comments lines until a not comment line is found</returns>
        public static List<String> ReadComments(List<String> iniLines)
        {
            Boolean longComments = false;
            Boolean notCommentLineFound = false;
            List<String> Comments = null;
            int i = 0;
            while (i < iniLines.Count & notCommentLineFound)
            {
                if (longComments)
                {
                    Comments.Add(String.Empty);
                    i++;
                    if (iniLines[i].EndsWith("*/"))
                    {
                        longComments = false;
                    }
                }
                else if (String.IsNullOrWhiteSpace(iniLines[i]))
                {
                    Comments.Add(String.Empty);
                    i++;
                }
                else if (iniLines[i].Substring(0, 1) == ";"
                    | iniLines[i].Substring(0, 2) == "//")
                {
                    Comments.Add(iniLines[i]);
                    i++;
                }
                else if (iniLines[i].Substring(0, 2) == "/*")
                {
                    longComments = true;
                    Comments.Add(iniLines[i]);
                    i++;
                }
                else
                {
                    notCommentLineFound = true;
                }
            }
            return Comments;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMMol_core.Crypto
{
    /// Static class to handle hashnames from everywhere
    public static class HashNames
    {
        /// <summary>Get the hash number Id from a full path+filename
        /// <para>Adapted from WindSoul Game Engine source code</para>
        /// <para>Function: unsigned long string_id(const char *str)</para>
        /// <para>http://read.pudn.com/downloads76/sourcecode/game/281928/%E9%A3%8E%E9%AD%82/wdfpck.cpp__.htm</para>
        /// <paramref name="str">String to convert into hash Id</paramref>
        /// <returns>a 32 bits unique Id</returns>
        /// </summary>
        public static UInt32 String2Id(string str)
        {
            const UInt32 MAX_INT32 = 0xFFFFFFFF;    // Max value for a 32 bits integer
            const UInt32 ADD_1 = 0x9BE74448;        // 
            const UInt32 ADD_2 = 0x66F42C48;        // ADD_1 and ADD_2 are added to the end of the string
            const UInt32 V_0 = 0xF4FA8928;          // Rotative mask, initial value
            const UInt32 X_0 = 0x37A8470E;          // Common mask x, initial value
            const UInt32 Y_0 = 0x7758B42B;          // Common mask y, initial value
            const UInt32 MASK_W = 0x267B0B11;       // 0010 1100 0111 1011 0000 1011 0001 0001
            const UInt32 MASK_A = 0x02040801;       // 0000 0010 0000 0100 0000 1000 0000 0001
            const UInt32 MASK_B = 0x00804021;       // 0000 0000 1000 0000 0100 0000 0010 0001
            const UInt32 MASK_C = 0xBFEF7FDF;       // 1011 1111 1110 1111 0111 1111 1101 1111
            const UInt32 MASK_D = 0x7DFEFBFF;       // 0111 1101 1111 1110 1111 1011 1111 1111
            UInt32 v = V_0;                         // Rotative mask
            UInt32 x = X_0;                         // Common mask x
            UInt32 y = Y_0;                         // Common mask y
            Int32 length;                           // String lenght, counted in 32 bits integers
            UInt32 low;                             // To handle 32bits numbers
            UInt64 high, op_1, op_2;                // To handle 64bits numbers

            // Replace '\\' to '/' and all letters to lowercase
            str = str.Replace('\\', '/');
            str = str.ToLowerInvariant();

            // Create an array (m) of unsigned int to store the string
            ASCIIEncoding enc = new ASCIIEncoding();    // Use ASCII to get unchanged bytes
            Byte[] by = new Byte[280];                  // The string should be less than 256 bytes
            Array.Clear(by, 0, 280);                    // Fill with zeroes
            UInt32[] m = new UInt32[70];                // 70 x 32 bits integers can host 280 bytes
            Array.Clear(m, 0, 70);                      // Fill with zeroes
            by = enc.GetBytes(str);
            Buffer.BlockCopy(by, 0, m, 0, str.Length);

            // Check the lenght of m, actually used by str and add 2 values to the end
            for (length = 0; length < 64 && m[length] != 0; length++) ; // length appoint to the last integer not zero in the m array 
            m[length++] = ADD_1;
            m[length++] = ADD_2;

            // With each int from the string perform several operations
            for (int i = 0; i < length; i++)
            {
                // Rotate left v 1 bit for each 32bits m[i]
                v = (v << 1) | (v >> 0x1F);
                // Mask x and y with the current 32bits m[i]
                x ^= m[i];
                y ^= m[i];

                // Operate to x and y several masks
                // MASK_C and MASK_D disable 4 bits each
                // MASK_A and MASK_B enable 4 bits each
                // MASK_W switches 14 bits from rotative mask v
                op_1 = (UInt64)y * ((((v ^ MASK_W) + x) | MASK_B) & MASK_D);
                op_2 = (UInt64)x * ((((v ^ MASK_W) + y) | MASK_A) & MASK_C);

                // if op_1 has more than 32 bits, op_1 = lower32bits + 2 * higher32bits
                low = (UInt32)op_1;
                high = (UInt64)2 * ((UInt32)(op_1 >> 32));
                op_1 = (UInt64)low + (UInt32)high;
                // if 2 * higher32bits was again more than 32 bits, add 1
                if (high > MAX_INT32) op_1++;

                // if op_2 has more than 32 bits, op_2 = lower32bits + higher32bits + 1
                // otherwise take just the lower32bits 
                op_2 = (op_2 > MAX_INT32) ? (UInt64)1 + (UInt32)op_2 + (UInt32)(op_2 >> 32) : (UInt32)op_2;

                // Store in x the lower32bits from the result of operation2
                // and add 1 if the last operation exceeded 32 bits
                x = (UInt32)op_2;
                if (op_2 > MAX_INT32) x++;
                // Store in y the lower32bits from the result of operation1
                // and add 2 if the last operation exceeded 32 bits
                y = (UInt32)op_1;
                if (op_1 > MAX_INT32) y += 2;
            }

            // Mask x with y and return the result
            UInt32 Id = x ^ y;
            return Id;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMMol_core.IO
{
    /// <summary>Materials list, used in Mesh files</summary>
    class Matr
    {
        #region Properties
        /// <summary>Identifier name, can be in chinese</summary>
        String Id;
        /// <summary>Diffuse color RGBA</summary>
        D3DColor Diffuse;
        /// <summary>Ambient color RGB</summary>
        D3DColor Ambient;
        /// <summary>Specular 'shininess'</summary>
        D3DColor Specular;
        /// <summary>Emissive color RGB</summary>
        D3DColor Emissive;
        /// <summary>Sharpness if specular highlight</summary>
        float Power;
        #endregion

        #region Constructor
        /// <summary>New default Material</summary>
        public Matr()
        {
            Id = "default";
            Diffuse = new D3DColor(0xFFFFFFFF);
            Ambient = new D3DColor(0xFFFFFFFF);
            Specular = new D3DColor(0xFFFFFFFF);
            Emissive = new D3DColor(0);
            Power = 0;
        }
        /// <summary>New Material from provided fields</summary>
        public Matr(String id, D3DColor d, D3DColor a, D3DColor s,
            D3DColor e, float p)
        {
            Id = id;
            Diffuse = d;
            Ambient = a;
            Specular = s;
            Emissive = e;
            Power = p;
        }
        /// <summary>New Material from a string, like in Matr ini file</summary>
        public Matr(String str)
        {
            UInt32 d, a, s, e;
            String[] st = str.Split(' ');
            if (st.Length != 6) throw new ArgumentException("Incorrect Material format; str must have 6 fields separated by spaces like this:\r\nwater FFFFFFFF FFFFFFFF FF60DEE6 FF47607E 5");
            if (!UInt32.TryParse(st[1], out d)) throw new ArgumentException("Incorrect Material format; 2nd field cannot be parsed to UInt32.");
            if (!UInt32.TryParse(st[2], out a)) throw new ArgumentException("Incorrect Material format;  3rd cannot be parsed to UInt32.");
            if (!UInt32.TryParse(st[3], out s)) throw new ArgumentException("Incorrect Material format;  4th cannot be parsed to UInt32.");
            if (!UInt32.TryParse(st[4], out e)) throw new ArgumentException("Incorrect Material format;  5th cannot be parsed to UInt32.");
            if (!float.TryParse(st[5], out Power)) throw new ArgumentException("Incorrect Material format; 6th field cannot be parsed to Single IEEE float.");
            Id = st[0];
            Diffuse = new D3DColor(d);
            Ambient = new D3DColor(a);
            Specular = new D3DColor(s);
            Emissive = new D3DColor(e);
        }
        /// <summary>New Material from a byte array, like in Matr dbc file</summary>
        public Matr(Byte[] b)
        {
            Byte[] c = new Byte[4];
            Byte[] i = new Byte[32];
            Buffer.BlockCopy(b, 0, i, 0, 32);
            Id = Common.Bytes32ToString(i);
            Diffuse = new D3DColor(b[32], b[33], b[34], b[35]);
            Ambient = new D3DColor(b[36], b[37], b[38], b[39]);
            Specular = new D3DColor(b[40], b[41], b[42], b[43]);
            Emissive = new D3DColor(b[44], b[45], b[46], b[47]);
            Power = BitConverter.ToSingle(b, 48);
        }
        #endregion

        #region PublicMethods
        /// <summary>Get the material in string format</summary>
        /// <returns>a string, as stored in Matr ini file</returns>
        public override string ToString()
        {
            return Id + " " + Diffuse.ToString() + " " + Ambient.ToString()
                + " " + Specular.ToString() + " " + Emissive.ToString()
                + " " + Power.ToString();
        }
        /// <summary>Get the material in a byte array</summary>
        /// <returns>a 52 bytes array, as stored in Matr dbc file</returns>
        public Byte[] ToBytes()
        {
            Byte[] b = new Byte[52];
            Buffer.BlockCopy(Diffuse.ToBytes(),0,b,32,4);
            Buffer.BlockCopy(Ambient.ToBytes(), 0, b, 36, 4);
            Buffer.BlockCopy(Specular.ToBytes(), 0, b, 40, 4);
            Buffer.BlockCopy(Emissive.ToBytes(), 0, b, 44, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Power), 0, b, 48, 4);
            return b;
        }
        #endregion
    }
}

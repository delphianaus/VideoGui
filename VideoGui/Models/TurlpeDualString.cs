using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class TurlpeDualString : IConvertible
    {
        public string turlpe1 { get; set; }
        public string turlpe2 { get; set; }

        public int Id { get; set; }
        public TurlpeDualString(string turlpe1, string turlpe2, int id=-1)
        {
            this.turlpe1 = turlpe1;
            this.turlpe2 = turlpe2;
            this.Id = id;
        }

        public TypeCode GetTypeCode() => TypeCode.Object;

        public bool ToBoolean(IFormatProvider provider) => throw new InvalidCastException();
        public byte ToByte(IFormatProvider provider) => throw new InvalidCastException();
        public char ToChar(IFormatProvider provider) => throw new InvalidCastException();
        public DateTime ToDateTime(IFormatProvider provider) => throw new InvalidCastException();
        public decimal ToDecimal(IFormatProvider provider) => throw new InvalidCastException();
        public double ToDouble(IFormatProvider provider) => throw new InvalidCastException();
        public short ToInt16(IFormatProvider provider) => throw new InvalidCastException();
        public int ToInt32(IFormatProvider provider) => Id;
        public long ToInt64(IFormatProvider provider) => Id;
        public sbyte ToSByte(IFormatProvider provider) => throw new InvalidCastException();
        public float ToSingle(IFormatProvider provider) => throw new InvalidCastException();
        public string ToString(IFormatProvider provider) => $"{turlpe1},{turlpe2}";
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(string))
                return ToString(provider);
            if (conversionType == typeof(int))
                return ToInt32(provider);
            throw new InvalidCastException();
        }
        public ushort ToUInt16(IFormatProvider provider) => throw new InvalidCastException();
        public uint ToUInt32(IFormatProvider provider) => throw new InvalidCastException();
        public ulong ToUInt64(IFormatProvider provider) => throw new InvalidCastException();
    }
}

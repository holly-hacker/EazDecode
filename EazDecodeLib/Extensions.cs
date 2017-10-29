using System;
using System.IO;
using System.Text;

namespace EazDecodeLib
{
    internal static class Extensions
    {
        public static Guid ReadGuid(this BinaryReader br)
        {
            byte[] array = br.ReadBytes(16);
            return new Guid(array[0] | array[1] << 8 | array[2] << 16 | array[3] << 24,
                (short)(array[4] | array[5] << 8),
                (short)(array[6] | array[7] << 8),
                array[8], array[9], array[10], array[11], array[12], array[13], array[14], array[15]);
        }

        public static string ReadStringNullTerminated(this BinaryReader br)
        {
            var sb = new StringBuilder();
            for (;;) {
                byte b = br.ReadByte();
                if (b == 0) break;
                sb.Append((char) b);
            }
            return sb.ToString();
        }

        public static int ReadByteCrypted(this BinaryReader br, byte unk3)
        {
            byte b = br.ReadByte();
            if (b <= unk3) return b;

            b -= (byte)(unk3 + 1);

            byte b2 = br.ReadByte();
            return (b << 8 | b2) + unk3 - b;
        }
    }
}

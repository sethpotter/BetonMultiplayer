using System.Text;
using System;

namespace BetonMultiplayer
{
    public class NetworkUtil
    {
        public static byte[] EncodeStringToBytes(string str)
        {
            return Encoding.Default.GetBytes(str);
        }

        public static string DecodeBytesToString(byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }

        public static byte[] EncodeIntToBytes(int integer)
        {
            return BitConverter.GetBytes(integer);
        }

        public static int DecodeBytesToInt(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}

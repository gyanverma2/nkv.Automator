using System.Security.Cryptography;

namespace nkv.GetAutomator.Utility
{
    public static class Helper
    {
        public static int GetRandomInteger()
        {
            byte[] four_bytes = new byte[4];

            System.Security.Cryptography.RandomNumberGenerator.Create().GetBytes(four_bytes);

            return Math.Abs(BitConverter.ToInt32(four_bytes, 0));
        }
    }
}

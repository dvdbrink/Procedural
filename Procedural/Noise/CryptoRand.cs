using System;
using System.Security.Cryptography;

namespace Procedural.Noise
{
    public static class CryptoRand
    {
        public static long NextInt64()
        {
            var bytes = new byte[sizeof (long)];
            var gen = new RNGCryptoServiceProvider();
            gen.GetBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }
    }
}
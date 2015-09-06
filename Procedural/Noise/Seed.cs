namespace Procedural.Noise
{
    public sealed class Seed
    {
        private readonly short[] _values = new short[256];
        private readonly short[] _values3D = new short[256];

        public Seed() : this(CryptoRand.NextInt64())
        {
        }

        public Seed(long seed)
        {
            var source = new short[256];
            for (short i = 0; i < 256; i++) source[i] = i;
            seed = seed*6364136223846793005L + 1442695040888963407L;
            seed = seed*6364136223846793005L + 1442695040888963407L;
            seed = seed*6364136223846793005L + 1442695040888963407L;
            for (var i = 255; i >= 0; i--)
            {
                seed = seed*6364136223846793005L + 1442695040888963407L;
                var r = (int) ((seed + 31)%(i + 1));
                if (r < 0) r += (i + 1);
                _values[i] = source[r];
                _values3D[i] = (short) ((_values[i]%(72/3))*3);
                source[r] = source[i];
            }
        }

        public int Get(int x)
        {
            return _values[x & 0xff];
        }

        public int Get(int x, int y)
        {
            return _values[(Get(x) + y) & 0xff];
        }

        public int Get(int x, int y, int z)
        {
            return _values3D[(Get(x, y) + z) & 0xff];
        }

        public int Get(int x, int y, int z, int w)
        {
            return _values[(Get(x, y, z) + w) & 0xff];
        }
    }
}
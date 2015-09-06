using System;

namespace Procedural.Noise
{
    public sealed class Brownian
    {
        private readonly int _octaves;
        private readonly double _startFrequency;
        private readonly double _lacunarity;
        private readonly double _persistence;

        public Brownian(int octaves, double startFrequency, double lacunarity, double persistence)
        {
            _octaves = octaves;
            _startFrequency = startFrequency;
            _lacunarity = lacunarity;
            _persistence = persistence;
        }

        public double Get(Seed seed, double x, double y, Func<Seed, double, double, double> noise)
        {
            var frequency = _startFrequency;
            double amplitude = 1;
            double result = 0;

            for (var octave = 0; octave < _octaves; ++octave)
            {
                result += noise(seed, x*frequency, y*frequency)*amplitude;
                amplitude *= _persistence;
                frequency *= _lacunarity;
            }

            return result;
        }

        public double Get(Seed seed, double x, double y, double z, Func<Seed, double, double, double, double> noise)
        {
            var frequency = _startFrequency;
            double amplitude = 1;
            double result = 0;

            for (var octave = 0; octave < _octaves; ++octave)
            {
                result += noise(seed, x*frequency, y*frequency, z*frequency)*amplitude;
                amplitude *= _persistence;
                frequency *= _lacunarity;
            }

            return result;
        }
    }
}
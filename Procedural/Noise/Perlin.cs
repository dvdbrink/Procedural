using System;

namespace Procedural.Noise
{
    public class Perlin
    {
        private static readonly Grad[] Grad3 =
        {
            new Grad {X = 1, Y = 1, Z = 0}, new Grad {X = -1, Y = 1, Z = 0}, new Grad {X = 1, Y = -1, Z = 0},
            new Grad {X = -1, Y = -1, Z = 0},
            new Grad {X = 1, Y = 0, Z = 1}, new Grad {X = -1, Y = 0, Z = 1}, new Grad {X = 1, Y = 0, Z = -1},
            new Grad {X = -1, Y = 0, Z = -1},
            new Grad {X = 0, Y = 1, Z = 1}, new Grad {X = 0, Y = -1, Z = 1}, new Grad {X = 0, Y = 1, Z = -1},
            new Grad {X = 0, Y = -1, Z = -1}
        };

        private static Grad[] _grad4 =
        {
            new Grad {X = 0, Y = 1, Z = 1, W = 1}, new Grad {X = 0, Y = 1, Z = 1, W = -1},
            new Grad {X = 0, Y = 1, Z = -1, W = 1}, new Grad {X = 0, Y = 1, Z = -1, W = -1},
            new Grad {X = 0, Y = -1, Z = 1, W = 1}, new Grad {X = 0, Y = -1, Z = 1, W = -1},
            new Grad {X = 0, Y = -1, Z = -1, W = 1}, new Grad {X = 0, Y = -1, Z = -1, W = -1},
            new Grad {X = 1, Y = 0, Z = 1, W = 1}, new Grad {X = 1, Y = 0, Z = 1, W = -1},
            new Grad {X = 1, Y = 0, Z = -1, W = 1}, new Grad {X = 1, Y = 0, Z = -1, W = -1},
            new Grad {X = -1, Y = 0, Z = 1, W = 1}, new Grad {X = -1, Y = 0, Z = 1, W = -1},
            new Grad {X = -1, Y = 0, Z = -1, W = 1}, new Grad {X = -1, Y = 0, Z = -1, W = -1},
            new Grad {X = 1, Y = 1, Z = 0, W = 1}, new Grad {X = 1, Y = 1, Z = 0, W = -1},
            new Grad {X = 1, Y = -1, Z = 0, W = 1}, new Grad {X = 1, Y = -1, Z = 0, W = -1},
            new Grad {X = -1, Y = 1, Z = 0, W = 1}, new Grad {X = -1, Y = 1, Z = 0, W = -1},
            new Grad {X = -1, Y = -1, Z = 0, W = 1}, new Grad {X = -1, Y = -1, Z = 0, W = -1},
            new Grad {X = 1, Y = 1, Z = 1, W = 0}, new Grad {X = 1, Y = 1, Z = -1, W = 0},
            new Grad {X = 1, Y = -1, Z = 1, W = 0}, new Grad {X = 1, Y = -1, Z = -1, W = 0},
            new Grad {X = -1, Y = 1, Z = 1, W = 0}, new Grad {X = -1, Y = 1, Z = -1, W = 0},
            new Grad {X = -1, Y = -1, Z = 1, W = 0}, new Grad {X = -1, Y = -1, Z = -1, W = 0}
        };

        private static readonly byte[] PermutationTable =
        {
            151, 160, 137, 91, 90, 15,
            131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
            190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
            88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
            77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
            102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
            135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
            5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
            223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
            129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
            251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
            49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
            138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
        };

        private static readonly byte[] p;

        static Perlin()
        {
            p = new byte[512];
            for (var i = 0; i < 512; ++i)
            {
                p[i] = PermutationTable[i & 255];
            }
        }

        private static int FastFloor(double x)
        {
            return x > 0 ? (int) x : (int) x - 1;
        }

        public static double Get(Seed seed, double x)
        {
            // Find unit grid cell containing point
            var X = FastFloor(x);

            // Get relative xyz coordinates of point within that cell
            x -= X;

            // Wrap the integer cells at 255 (smaller integer period can be introduced here)
            X &= 255;

            // Calculate a set of eight hashed gradient indices
            var gi0 = p[X]%12;
            var gi1 = p[X + 1]%12;

            // The gradients of each corner are now:
            // g0 = grad3[gi0];
            // g1 = grad3[gi1];

            // Calculate noise contributions from each of the four corners
            var n0 = Dot(Grad3[gi0], x);
            var n1 = Dot(Grad3[gi1], x - 1);

            // Compute the fade curve value for each of x, y
            var u = Fade(x);

            // Interpolate along x the contributions from each of the corners
            var nx = Lerp(n0, n1, u);

            return nx;
        }

        public static double Get(Seed seed, double x, double y)
        {
            // Find unit grid cell containing point
            var X = FastFloor(x);
            var Y = FastFloor(y);

            // Get relative xyz coordinates of point within that cell
            x -= X;
            y -= Y;

            // Wrap the integer cells at 255 (smaller integer period can be introduced here)
            X &= 255;
            Y &= 255;

            // Calculate a set of eight hashed gradient indices
            var gi00 = p[X + p[Y]]%12;
            var gi01 = p[X + p[Y]]%12;
            var gi10 = p[X + 1 + p[Y]]%12;
            var gi11 = p[X + 1 + p[Y]]%12;

            // The gradients of each corner are now:
            // g00 = grad3[gi00];
            // g01 = grad3[gi01];
            // g10 = grad3[gi10];
            // g11 = grad3[gi11];

            // Calculate noise contributions from each of the four corners
            var n00 = Dot(Grad3[gi00], x, y);
            var n10 = Dot(Grad3[gi10], x - 1, y);
            var n01 = Dot(Grad3[gi01], x, y - 1);
            var n11 = Dot(Grad3[gi11], x - 1, y - 1);

            // Compute the fade curve value for each of x, y
            var u = Fade(x);
            var v = Fade(y);

            // Interpolate along x the contributions from each of the corners
            var nx0 = Lerp(n00, n10, u);
            var nx1 = Lerp(n01, n11, u);

            // Interpolate the two results along y
            var nxy = Lerp(nx0, nx1, v);

            return nxy;
        }

        public static double Get(Seed seed, double x, double y, double z)
        {
            // Find unit grid cell containing point
            var X = FastFloor(x);
            var Y = FastFloor(y);
            var Z = FastFloor(z);

            // Get relative xyz coordinates of point within that cell
            x -= X;
            y -= Y;
            z -= Z;

            // Wrap the integer cells at 255 (smaller integer period can be introduced here)
            X &= 255;
            Y &= 255;
            Z &= 255;

            // Calculate a set of eight hashed gradient indices
            var gi000 = p[X + p[Y + p[Z]]]%12;
            var gi001 = p[X + p[Y + p[Z + 1]]]%12;
            var gi010 = p[X + p[Y + 1 + p[Z]]]%12;
            var gi011 = p[X + p[Y + 1 + p[Z + 1]]]%12;
            var gi100 = p[X + 1 + p[Y + p[Z]]]%12;
            var gi101 = p[X + 1 + p[Y + p[Z + 1]]]%12;
            var gi110 = p[X + 1 + p[Y + 1 + p[Z]]]%12;
            var gi111 = p[X + 1 + p[Y + 1 + p[Z + 1]]]%12;

            // The gradients of each corner are now:
            // g000 = grad3[gi000];
            // g001 = grad3[gi001];
            // g010 = grad3[gi010];
            // g011 = grad3[gi011];
            // g100 = grad3[gi100];
            // g101 = grad3[gi101];
            // g110 = grad3[gi110];
            // g111 = grad3[gi111];

            // Calculate noise contributions from each of the eight corners
            var n000 = Dot(Grad3[gi000], x, y, z);
            var n100 = Dot(Grad3[gi100], x - 1, y, z);
            var n010 = Dot(Grad3[gi010], x, y - 1, z);
            var n110 = Dot(Grad3[gi110], x - 1, y - 1, z);
            var n001 = Dot(Grad3[gi001], x, y, z - 1);
            var n101 = Dot(Grad3[gi101], x - 1, y, z - 1);
            var n011 = Dot(Grad3[gi011], x, y - 1, z - 1);
            var n111 = Dot(Grad3[gi111], x - 1, y - 1, z - 1);

            // Compute the fade curve value for each of x, y, z
            var u = Fade(x);
            var v = Fade(y);
            var w = Fade(z);

            // Interpolate along x the contributions from each of the corners
            var nx00 = Lerp(n000, n100, u);
            var nx01 = Lerp(n001, n101, u);
            var nx10 = Lerp(n010, n110, u);
            var nx11 = Lerp(n011, n111, u);

            // Interpolate the four results along y
            var nxy0 = Lerp(nx00, nx10, v);
            var nxy1 = Lerp(nx01, nx11, v);

            // Interpolate the two last results along z
            var nxyz = Lerp(nxy0, nxy1, w);

            return nxyz;
        }

        public static double Get(Seed seed, double x, double y, double z, double w)
        {
            throw new NotImplementedException();
        }

        private static double Dot(Grad g, double x)
        {
            return g.X*x;
        }

        private static double Dot(Grad g, double x, double y)
        {
            return g.X*x + g.Y*y;
        }

        private static double Dot(Grad g, double x, double y, double z)
        {
            return g.X*x + g.Y*y + g.Z*z;
        }

        private static double Fade(double t)
        {
            return t*t*t*(t*(t*6 - 15) + 10);
        }

        private static double Lerp(double a, double b, double t)
        {
            return (1 - t)*a + t*b;
        }

        private struct Grad
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public double W { get; set; }
        }
    }
}
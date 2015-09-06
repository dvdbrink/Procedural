using System;

namespace Procedural.Noise
{
    public static class OpenSimplex
    {
        private const double StretchConstant_2D = -0.211324865405187; //(1/Math.Sqrt(2+1)-1)/2;
        private const double SquishConstant_2D = 0.366025403784439; //(Math.Sqrt(2+1)-1)/2;
        private const double StretchConstant_3D = -1.0/6; //(1/Math.Sqrt(3+1)-1)/3;
        private const double SquishConstant_3D = 1.0/3; //(Math.Sqrt(3+1)-1)/3;
        private const double StretchConstant_4D = -0.138196601125011; //(1/Math.Sqrt(4+1)-1)/4;
        private const double SquishConstant_4D = 0.309016994374947; //(Math.Sqrt(4+1)-1)/4;
        private const double NormConstant_2D = 47;
        private const double NormConstant_3D = 103;
        private const double NormConstant_4D = 30;

        public static double Get(Seed seed, double x)
        {
            throw new NotImplementedException();
        }

        public static double Get(Seed seed, double x, double y)
        {
            const double zero = 0d;
            const double one = 1d;
            const double two = 2d;

            // Place input coordinates onto grid.
            var stretchOffset = (x + y)*StretchConstant_2D;
            var xs = x + stretchOffset;
            var ys = y + stretchOffset;

            // Floor to get grid coordinates of rhombus (stretched square) super-cell origin.
            var xsFloor = Math.FastFloor(xs);
            var ysFloor = Math.FastFloor(ys);

            // Skew out to get actual coordinates of rhombus origin. We'll need these later.
            var squishOffset = (xsFloor + ysFloor)*SquishConstant_2D;
            var xFloor = xsFloor + squishOffset;
            var yFloor = ysFloor + squishOffset;

            // Compute grid coordinates relative to rhombus origin.
            var xsFrac = xs - xsFloor;
            var ysFrac = ys - ysFloor;

            // Sum those together to get a value that determines which region we're in.
            var fracSum = xsFrac + ysFrac;

            // Positions relative to origin point.
            var dx0 = x - xFloor;
            var dy0 = y - yFloor;

            var value = zero;

            // Contribution (1,0)
            var dx1 = dx0 - one - SquishConstant_2D;
            var dy1 = dy0 - zero - SquishConstant_2D;
            value += Gradient.Get(seed, xsFloor + 1, ysFloor, dx1, dy1);

            // Contribution (0,1)
            var dx2 = dx0 - zero - SquishConstant_2D;
            var dy2 = dy0 - one - SquishConstant_2D;
            value += Gradient.Get(seed, xsFloor, ysFloor + 1, dx2, dy2);

            // Check if we're inside the triangle (2-Simplex) at (1,1)
            if (fracSum > one)
            {
                xsFloor++;
                ysFloor++;
                dx0 = dx0 - one - two*SquishConstant_2D;
                dy0 = dy0 - one - two*SquishConstant_2D;
            }

            // Contribution (0,0) or (1,1)
            value += Gradient.Get(seed, xsFloor, ysFloor, dx0, dy0);

            return value/NormConstant_2D;
        }

        public static double Get(Seed seed, double x, double y, double z)
        {
            const double zero = 0d;
            const double one = 1d;
            const double two = 2d;
            const double three = 3d;

            // Place input coordinates on simplectic honeycomb.
            var stretchOffset = (x + y + z)*StretchConstant_3D;
            var xs = x + stretchOffset;
            var ys = y + stretchOffset;
            var zs = z + stretchOffset;

            // Floor to get simplectic honeycomb coordinates of rhombohedron (stretched cube) super-cell origin.
            var xsFloor = Math.FastFloor(xs);
            var ysFloor = Math.FastFloor(ys);
            var zsFloor = Math.FastFloor(zs);

            // Skew out to get actual coordinates of rhombohedron origin. We'll need these later.
            var squishOffset = (xsFloor + ysFloor + zsFloor)*SquishConstant_3D;
            var xFloor = xsFloor + squishOffset;
            var yFloor = ysFloor + squishOffset;
            var zFloor = zsFloor + squishOffset;

            // Compute simplectic honeycomb coordinates relative to rhombohedral origin.
            var xsFrac = xs - xsFloor;
            var ysFrac = ys - ysFloor;
            var zsFrac = zs - zsFloor;

            // Sum those together to get a value that determines which region we're in.
            var fracSum = xsFrac + ysFrac + zsFrac;

            // Positions relative to origin point.
            var dx0 = x - xFloor;
            var dy0 = y - yFloor;
            var dz0 = z - zFloor;

            var value = zero;

            // Check if we're inside the tetrahedron (3-Simplex) at (0,0,0)
            if (fracSum <= one)
            {
                // Contribution (0,0,0)
                value += Gradient.Get(seed, xsFloor, ysFloor, zsFloor, dx0, dy0, dz0);

                // Contribution (1,0,0)
                var dx1 = dx0 - one - SquishConstant_3D;
                var dy1 = dy0 - zero - SquishConstant_3D;
                var dz1 = dz0 - zero - SquishConstant_3D;
                value += Gradient.Get(seed, xsFloor + 1, ysFloor, zsFloor, dx1, dy1, dz1);

                // Contribution (0,1,0)
                var dx2 = dx0 - zero - SquishConstant_3D;
                var dy2 = dy0 - one - SquishConstant_3D;
                var dz2 = dz1;
                value += Gradient.Get(seed, xsFloor, ysFloor + 1, zsFloor, dx2, dy2, dz2);

                // Contribution (0,0,1)
                var dx3 = dx2;
                var dy3 = dy1;
                var dz3 = dz0 - one - SquishConstant_3D;
                value += Gradient.Get(seed, xsFloor, ysFloor, zsFloor + 1, dx3, dy3, dz3);
            }
            // Check if we're inside the tetrahedron (3-Simplex) at (1,1,1)
            else if (fracSum >= two)
            {
                // Contribution (1,1,0)
                var dx3 = dx0 - one - two*SquishConstant_3D;
                var dy3 = dy0 - one - two*SquishConstant_3D;
                var dz3 = dz0 - zero - two*SquishConstant_3D;
                value += Gradient.Get(seed, xsFloor + 1, ysFloor + 1, zsFloor, dx3, dy3, dz3);

                // Contribution (1,0,1)
                var dx2 = dx3;
                var dy2 = dy0 - zero - two*SquishConstant_3D;
                var dz2 = dz0 - one - two*SquishConstant_3D;
                value += Gradient.Get(seed, xsFloor + 1, ysFloor, zsFloor + 1, dx2, dy2, dz2);

                // Contribution (0,1,1)
                var dx1 = dx0 - zero - two*SquishConstant_3D;
                var dy1 = dy3;
                var dz1 = dz2;
                value += Gradient.Get(seed, xsFloor, ysFloor + 1, zsFloor + 1, dx1, dy1, dz1);

                // Contribution (1,1,1)
                dx0 = dx0 - one - three*SquishConstant_3D;
                dy0 = dy0 - one - three*SquishConstant_3D;
                dz0 = dz0 - one - three*SquishConstant_3D;
                value += Gradient.Get(seed, xsFloor + 1, ysFloor + 1, zsFloor + 1, dx0, dy0, dz0);
            }
            // Check if we're inside the octahedron (Rectified 3-Simplex) in between.
            else
            {
                // Contribution (1,0,0)
                var dx1 = dx0 - one - SquishConstant_3D;
                var dy1 = dy0 - zero - SquishConstant_3D;
                var dz1 = dz0 - zero - SquishConstant_3D;
                value += Gradient.Get(seed, xsFloor + 1, ysFloor, zsFloor, dx1, dy1, dz1);

                // Contribution (0,1,0)
                var dx2 = dx0 - zero - SquishConstant_3D;
                var dy2 = dy0 - one - SquishConstant_3D;
                var dz2 = dz1;
                value += Gradient.Get(seed, xsFloor, ysFloor + 1, zsFloor, dx2, dy2, dz2);

                // Contribution (0,0,1)
                var dx3 = dx2;
                var dy3 = dy1;
                var dz3 = dz0 - one - SquishConstant_3D;
                value += Gradient.Get(seed, xsFloor, ysFloor, zsFloor + 1, dx3, dy3, dz3);

                // Contribution (1,1,0)
                var dx4 = dx0 - one - two*SquishConstant_3D;
                var dy4 = dy0 - one - two*SquishConstant_3D;
                var dz4 = dz0 - zero - two*SquishConstant_3D;
                value += Gradient.Get(seed, xsFloor + 1, ysFloor + 1, zsFloor, dx4, dy4, dz4);

                // Contribution (1,0,1)
                var dx5 = dx4;
                var dy5 = dy0 - zero - two*SquishConstant_3D;
                var dz5 = dz0 - one - two*SquishConstant_3D;
                value += Gradient.Get(seed, xsFloor + 1, ysFloor, zsFloor + 1, dx5, dy5, dz5);

                // Contribution (0,1,1)
                var dx6 = dx0 - zero - two*SquishConstant_3D;
                var dy6 = dy4;
                var dz6 = dz5;
                value += Gradient.Get(seed, xsFloor, ysFloor + 1, zsFloor + 1, dx6, dy6, dz6);
            }

            return value/NormConstant_3D;
        }

        public static double Get(Seed seed, double x, double y, double z, double w)
        {
            throw new NotImplementedException();
        }
    }
}
namespace Procedural.Noise
{
    public static class Gradient
    {
        //Gradients for 2D. They approximate the directions to the
        //vertices of an octagon from the center.
        private static readonly sbyte[] Gradients2D =
        {
            5, 2, 2, 5,
            -5, 2, -2, 5,
            5, -2, 2, -5,
            -5, -2, -2, -5
        };

        //Gradients for 3D. They approximate the directions to the
        //vertices of a rhombicuboctahedron from the center, skewed so
        //that the triangular and square facets can be inscribed inside
        //circles of the same radius.
        private static readonly sbyte[] Gradients3D =
        {
            -11, 4, 4, -4, 11, 4, -4, 4, 11,
            11, 4, 4, 4, 11, 4, 4, 4, 11,
            -11, -4, 4, -4, -11, 4, -4, -4, 11,
            11, -4, 4, 4, -11, 4, 4, -4, 11,
            -11, 4, -4, -4, 11, -4, -4, 4, -11,
            11, 4, -4, 4, 11, -4, 4, 4, -11,
            -11, -4, -4, -4, -11, -4, -4, -4, -11,
            11, -4, -4, 4, -11, -4, 4, -4, -11
        };

        public static double Get(Seed seed, int xsFloor, int ysFloor, double dx, double dy)
        {
            const double zero = 0d;
            const double two = 2d;

            var attn = two - dx*dx - dy*dy;
            if (attn > zero)
            {
                return attn*attn*attn*attn*Extrapolate(seed, xsFloor, ysFloor, dx, dy);
            }

            return zero;
        }

        public static double Get(Seed seed, int xsFloor, int ysFloor, int zsFloor, double dx, double dy, double dz)
        {
            const double zero = 0d;
            const double two = 2d;

            var attn = two - dx*dx - dy*dy - dz*dz;
            if (attn > zero)
            {
                return attn*attn*attn*attn*Extrapolate(seed, xsFloor, ysFloor, zsFloor, dx, dy, dz);
            }

            return zero;
        }

        public static double Extrapolate(Seed seed, int xsFloor, int ysFloor, double dx, double dy)
        {
            var index = seed.Get(xsFloor, ysFloor) & 0x0E;
            return Gradients2D[index]*dx + Gradients2D[index + 1]*dy;
        }

        public static double Extrapolate(Seed seed, int xsFloor, int ysFloor, int zsFloor, double dx, double dy, double dz)
        {
            var index = seed.Get(xsFloor, ysFloor, zsFloor);
            return Gradients3D[index]*dx + Gradients3D[index + 1]*dy + Gradients3D[index + 2]*dz;
        }
    }
}
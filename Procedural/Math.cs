namespace Procedural
{
    public static class Math
    {
        public static float Round(float f)
        {
            return (float) System.Math.Round(f);
        }

        public static int FastFloor(double x)
        {
            var xi = (int) x;
            return x < xi ? xi - 1 : xi;
        }
    }
}
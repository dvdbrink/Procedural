namespace Procedural
{
    public struct Volume
    {
        public int depth;
        public int height;
        public int width;

        public Volume(int width, int height, int depth)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
        }
    }
}
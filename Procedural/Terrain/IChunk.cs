namespace Procedural.Terrain
{
    public interface IChunk
    {
        Vector3F Position { get; }
        int Width { get; }
        int Height { get; }
        int Depth { get; }
        bool Disposed { get; }
        float[] Voxels { get; set; }
        Vector3F[] Vertices { get; set; }
        int[] Indices { get; set; }

        void Update();
        void Draw();
        void Dispose();
    }
}
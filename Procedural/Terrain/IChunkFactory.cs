using Procedural.Noise;

namespace Procedural.Terrain
{
    public interface IChunkFactory
    {
        IChunk CreateChunk(Vector3F position, int width, int height, int depth);
        IChunkGenerator CreateChunkGenerator(Seed seed);
    }
}
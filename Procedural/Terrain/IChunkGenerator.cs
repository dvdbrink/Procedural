namespace Procedural.Terrain
{
    public interface IChunkGenerator
    {
        void Request(IChunk chunk);
        IChunk GetNext();
    }
}
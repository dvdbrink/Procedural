using System.Collections.Generic;
using System.Threading;
using Procedural.Noise;
using Procedural.Voxel;

namespace Procedural.Terrain
{
    public class ThreadedChunkGenerator : IChunkGenerator
    {
        private readonly Brownian _b;
        private readonly Queue<IChunk> _in;
        private readonly Queue<IChunk> _out;
        private readonly Seed _seed;

        public ThreadedChunkGenerator(Seed seed)
        {
            _in = new Queue<IChunk>();
            _out = new Queue<IChunk>();

            _seed = seed;
            _b = new Brownian(3, 0.01, 2, 0.8);

            var thread = new Thread(Run)
            {
                Priority = ThreadPriority.BelowNormal,
                IsBackground = true
            };
            thread.Start();
        }

        public void Request(IChunk chunk)
        {
            lock (_in)
            {
                _in.Enqueue(chunk);
                Monitor.Pulse(_in);
            }
        }

        public IChunk GetNext()
        {
            if (_out.Count <= 0) return null;
            lock (_out)
            {
                return _out.Dequeue();
            }
        }

        private void Run()
        {
            while (true)
            {
                IChunk chunk;
                lock (_in)
                {
                    while (_in.Count == 0)
                    {
                        Monitor.Wait(_in);
                    }
                    chunk = _in.Dequeue();
                }
                if (chunk != null) Generate(chunk);
            }
        }

        private void Generate(IChunk chunk)
        {
            var voxels = GenData(chunk.Position, chunk.Width, chunk.Height, chunk.Depth);
            var marchingCubes = new MarchingCubes(voxels, chunk.Width, chunk.Height, chunk.Depth, 0);

            chunk.Voxels = voxels;
            chunk.Vertices = marchingCubes.vertices.ToArray();
            chunk.Indices = marchingCubes.indices.ToArray();

            lock (_out)
            {
                _out.Enqueue(chunk);
            }
        }

        private float[] GenData(Vector3F origin, int width, int height, int depth)
        {
            var data = new float[width*height*depth];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    for (var z = 0; z < depth; z++)
                    {
                        var i = x + width*(y + height*z);

                        if (y == height - 1)
                        {
                            data[i] = -1;
                            continue;
                        }

                        if (y == 0)
                        {
                            data[i] = 1;
                            continue;
                        }

                        data[i] = (float) _b.Get(_seed, origin.x + x, origin.y + y, origin.z + z, OpenSimplex.Get);
                    }
                }
            }
            return data;
        }
    }
}
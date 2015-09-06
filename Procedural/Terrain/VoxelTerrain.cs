using Procedural.Noise;

namespace Procedural.Terrain
{
    public class VoxelTerrain : ITerrain
    {
        private readonly IChunkFactory _chunkFactory;
        private uint _chunkGeneratorCount;
        private IChunkGenerator[] _chunkGenerators;
        private IChunk[,,] _chunks;
        private Volume _chunkSize;
        private uint _nextChunkGeneratorIndex;
        private Vector3F _previousPosition;
        private Volume _terrainSize;

        public VoxelTerrain(IChunkFactory chunkFactory, Volume terrainSize, Volume chunkSize, uint chunkGeneratorCount,
            Vector3F pos)
        {
            _chunkFactory = chunkFactory;
            _terrainSize = terrainSize;
            _chunkSize = chunkSize;
            _previousPosition = GetFirstChunkWorldPosition(pos);

            InitializeChunkGenerators(chunkGeneratorCount);
            InitializeChunks();
        }

        public void Update(Vector3F position)
        {
            UpdateGeneratedChunks();
            UpdateChunkPositions(position);
        }

        public void Draw()
        {
            for (var x = 0; x < _terrainSize.width; ++x)
            {
                for (var y = 0; y < _terrainSize.height; ++y)
                {
                    for (var z = 0; z < _terrainSize.depth; ++z)
                    {
                        _chunks[x, y, z].Draw();
                    }
                }
            }
        }

        private void InitializeChunkGenerators(uint chunkGeneratorCount)
        {
            _chunkGeneratorCount = chunkGeneratorCount;
            _nextChunkGeneratorIndex = 0;
            _chunkGenerators = new IChunkGenerator[_chunkGeneratorCount];
            var seed = new Seed(CryptoRand.NextInt64());
            for (var i = 0; i < _chunkGeneratorCount; ++i)
            {
                _chunkGenerators[i] = _chunkFactory.CreateChunkGenerator(seed);
            }
        }

        private void InitializeChunks()
        {
            _chunks = new IChunk[_terrainSize.width, _terrainSize.height, _terrainSize.depth];
            for (var x = 0; x < _terrainSize.width; ++x)
            {
                for (var y = 0; y < _terrainSize.height; ++y)
                {
                    for (var z = 0; z < _terrainSize.depth; ++z)
                    {
                        var chunkPos = GetChunkPosition(x, y, z);
                        var chunk = CreateChunk(chunkPos);
                        QueueChunk(chunk);
                        _chunks[x, y, z] = chunk;
                    }
                }
            }
        }

        private void UpdateGeneratedChunks()
        {
            for (var i = 0; i < _chunkGeneratorCount; ++i)
            {
                IChunk chunk;
                while ((chunk = _chunkGenerators[i].GetNext()) != null)
                {
                    if (!chunk.Disposed)
                    {
                        chunk.Update();
                    }
                }
            }
        }

        private void UpdateChunkPositions(Vector3F position)
        {
            var newPosition = GetFirstChunkWorldPosition(position);

            if (newPosition.x < _previousPosition.x)
            {
                MoveLeft();
            }
            else if (newPosition.x > _previousPosition.x)
            {
                MoveRight();
            }

            if (newPosition.z < _previousPosition.z)
            {
                MoveForward();
            }
            else if (newPosition.z > _previousPosition.z)
            {
                MoveBackward();
            }
        }

        private void MoveForward()
        {
            _previousPosition.z -= _chunkSize.depth;

            for (var x = _terrainSize.width - 1; x >= 0; --x)
            {
                for (var y = _terrainSize.height - 1; y >= 0; --y)
                {
                    for (var z = _terrainSize.depth - 1; z >= 0; --z)
                    {
                        var chunkPos = GetChunkPosition(x, y, z);

                        if (z == 0)
                        {
                            var chunk = CreateChunk(chunkPos);
                            QueueChunk(chunk);
                            _chunks[x, y, z] = chunk;
                        }
                        else
                        {
                            if (z == _terrainSize.depth - 1)
                            {
                                _chunks[x, y, z].Dispose();
                            }
                            _chunks[x, y, z] = _chunks[x, y, z - 1];
                        }
                    }
                }
            }
        }

        private void MoveRight()
        {
            _previousPosition.x += _chunkSize.width;

            for (var x = 0; x < _terrainSize.width; ++x)
            {
                for (var y = 0; y < _terrainSize.height; ++y)
                {
                    for (var z = 0; z < _terrainSize.depth; ++z)
                    {
                        var chunkPos = GetChunkPosition(x, y, z);

                        if (x == _terrainSize.width - 1)
                        {
                            var chunk = CreateChunk(chunkPos);
                            QueueChunk(chunk);
                            _chunks[x, y, z] = chunk;
                        }
                        else
                        {
                            if (x == 0)
                            {
                                _chunks[x, y, z].Dispose();
                            }
                            _chunks[x, y, z] = _chunks[x + 1, y, z];
                        }
                    }
                }
            }
        }

        private void MoveBackward()
        {
            _previousPosition.z += _chunkSize.depth;

            for (var x = 0; x < _terrainSize.width; ++x)
            {
                for (var y = 0; y < _terrainSize.height; ++y)
                {
                    for (var z = 0; z < _terrainSize.depth; ++z)
                    {
                        var chunkPos = GetChunkPosition(x, y, z);

                        if (z == _terrainSize.depth - 1)
                        {
                            var chunk = CreateChunk(chunkPos);
                            QueueChunk(chunk);
                            _chunks[x, y, z] = chunk;
                        }
                        else
                        {
                            if (z == 0)
                            {
                                _chunks[x, y, z].Dispose();
                            }
                            _chunks[x, y, z] = _chunks[x, y, z + 1];
                        }
                    }
                }
            }
        }

        private void MoveLeft()
        {
            _previousPosition.x -= _chunkSize.width;

            for (var x = _terrainSize.width - 1; x >= 0; --x)
            {
                for (var y = _terrainSize.height - 1; y >= 0; --y)
                {
                    for (var z = _terrainSize.depth - 1; z >= 0; --z)
                    {
                        var chunkPos = GetChunkPosition(x, y, z);

                        if (x == 0)
                        {
                            var chunk = CreateChunk(chunkPos);
                            QueueChunk(chunk);
                            _chunks[x, y, z] = chunk;
                        }
                        else
                        {
                            if (x == _terrainSize.width - 1)
                            {
                                _chunks[x, y, z].Dispose();
                            }
                            _chunks[x, y, z] = _chunks[x - 1, y, z];
                        }
                    }
                }
            }
        }

        private IChunk CreateChunk(Vector3F pos)
        {
            return _chunkFactory.CreateChunk(pos, _chunkSize.width + 1, _chunkSize.height, _chunkSize.depth + 1);
        }

        private void QueueChunk(IChunk chunk)
        {
            _chunkGenerators[_nextChunkGeneratorIndex].Request(chunk);
            _nextChunkGeneratorIndex = (_nextChunkGeneratorIndex + 1)%_chunkGeneratorCount;
        }

        private Vector3F GetChunkPosition(int x, int y, int z)
        {
            return new Vector3F(_previousPosition.x + (x*_chunkSize.width), 0,
                _previousPosition.z + (z*_chunkSize.depth));
        }

        private Vector3F GetFirstChunkWorldPosition(Vector3F origin)
        {
            var worldPos = GetChunkWorldPosition(origin);
            return new Vector3F(worldPos.x - Math.Round(_terrainSize.width/2f)*_chunkSize.width, 0,
                worldPos.z - Math.Round(_terrainSize.depth/2f)*_chunkSize.depth);
        }

        private Vector3F GetChunkWorldPosition(Vector3F origin)
        {
            var gridPos = GetChunkGridPosition(origin);
            return new Vector3F(gridPos.x*_chunkSize.width, 0, gridPos.z*_chunkSize.depth);
        }

        private Vector3F GetChunkGridPosition(Vector3F origin)
        {
            return new Vector3F(Math.FastFloor(origin.x/_chunkSize.width), 0, Math.FastFloor(origin.z/_chunkSize.depth));
        }
    }
}
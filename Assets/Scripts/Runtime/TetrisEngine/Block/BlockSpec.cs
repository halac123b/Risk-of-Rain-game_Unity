using System;
using System.Collections.Generic;
using UnityEngine;

namespace TetrisEngine.BlockPiece
{
    /// <summary>Struct to hold informationa about each block</summary>
    [Serializable]
    public struct BlockSpec
    {
        public string name;
        public Color color;
        public List<int> serializedBlockPositions;
        // Initial position of the block
        public Vector2Int[] initialPosition;
    }
}

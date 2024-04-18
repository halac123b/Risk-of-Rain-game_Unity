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
        // Each block has shape of 5x5, each square is represented by a 1 or 0
        // This list store the block shape in 4 rotation state
        // Each state store 2D array of 1s and 0s
        public List<int> serializedBlockPositions;
        // Initial position of the block
        public Vector2Int[] initialPosition;
    }
}

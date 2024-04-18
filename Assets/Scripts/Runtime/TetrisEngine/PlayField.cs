using System;
using System.Collections;
using System.Collections.Generic;
using TetrisEngine.BlockPiece;
using UnityEngine;

namespace TetrisEngine
{
    /// <summary>This class is a representation of the field in the engine</summary>
    // Stores the field spots as a bidimensional array of 0s and 1s
    // Where 0 means empty slot and 1 means filled spot
    public class Playfield
    {
        internal enum SpotState { EMPTY_SPOT = 0, FILLED_SPOT = 1 }

        // Size of playfield
        public const int WIDTH = 10;
        public const int HEIGHT = 22;

        // Actions when some events on PlayField
        public Action OnCurrentPieceReachBottom;
        public Action OnGameOver;
        public Action<int> OnDestroyLine;

        // Array to store state the playfield
        private int[][] mPlayfield = new int[WIDTH][];
        private BlockSpawner mSpawner;
        private Block mCurrentBlock;
    }
}

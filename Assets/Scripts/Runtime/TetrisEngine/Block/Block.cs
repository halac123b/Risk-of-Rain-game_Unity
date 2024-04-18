using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TetrisEngine.BlockPiece
{
    /// <summary>Class present a block</summary>
    // Each block requires a BlockSpec to hold its information
    public class Block
    {
        // Max area of a block is 5x5
        public const int BLOCK_AREA = 5;
        public const int BLOCK_ROTATIONS = 4;

        // Action called when rotate or move the block
        public Action OnChangePosition;
        public Action OnChangeRotation;

        public bool IsLocked;
        public string Name { get; private set; }
        public Color Color { get; private set; }

        // Position information
        public int[][][] BlockPositions { get; private set; }
        private Vector2Int[] initialPosition;
        private Vector2Int mCurrentPosition;
        public Vector2Int CurrentPosition
        {
            // Set new position and call OnChangePosition
            set
            {
                mCurrentPosition = value;
                if (!IsLocked)
                    OnChangePosition?.Invoke();
            }
            get
            {
                return mCurrentPosition;
            }
        }

        // Rotation information
        private int mCurrentRotation;
        public int CurrentRotation
        {
            set
            {
                mCurrentRotation = value;
                OnChangeRotation?.Invoke();
            }
            get
            {
                return mCurrentRotation;
            }
        }

        public Block(BlockSpec specs)
        {
            Name = specs.name;
            Color = specs.color;
            initialPosition = specs.initialPosition;

            // Check if the shape of the block is correct
            if (specs.serializedBlockPositions.Count != BLOCK_ROTATIONS * BLOCK_AREA * BLOCK_AREA)
                throw new Exception(
                    string.Format(
                        "The layout of piece {0} is wrong in Json file. It must have {1} rotations of {2}x{3} grid.",
                        Name, BLOCK_ROTATIONS, BLOCK_AREA, BLOCK_AREA));

            int position = 0;
            BlockPositions = new int[BLOCK_ROTATIONS][][];
            for (int i = 0; i < BlockPositions.Length; i++)
            {
                BlockPositions[i] = new int[BLOCK_AREA][];
                for (int j = 0; j < BlockPositions[i].Length; j++)
                {
                    BlockPositions[i][j] = new int[BLOCK_AREA];
                    for (int k = 0; k < BlockPositions[i][j].Length; k++)
                    {
                        // Check if value of a cell is valid by 1 and 0
                        if (specs.serializedBlockPositions[position] != (int)Playfield.SpotState.EMPTY_SPOT &&
                                specs.serializedBlockPositions[position] != (int)Playfield.SpotState.FILLED_SPOT)
                        {
                            throw new Exception(
                            string.Format(
                                "The layout of piece {0} is wrong in Json file. It contains '{1}' when only {2}s and {3}s are supported.",
                                Name,
                                specs.serializedBlockPositions[position],
                                (int)Playfield.SpotState.EMPTY_SPOT,
                                (int)Playfield.SpotState.FILLED_SPOT));
                        }
                        // If value is valid, store it
                        BlockPositions[i][j][k] = specs.serializedBlockPositions[position++];
                    }
                }
            }
        }
        // Next rotation state in 4 states
        public int NextRotation { get { return CurrentRotation + 1 > 3 ? 0 : CurrentRotation + 1; } }
        public int PreviousRotation { get { return CurrentRotation - 1 < 0 ? 3 : CurrentRotation - 1; } }

        // Get value 1 or 0 of a cell in a block
        public int GetBlockType(int rotation, int x, int y)
        {
            return BlockPositions[rotation][x][y];
        }

        public Vector2Int GetInitialPosition(int rotation)
        {
            return initialPosition[rotation];
        }

        // Check if this is a cell
        public bool ValidBlock(int rotation, int x, int y)
        {
            return BlockPositions[rotation][x][y] != 0;
        }
    }
}

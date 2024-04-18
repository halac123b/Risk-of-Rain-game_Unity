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
                }
            }
        }
    }
}

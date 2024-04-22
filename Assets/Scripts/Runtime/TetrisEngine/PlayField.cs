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
        private GameSettings mGameSettings;

        public Playfield(GameSettings gameSettings)
        {
            mGameSettings = gameSettings;
            // Create array store grid information
            for (int i = 0; i < WIDTH; i++)
            {
                mPlayfield[i] = new int[HEIGHT];
            }
            ResetGame();
            mSpawner = new BlockSpawner(mGameSettings.ControledRandomMode, mGameSettings.pieces);
        }

        public void ResetGame()
        {
            mCurrentBlock = null;
            // Reset all spots to empty
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    mPlayfield[i][j] = (int)SpotState.EMPTY_SPOT;
                }
            }
            if (mGameSettings.DebugMode)
            {
                Debug.Log("RESETING GAME");
            }
        }

        public Block CreateBlock()
        {
            // Get a new block from spanwer
            mCurrentBlock = mSpawner.GetRandomBlock();
            // Random rotation
            int rotation = RandomGenerator.random.Next(0, mCurrentBlock.BlockPositions.GetLength(0));
            // Get position according to rotation
            Vector2Int position = mCurrentBlock.GetInitialPosition(rotation);
            // Set block position to the center of the playfield
            position.x += WIDTH / 2;
            mCurrentBlock.CurrentPosition = position;
            mCurrentBlock.CurrentRotation = rotation;

            if (mGameSettings.DebugMode)
            {
                Debug.Log($"Creating block: {mCurrentBlock.Name}");
            }
            return mCurrentBlock;
        }

        // If possible, make the current piece fall, else locks the piece in the playfield and check for full lines
        // Also checks for GameOver
        public void Step()
        {
            // Check if the movement is possible (the y position is + 1)
            if (IsPossibleMovement(mCurrentBlock.CurrentPosition.x, mCurrentBlock.CurrentPosition.y + 1, mCurrentBlock, mCurrentBlock.CurrentRotation))
            {
                mCurrentBlock.CurrentPosition = new Vector2Int(mCurrentBlock.CurrentPosition.x, mCurrentBlock.CurrentPosition.y + 1);
            }
            else
            {
                PlaceBlock(mCurrentBlock);
                DeletePossibleLines();

                if (IsGameOver())
                {
                    if (mGameSettings.DebugMode)
                    {
                        Debug.Log("GAME OVER");
                    }
                    OnGameOver.Invoke();
                    return;
                }
                if (mGameSettings.DebugMode)
                {
                    Dump();
                }
                OnCurrentPieceReachBottom.Invoke();
            }
        }

        // Check if the movemente is valid before it occours.
        public bool IsPossibleMovement(int x, int y, Block block, int rotation)
        {
            // i1, j1: coordiante on global playfield, i2, j2: coordinate on block
            for (int i1 = x, i2 = 0; i1 < x + Block.BLOCK_AREA; i1++, i2++)
            {
                for (int j1 = y, j2 = 0; j1 < y + Block.BLOCK_AREA; j1++, j2++)
                {
                    // When the block's postion is outside the playfield
                    if (i1 < 0 || i1 > WIDTH - 1 || j1 > HEIGHT - 1)
                    {
                        // And the block is not empty
                        if (block.ValidBlock(rotation, j2, i2))
                        {
                            return false;
                        }
                    }

                    if (j1 >= 0)
                    {
                        // If the cell is valid and next step to go is already filled
                        if (block.ValidBlock(rotation, j2, i2) && !IsFreeBlock(i1, j1))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>If the spot is 0, returns true</summary>
        private bool IsFreeBlock(int pX, int pY)
        {
            return mPlayfield[pX][pY] == (int)SpotState.EMPTY_SPOT;
        }

        /// <summary>Places when a piece either reaches bottom, either collides in a way that is not possible to go lower anymore.<br/>
        /// Update the state of the playfield
        /// </summary>
        private void PlaceBlock(Block block)
        {
            // Similarly, we need 2 for loop to iterate through the block
            for (int i1 = block.CurrentPosition.x, i2 = 0; i1 < block.CurrentPosition.x + Block.BLOCK_AREA; i1++, i2++)
            {
                for (int j1 = block.CurrentPosition.y, j2 = 0; j1 < block.CurrentPosition.y + Block.BLOCK_AREA; j1++, j2++)
                {
                    if (block.ValidBlock(block.CurrentRotation, j2, i2) && InBounds(i1, j1))
                    {
                        mPlayfield[i1][j1] = (int)SpotState.FILLED_SPOT;
                    }
                }
            }
        }

        /// <summary>Check if (x, y) is inside the playfield</summary>
        private bool InBounds(int x, int y)
        {
            return x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT;
        }

        /// <summary>Checks for full lines, if any, deletes it</summary>
		private void DeletePossibleLines()
        {
            // Iterate through each line, counting the number of filled spots
            for (int j = 0; j < HEIGHT; j++)
            {
                int i = 0;
                while (i < WIDTH)
                {
                    if (mPlayfield[i][j] != (int)SpotState.FILLED_SPOT) break;
                    i++;
                }

                if (i == WIDTH)
                {
                    DeleteLine(j);
                }
            }
        }

        /// <summary>
        /// Deletes a line in the playfield<br/>
        /// Also makes the pieces below that line to move 1 spot down
        /// </summary>
        private void DeleteLine(int y)
        {
            if (mGameSettings.DebugMode)
            {
                Debug.Log($"DESTROYING LINE: {y}");
            }
            for (int j = y; j > 0; j--)
            {
                for (int i = 0; i < WIDTH; i++)
                {
                    mPlayfield[i][j] = mPlayfield[i][j - 1];
                }
            }
            OnDestroyLine.Invoke(y);
        }

        /// <summary> Checks the first line for 1s, if any, Game Over is true </summary>
		public bool IsGameOver()
        {
            for (int i = 0; i < WIDTH; i++)
            {
                if (mPlayfield[i][0] == (int)SpotState.FILLED_SPOT)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Method used to debug the field, it logs the spots</summary>
		public void Dump()
        {
            string playfield = "";
            for (int i = 0; i < HEIGHT; i++)
            {
                for (int j = 0; j < WIDTH; j++)
                {
                    playfield += string.Format("[{0}]", mPlayfield[j][i]);
                }
                playfield += Environment.NewLine;
            }

            Debug.Log(playfield);
        }
    }
}

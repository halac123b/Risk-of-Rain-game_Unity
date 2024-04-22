using System.Collections.Generic;
using TetrisEngine.BlockPiece;
using UnityEngine;

namespace TetrisEngine
{
  /// <summary>Class to hold game settings</summary>
  public class GameSettings
  {
    private const float MIN_TIME_TO_STEP = 0.01f;
    // Time to step the block down
    public float TimeToStep;
    // Points to receive when break a line
    public int PointsByBreakingLine;
    // Control random mode of BlockSpawner
    public bool ControledRandomMode;

    public bool DebugMode;

    // Input key to control the block
    public KeyCode MoveRightKey;
    public KeyCode MoveLeftKey;
    public KeyCode MoveDownKey;
    public KeyCode RotateRightKey;
    public KeyCode RotateLeftKey;

    // List of blocks to be spawned in the game
    public List<BlockSpec> pieces;

    public void CheckValidSettings()
    {
      if (TimeToStep < MIN_TIME_TO_STEP)
        throw new System.Exception(string.Format("timeToStep inside GameSettings.json must be higher than {0}", MIN_TIME_TO_STEP));

      if (PointsByBreakingLine < 0)
        throw new System.Exception("pointsByBreakingLine inside GameSettings.json must be higher or equal 0");

      if (MoveRightKey == KeyCode.None)
        throw new System.Exception("moveRightKey inside GameSettings.json must different than None");
      if (MoveLeftKey == KeyCode.None)
        throw new System.Exception("moveLeftKey inside GameSettings.json must different than None");
      if (MoveDownKey == KeyCode.None)
        throw new System.Exception("moveDownKey inside GameSettings.json must different than None");
      if (RotateRightKey == KeyCode.None)
        throw new System.Exception("rotateRightKey inside GameSettings.json must different than None");
      if (RotateLeftKey == KeyCode.None)
        throw new System.Exception("rotateLeftKey inside GameSettings.json must different than None");
    }
  }
}
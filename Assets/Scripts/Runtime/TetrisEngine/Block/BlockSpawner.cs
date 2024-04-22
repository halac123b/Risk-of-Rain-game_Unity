using System.Collections.Generic;

namespace TetrisEngine.BlockPiece
{
  /// <summary>Class responsable for generating random blocks</summary>
  public class BlockSpawner
  {
    // List contains all blocks spec can be spawned
    private List<BlockSpec> mAllBlocks = new List<BlockSpec>();
    // List contains block spec remaining to be spawned
    private List<BlockSpec> mAvailableBlocks = new List<BlockSpec>();

    // If mControledRandom is true, it makes sure no piece is choosen twice befere all other types are choosen
    // If not, a random type is choosen
    private bool mControledRandom;
    public BlockSpawner(bool controledRandom, List<BlockSpec> allTetriminos)
    {
      mAllBlocks = allTetriminos;
      mControledRandom = controledRandom;
    }

    public Block GetRandomBlock()
    {
      if (mControledRandom)
      {
        // If the list is empty, it creates a new one with all the blocks inside the project and chooses one to return
        if (mAvailableBlocks.Count == 0)
        {
          mAvailableBlocks = GetFullBlockBaseList();
        }
        // Get a random block from the list
        var blockSpecs = mAvailableBlocks[RandomGenerator.random.Next(0, mAvailableBlocks.Count)];
        mAvailableBlocks.Remove(blockSpecs);
        return new Block(blockSpecs);
      }
      return new Block(mAllBlocks[RandomGenerator.random.Next(0, mAllBlocks.Count)]);
    }

    private List<BlockSpec> GetFullBlockBaseList()
    {
      var allBlocks = new List<BlockSpec>(mAllBlocks);
      return allBlocks;
    }
  }
}
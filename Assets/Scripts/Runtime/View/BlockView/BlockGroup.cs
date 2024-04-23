using UnityEngine;
using System.Collections.Generic;
using System;
using pooling;
using TetrisEngine.BlockPiece;

/// <summary>
/// This class represents a full group of block<br/>
/// Receives information from engine about the positions of the blocks and its color<br/>
/// Its a PoolingObject, so no object is deleted and few are created<br/>
///</summary>
public class BlockGroup : PoolingObject
{
    public override string objectName
    {
        get
        {
            return "BlockGroup";
        }
    }

    public Block currentBlock;

    public bool IsLocked
    {
        get
        {
            return currentBlock.IsLocked;
        }
    }

    public bool IsDestroyed;
    public Action<BlockGroup> OnDestroyBlockGroup;
    public Pooling<BlockSingle> blockPool;

    private readonly List<BlockSingle> mPieces = new();

    private Color mBlockColor;
    private RectTransform mRectTransform;
    private Vector2Int mBlockPosition;

    private void Awake()
    {
        mRectTransform = GetComponent<RectTransform>();
    }

    //overrides the Collect method to make sure its anchored in the right place
    public override void OnCollect()
    {
        base.OnCollect();

        IsDestroyed = false;

        mRectTransform.anchorMin = Vector2.zero;
        mRectTransform.anchorMax = Vector2.one;
        mRectTransform.offsetMin = Vector2.zero;
        mRectTransform.offsetMax = Vector2.zero;
    }

    // Receives a Block from the engine and creates in the view
}
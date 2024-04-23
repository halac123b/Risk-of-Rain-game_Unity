using UnityEngine;
using pooling;

// Single block from pieces in view
public class BlockSingle : PoolingObject
{
    public override string objectName
    {
        get
        {
            return "BlockSingle";
        }
    }

    public Vector2Int Position { get; private set; }
    private SpriteRenderer mSpriteRenderer;

    public void Awake()
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        mSpriteRenderer.color = color;
    }

    //Positioning the block
    public void MoveTo(int x, int y)
    {
        Position = new Vector2Int(x, y);
        transform.localPosition = new Vector3(x, -y, 0);
    }
}
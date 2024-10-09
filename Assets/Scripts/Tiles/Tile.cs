using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer m_renderer;
    public bool isWalkable = true; 
    public float gCost;
    public float hCost;
    public float fCost
    {
        get { return gCost + hCost; }
    }
    public Tile parent;
    private void Awake()
    {
        m_renderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnMouseEnter()
    {
        HighlightTile(Color.green);
        TileManager.Instance.ActiveTile = this;
    }

    private void OnMouseExit()
    {
        SetTileDefault();
    }

    public void HighlightTile(Color color)
    {
        m_renderer.color = color;
    }

    public void SetTileDefault()
    {
        m_renderer.color = Color.white;
    }
}
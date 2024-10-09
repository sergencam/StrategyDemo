using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer m_renderer;
    [HideInInspector] public bool isWalkable = true; 
    [HideInInspector] public float GCost;
    [HideInInspector] public float HCost;
    public float FCost => GCost + HCost;
    [HideInInspector]public Tile parent;
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
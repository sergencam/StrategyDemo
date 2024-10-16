using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;
    [SerializeField] private int m_width, m_height;
    public float tileSize;
    [SerializeField] private Tile m_tilePrefab;
    private Tile m_activeTile;
    private List<Tile> m_allTiles = new();
    private Pathfinder m_pathfinder = new();

    public Tile ActiveTile
    {
        get => m_activeTile;
        set => m_activeTile = value;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        GenerateTile();
        SetCameraPosition();
    }

    //Produces as many tiles as the given width and height value
    private void GenerateTile()
    {
        for (int x = 0; x < m_width; x++)
            for (int y = 0; y < m_height; y++)
                m_allTiles.Add(Instantiate(m_tilePrefab, new Vector3(x*tileSize, y*tileSize, 0), Quaternion.identity, transform));
    }

    //Sets camera position middle of the created tiles after tiles created
    private void SetCameraPosition()
    {
        Camera cam = Camera.main;
        Vector3 camPos = cam.transform.position;
        camPos.x = m_width*tileSize / 2f;
        camPos.y = m_height*tileSize / 2f;
        cam.transform.position = camPos;
    }
    
    //Deactivates tiles if given values cover it
    public void DisableCoveredTiles(Vector3 position, Vector2 size)
    {
        foreach (var tile in m_allTiles)
        {
            Vector3 tilePosition = tile.transform.position;

            if (tilePosition.x >= position.x - size.x / 2f &&
                tilePosition.x <= position.x + size.x / 2f &&
                tilePosition.y >= position.y - size.y / 2f &&
                tilePosition.y <= position.y + size.y / 2f)
            {
                tile.gameObject.SetActive(false);
                tile.isWalkable = false;
            }
        }
    }
    
    //Activates tiles if given values cover it
    public void EnableCoveredTiles(Vector3 position, Vector2 size)
    {
        foreach (var tile in m_allTiles)
        {
            Vector3 tilePosition = tile.transform.position;

            if (tilePosition.x >= position.x - size.x / 2f &&
                tilePosition.x <= position.x + size.x / 2f &&
                tilePosition.y >= position.y - size.y / 2f &&
                tilePosition.y <= position.y + size.y / 2f)
            {
                tile.gameObject.SetActive(true);
                tile.isWalkable = true;
                tile.SetTileDefault();
            }
        }
    }
    
    //Highlights tiles if given values cover it
    public void HighlightCoveredTiles(Vector3 position, Vector2 size, Color highlightColor)
    {
        foreach (var tile in m_allTiles)
        {
            Vector3 tilePosition = tile.transform.position;

            if (tilePosition.x >= position.x - size.x / 2f &&
                tilePosition.x <= position.x + size.x / 2f &&
                tilePosition.y >= position.y - size.y / 2f &&
                tilePosition.y <= position.y + size.y / 2f)
            {
                tile.HighlightTile(highlightColor);
            }
            else
                tile.SetTileDefault();
        }
    }

    //Gets the closest tile in all tiles by given vector3 value 
    private Tile GetClosestTile(Vector3 pos)
    {
        Tile closestTile = null;
        float closestDistance = float.MaxValue;

        foreach (Tile tile in m_allTiles)
        {
            if(!tile.isWalkable)continue;
            Vector3 tilePosition = tile.transform.position;
            float distance = Vector3.Distance(tilePosition, pos);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = tile;
            }
        }
        return closestTile;
    }

    //Will find the start tile by given position and create path to selected tile
    public List<Tile> FindPath(Vector3 startPos)
    {
        Tile startTile = GetClosestTile(startPos);
        return m_pathfinder.FindPath(startTile, m_activeTile, tileSize, m_allTiles);
    }
}
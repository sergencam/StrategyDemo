using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    // Finds the shortest path from startTile to targetTile
    public List<Tile> FindPath(Tile startTile, Tile targetTile, float tileSize, List<Tile> allTiles)
    {
        List<Tile> openList = new List<Tile>();
        HashSet<Tile> closedList = new HashSet<Tile>();

        openList.Add(startTile);

        while (openList.Count > 0)
        {
            Tile currentTile = GetTileWithLowestFScore(openList);

            if (currentTile == targetTile)
                return RetracePath(startTile, targetTile);

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            foreach (Tile neighbor in GetNeighbors(currentTile, tileSize, allTiles))
            {
                if (!neighbor.isWalkable || closedList.Contains(neighbor))
                    continue;

                float newCostToNeighbor = currentTile.GCost + GetDistance(currentTile, neighbor);
                if (newCostToNeighbor < neighbor.GCost || !openList.Contains(neighbor))
                {
                    neighbor.GCost = newCostToNeighbor;
                    neighbor.HCost = GetDistance(neighbor, targetTile);
                    neighbor.parent = currentTile;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }
        return null;
    }

    // Retraces the path from endTile to startTile
    private List<Tile> RetracePath(Tile startTile, Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;

        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Reverse();
        return path;
    }

    // Finds the tile with the lowest F score from the open list
    private Tile GetTileWithLowestFScore(List<Tile> openList)
    {
        Tile lowestFScoreTile = openList[0];
        foreach (Tile tile in openList)
            if (tile.FCost < lowestFScoreTile.FCost)
                lowestFScoreTile = tile;
        return lowestFScoreTile;
    }

    //Gets walkable neighbor tiles of the current tile
    private List<Tile> GetNeighbors(Tile currentTile, float tileSize, List<Tile> allTiles)
    {
        List<Tile> neighbors = new List<Tile>();

        float currentX = currentTile.transform.position.x;
        float currentY = currentTile.transform.position.y;

        Vector2[] directions = {
            new(tileSize, 0),
            new(-tileSize, 0),
            new(0, tileSize),
            new(0, -tileSize)
        };

        foreach (Vector2 dir in directions)
        {
            Vector2 neighborPosition = new Vector2(currentX + dir.x, currentY + dir.y);

            Tile neighbor = allTiles.Find(tile => 
                Mathf.Approximately(tile.transform.position.x, neighborPosition.x) &&
                Mathf.Approximately(tile.transform.position.y, neighborPosition.y)
            );

            if (neighbor != null && neighbor.isWalkable)
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    // Calculates the distance between two tiles using Manhattan distance
    private float GetDistance(Tile tileA, Tile tileB)
    {
        return Mathf.Abs(tileA.transform.position.x - tileB.transform.position.x) + Mathf.Abs(tileA.transform.position.y - tileB.transform.position.y);
    }
}
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class PathFinding {

    class Node {

        public Node parent;
        public Vector2Int pos;

        public Node(Vector2Int pos, Node parent = null) {
            this.pos = pos;
            this.parent = parent;
        }

    }
    Tilemap tilemap;
    LayerMask path;

    public PathFinding(Tilemap tilemap, LayerMask path) {
        this.tilemap = tilemap;
        this.path = path;
    }

    static readonly List<Vector2Int> directions = new List<Vector2Int> {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
    };

    public List<Vector2> FindPath(Vector2 start, Vector2 end)
    {

        Vector2Int startInt = Vector2Int.FloorToInt(start);
        Vector2Int endInt = Vector2Int.FloorToInt(end);

        Queue<Node> queue = new Queue<Node>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        Node startingNode = new Node(startInt);
        queue.Enqueue(startingNode);
        visited.Add(startInt);

        Node endingNode = null;

        bool foundEnd = false;
        while (queue.Count > 0 && !foundEnd)
        {
            Node current = queue.Dequeue();

            foreach (Vector2Int direction in directions)
            {
                Vector2Int next = current.pos + direction;

                if (visited.Contains(next)) continue;
                if (!IsPathTile(next)) continue;

                Node nextNode = new Node(next, current);
                queue.Enqueue(nextNode);
                visited.Add(next);

                if (next == endInt) {
                    endingNode = nextNode;
                    foundEnd = true;
                    break;
                }


            }
        }

        List<Vector2> path = new List<Vector2>();
        while(endingNode != null && endingNode.parent != null) {
            path.Add(endingNode.pos + Vector2.one * .5f);
            endingNode = endingNode.parent;
        }
        path.Reverse();

        return path;
    }

    bool IsPathTile(Vector2 position)
    {
        return tilemap.HasTile(new Vector3Int((int)position.x, (int)position.y, 0));
    }

    public List<Vector2> GetPath(Vector3 start, Vector3 end)
    {   

        Vector2 startTile = FindClosestPathTile(start);
        Vector2 endTile = FindClosestPathTile(end);
        List<Vector2> pathPoints = FindPath(startTile, endTile);
        pathPoints.Insert(0, start);
        pathPoints.Add(end);

        return pathPoints;
    }

    private Vector2 FindClosestPathTile(Vector3 startPosition)
    {
        Vector2 closestTilePos = Vector2.zero;
        float closestDistance = float.MaxValue;

        foreach (Vector2 direction in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, Mathf.Infinity, path);
            
            if (hit.collider != null)
            {
                Vector2 hitPos = hit.point + direction * .5f;

                float distance = Vector2.Distance(startPosition, hit.point);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTilePos = hitPos;
                }
            }
        }

        return closestTilePos;
    }

}
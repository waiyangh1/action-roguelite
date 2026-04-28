using UnityEngine;
using System.Collections.Generic;

public class GridAStar : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2 gridWorldSize = new Vector2(30, 20);
    [SerializeField] private float nodeRadius = 0.5f;
    [SerializeField] private LayerMask obstacleMask;

    private Node[,] _grid;
    private float _nodeDiameter;
    private int _gridSizeX, _gridSizeY;
    public int gridSizeY => _gridSizeY;          // exposed for time‑slicing

    private List<Node> _openSet = new List<Node>();
    private HashSet<Node> _closedSet = new HashSet<Node>();

    private int _searchID;

    private static GridAStar _instance;
    public static GridAStar Instance => _instance;

    private void Awake()
    {
        _instance = this;
        _nodeDiameter = nodeRadius * 2f;
        _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);
        CreateGrid();
    }

    private void CreateGrid()
    {
        _grid = new Node[_gridSizeX, _gridSizeY];
        Vector2 worldBottomLeft = (Vector2)transform.position
                                  - Vector2.right * gridWorldSize.x / 2
                                  - Vector2.up * gridWorldSize.y / 2;

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft
                    + Vector2.right * (x * _nodeDiameter + nodeRadius)
                    + Vector2.up * (y * _nodeDiameter + nodeRadius);
                bool walkable = !Physics2D.OverlapCircle(worldPoint, nodeRadius, obstacleMask);
                _grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    /// <summary>Full one‑shot refresh (use only for initialisation or debugging).</summary>
    public void RefreshGrid()
    {
        for (int x = 0; x < _gridSizeX; x++)
            for (int y = 0; y < _gridSizeY; y++)
                _grid[x, y].walkable =
                    !Physics2D.OverlapCircle(_grid[x, y].worldPosition, nodeRadius, obstacleMask);
    }

    /// <summary>Time‑sliced refresh: scan only the given rows (y from startY inclusive, up to count rows).</summary>
    public void RefreshGridRows(int startY, int count)
    {
        int endY = Mathf.Min(startY + count, _gridSizeY);
        for (int y = startY; y < endY; y++)
            for (int x = 0; x < _gridSizeX; x++)
                _grid[x, y].walkable =
                    !Physics2D.OverlapCircle(_grid[x, y].worldPosition, nodeRadius, obstacleMask);
    }

    /// <summary>Finds a path. No grid refresh inside. Target blocked? Falls back to spiral search for nearest walkable.</summary>
    public List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Node startNode = WorldToNode(startPos);
        Node targetNode = WorldToNode(targetPos);

        if (targetNode != null && !targetNode.walkable)
        {
            targetNode = FindClosestWalkableNode(targetNode);   // now uses spiral search
            if (targetNode == null)
                return null;
        }

        if (startNode == null || targetNode == null)
            return null;

        _openSet.Clear();
        _closedSet.Clear();
        _searchID++;

        _openSet.Add(startNode);
        startNode.SetFresh(_searchID);
        startNode.hCost = GetDistance(startNode, targetNode);

        while (_openSet.Count > 0)
        {
            Node currentNode = _openSet[0];
            for (int i = 1; i < _openSet.Count; i++)
                if (_openSet[i].fCost < currentNode.fCost ||
                   (_openSet[i].fCost == currentNode.fCost && _openSet[i].hCost < currentNode.hCost))
                    currentNode = _openSet[i];

            _openSet.Remove(currentNode);
            _closedSet.Add(currentNode);

            if (currentNode == targetNode)
                return RetracePath(startNode, targetNode);

            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || _closedSet.Contains(neighbour))
                    continue;

                if (neighbour.lastSearchID != _searchID)
                    neighbour.SetFresh(_searchID);

                int newCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newCost < neighbour.gCost || !_openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!_openSet.Contains(neighbour))
                        _openSet.Add(neighbour);
                }
            }
        }
        return null;
    }

    // ----- Spiral search: no full grid scan! -----
    private Node FindClosestWalkableNode(Node originalTarget)
    {
        int cx = originalTarget.gridX;
        int cy = originalTarget.gridY;

        if (originalTarget.walkable)
            return originalTarget;

        int maxRadius = Mathf.Max(_gridSizeX, _gridSizeY);   // safe upper bound
        for (int d = 1; d <= maxRadius; d++)
        {
            // Check the full square ring where max(|dx|,|dy|) == d
            for (int dx = -d; dx <= d; dx++)
            {
                int x = cx + dx;
                if (x < 0 || x >= _gridSizeX) continue;

                int absDx = Mathf.Abs(dx);
                if (absDx == d)
                {
                    // All y from -d to d
                    for (int dy = -d; dy <= d; dy++)
                    {
                        int y = cy + dy;
                        if (y >= 0 && y < _gridSizeY && _grid[x, y].walkable)
                            return _grid[x, y];
                    }
                }
                else
                {
                    // |dx| < d => only y = cy - d or cy + d
                    int y1 = cy - d;
                    if (y1 >= 0 && _grid[x, y1].walkable) return _grid[x, y1];
                    int y2 = cy + d;
                    if (y2 < _gridSizeY && _grid[x, y2].walkable) return _grid[x, y2];
                }
            }
        }
        return null;   // no walkable node anywhere (should never happen)
    }

    // ... (rest of the helper methods unchanged: RetracePath, GetNeighbours, WorldToNode, GetDistance, Node class)
    private List<Vector2> RetracePath(Node start, Node end)
    {
        List<Vector2> path = new List<Vector2>();
        Node current = end;
        while (current != start)
        {
            path.Add(current.worldPosition);
            current = current.parent;
        }
        path.Reverse();
        return path;
    }

    private List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                    neighbours.Add(_grid[checkX, checkY]);
            }
        }
        return neighbours;
    }

    private Node WorldToNode(Vector2 worldPosition)
    {
        Vector2 bottomLeft = (Vector2)transform.position
                             - Vector2.right * gridWorldSize.x / 2
                             - Vector2.up * gridWorldSize.y / 2;
        Vector2 localPos = worldPosition - bottomLeft;
        float percentX = Mathf.Clamp01(localPos.x / gridWorldSize.x);
        float percentY = Mathf.Clamp01(localPos.y / gridWorldSize.y);
        int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);
        return _grid[x, y];
    }

    private int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);
        return (dstX > dstY)
            ? 14 * dstY + 10 * (dstX - dstY)
            : 14 * dstX + 10 * (dstY - dstX);
    }

    public class Node
    {
        public bool walkable;
        public Vector2 worldPosition;
        public int gridX, gridY;
        public int gCost, hCost;
        public int fCost => gCost + hCost;
        public Node parent;
        public int lastSearchID;

        public Node(bool walkable, Vector2 worldPos, int x, int y)
        {
            this.walkable = walkable;
            worldPosition = worldPos;
            gridX = x;
            gridY = y;
        }

        public void SetFresh(int searchID)
        {
            gCost = 0;
            hCost = 0;
            parent = null;
            lastSearchID = searchID;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_grid == null) return;
        foreach (Node n in _grid)
        {
            Gizmos.color = n.walkable ? Color.green : Color.red;
            Gizmos.DrawCube(n.worldPosition, Vector3.one * (_nodeDiameter * 0.9f));
        }
    }
}
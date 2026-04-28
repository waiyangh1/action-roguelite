using UnityEngine;

public class GridRefreshManager : MonoBehaviour
{
    [Header("Time‑Sliced Refresh")]
    [SerializeField] private int rowsPerFrame = 5;      // adjust based on grid height and performance
    private int currentRow = 0;
    private GridAStar grid;

    private void Start()
    {
        grid = GridAStar.Instance;
        if (grid == null)
            Debug.LogError("No GridAStar instance found in scene.");
    }

    private void FixedUpdate()
    {
        if (grid == null) return;

        int totalRows = grid.gridSizeY;
        if (totalRows == 0) return;

        // Refresh a slice of rows each physics frame
        grid.RefreshGridRows(currentRow, rowsPerFrame);
        currentRow += rowsPerFrame;

        // Wrap around when we pass the end
        if (currentRow >= totalRows)
            currentRow = 0;
    }
}
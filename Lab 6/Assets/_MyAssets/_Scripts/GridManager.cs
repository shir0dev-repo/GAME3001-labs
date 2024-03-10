using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileStatus
{
    UNVISITED,
    OPEN,
    CLOSED,
    IMPASSABLE,
    GOAL,
    START,
    PATH
};

public enum NeighbourTile
{
    TOP_TILE,
    RIGHT_TILE,
    BOTTOM_TILE,
    LEFT_TILE,
    NUM_OF_NEIGHBOUR_TILES
};

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject tilePanelPrefab;
    [SerializeField] private GameObject panelParent;
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private Color[] colors;
    [SerializeField] private float baseTileCost = 1f;
    [SerializeField] private bool useManhattanHeuristic = true;
    
    private GameObject[,] grid;
    private int rows = 12;
    private int columns = 16;
    private List<GameObject> mines = new List<GameObject>();

    public static GridManager Instance { get; private set; } // Static object of the class.

    void Awake()
    {
        if (Instance == null) // If the object/instance doesn't exist yet.
        {
            Instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void Initialize()
    {
        BuildGrid();
        // TODO: Comment out for Lab 6a.
        //ConnectGrid();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            foreach(Transform child in transform)
                child.gameObject.SetActive(!child.gameObject.activeSelf);
            panelParent.gameObject.SetActive(!panelParent.gameObject.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Vector2 gridPosition = GetGridPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            GameObject mineInst = GameObject.Instantiate(minePrefab, new Vector3(gridPosition.x, gridPosition.y, 0f), Quaternion.identity);
            Vector2 mineIndex = mineInst.GetComponent<NavigationObject>().GetGridIndex();
            Destroy(grid[(int)mineIndex.y, (int)mineIndex.x]);
            grid[(int)mineIndex.y, (int)mineIndex.x] = null;
            mines.Add(mineInst);
            // TODO: Comment out for Lab 6a.
            //ConnectGrid();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach (GameObject mine in mines)
            {
                Vector2 mineIndex = mine.GetComponent<NavigationObject>().GetGridIndex();
                GameObject tileInst = GameObject.Instantiate(tilePrefab, 
                    new Vector3(mine.transform.position.x, mine.transform.position.y, 0f), Quaternion.identity);
                tileInst.transform.parent = transform;
                grid[(int)mineIndex.y, (int)mineIndex.x] = tileInst;
                Destroy(mine);
            }
            mines.Clear();
            // TODO: Comment out for Lab 6a.
            //ConnectGrid();
        }
        if (Input.GetKeyDown(KeyCode.F)) // Start pathfinding.
        {
            // Get ship node.
            GameObject ship = GameObject.FindGameObjectWithTag("Ship");
            Vector2 shipIndices = ship.GetComponent<NavigationObject>().GetGridIndex();
            PathNode start = grid[(int)shipIndices.y, (int)shipIndices.x].GetComponent<TileScript>().Node;
            // Get goal node.
            GameObject planet = GameObject.FindGameObjectWithTag("Planet");
            Vector2 planetIndices = planet.GetComponent<NavigationObject>().GetGridIndex();
            PathNode goal = grid[(int)planetIndices.y, (int)planetIndices.x].GetComponent<TileScript>().Node;
            // Start the algorithm.
            PathManager.Instance.GetShortestPath(start, goal);
        }
        if (Input.GetKeyDown(KeyCode.R)) // Reset grid/pathfinding.
        {
            SetTileStatuses();
        }
    }

    private void BuildGrid()
    {
        grid = new GameObject[rows, columns];
        //int count = 0;
        float rowPos = 5.5f;
        for (int row = 0; row < rows; row++, rowPos--)
        {
            float colPos = -7.5f;
            for (int col = 0; col < columns; col++, colPos++)
            {
                GameObject tileInst = GameObject.Instantiate(tilePrefab, new Vector3(colPos, rowPos, 0f), Quaternion.identity);

                // TODO: Commented out for Lab 6a.
                //TileScript tileScript = tileInst.GetComponent<TileScript>();
                //tileScript.SetColor(colors[System.Convert.ToInt32((count++ % 2 == 0))]);
                tileInst.transform.parent = transform;
                grid[row,col] = tileInst;
                // TODO: Commented out for Lab 6a.
                // Instantiate a new TilePanel and link it to the Tile instance.
                //GameObject panelInst = GameObject.Instantiate(tilePanelPrefab, tilePanelPrefab.transform.position, Quaternion.identity);
                //panelInst.transform.SetParent(panelParent.transform);
                //RectTransform panelTransform = panelInst.GetComponent<RectTransform>();
                //panelTransform.localScale = Vector3.one;
                //panelTransform.anchoredPosition = new Vector3(64f * col, -64f * row);
                //tileScript.tilePanel = panelInst.GetComponent<TilePanelScript>();
                // Create a new PathNode for the new tile.
                //tileScript.Node = new PathNode(tileInst);
            }
            // TODO: Commented out for Lab 6a.
            //count--;
        }
        // TODO: Commented out for Lab 6a.
        // Set the tile under the ship to Start.
        //GameObject ship = GameObject.FindGameObjectWithTag("Ship");
        //Vector2 shipIndices = ship.GetComponent<NavigationObject>().GetGridIndex();
        //grid[(int)shipIndices.y, (int)shipIndices.x].GetComponent<TileScript>().SetStatus(TileStatus.START);
        // Set the tile under the player to Goal and set tile costs.
        //GameObject planet = GameObject.FindGameObjectWithTag("Planet");
        //Vector2 planetIndices = planet.GetComponent<NavigationObject>().GetGridIndex();
        //grid[(int)planetIndices.y, (int)planetIndices.x].GetComponent<TileScript>().SetStatus(TileStatus.GOAL);
        //SetTileCosts(planetIndices);
    }

    // TODO: Comment out for Lab 6a. We don't need to connect grid for Lab 6.
    public void ConnectGrid() // TODO: Made public for Lab 5. Also lots of changes within.
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                TileScript tileScript = grid[row, col].GetComponent<TileScript>();
                tileScript.ResetNeighbourConnections(); // TODO: New for Lab 5.
                if (tileScript.status == TileStatus.IMPASSABLE) continue;
                if (row > 0) // Set top neighbour if tile is not in top row.
                {
                    if (!(grid[row - 1, col].GetComponent<TileScript>().status == TileStatus.IMPASSABLE))
                    {
                        tileScript.SetNeighbourTile((int)NeighbourTile.TOP_TILE, grid[row - 1, col]);
                        tileScript.Node.AddConnection(new PathConnection(tileScript.Node, grid[row - 1, col].GetComponent<TileScript>().Node,
                            Vector3.Distance(tileScript.transform.position, grid[row - 1, col].transform.position)));
                    }
                }
                if (col < columns - 1) // Set right neighbour if tile is not in rightmost row.
                {
                    if (!(grid[row, col + 1].GetComponent<TileScript>().status == TileStatus.IMPASSABLE))
                    {
                        tileScript.SetNeighbourTile((int)NeighbourTile.RIGHT_TILE, grid[row, col + 1]);
                        tileScript.Node.AddConnection(new PathConnection(tileScript.Node, grid[row, col + 1].GetComponent<TileScript>().Node,
                            Vector3.Distance(tileScript.transform.position, grid[row, col + 1].transform.position)));
                    }
                }
                if (row < rows - 1) // Set bottom neighbour if tile is not in bottom row.
                {
                    if (!(grid[row + 1, col].GetComponent<TileScript>().status == TileStatus.IMPASSABLE))
                    {
                        tileScript.SetNeighbourTile((int)NeighbourTile.BOTTOM_TILE, grid[row + 1, col]);
                        tileScript.Node.AddConnection(new PathConnection(tileScript.Node, grid[row + 1, col].GetComponent<TileScript>().Node,
                             Vector3.Distance(tileScript.transform.position, grid[row + 1, col].transform.position)));
                    }
                }
                if (col > 0) // Set left neighbour if tile is not in leftmost row.
                {
                    if (!(grid[row, col - 1].GetComponent<TileScript>().status == TileStatus.IMPASSABLE))
                    {
                        tileScript.SetNeighbourTile((int)NeighbourTile.LEFT_TILE, grid[row, col - 1]);
                        tileScript.Node.AddConnection(new PathConnection(tileScript.Node, grid[row, col - 1].GetComponent<TileScript>().Node,
                            Vector3.Distance(tileScript.transform.position, grid[row, col - 1].transform.position)));
                    }
                }
            }
        }
    }
    // Comment out to here.

    public GameObject[,] GetGrid()
    {
        return grid;
    }
    public Vector2 GetGridPosition(Vector2 worldPosition)
    {
        float xPos = Mathf.Floor(worldPosition.x) + 0.5f;
        float yPos = Mathf.Floor(worldPosition.y) + 0.5f;
        return new Vector2(xPos, yPos);
    }

    public void SetTileCosts(Vector2 targetIndices)
    {
        float distance = 0f;
        float dx = 0f;
        float dy = 0f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                TileScript tileScript = grid[row, col].GetComponent<TileScript>();
                if (useManhattanHeuristic)
                {
                    dx = Mathf.Abs(col - targetIndices.x);
                    dy = Mathf.Abs(row - targetIndices.y);
                    distance = dx + dy;
                }
                else // Euclidean.
                {
                    dx = targetIndices.x - col;
                    dy = targetIndices.y - row;
                    distance = Mathf.Sqrt(dx * dx + dy * dy);
                }
                float adjustedCost = distance * baseTileCost;
                tileScript.cost = adjustedCost;
                tileScript.tilePanel.costText.text = tileScript.cost.ToString("F1");
            }
        }
    }

    public void SetTileStatuses()
    {
        foreach (GameObject go in grid)
        {
            go.GetComponent<TileScript>().SetStatus(TileStatus.UNVISITED);
        }
        foreach (GameObject mine in mines)
        {
            Vector2 mineIndex = mine.GetComponent<NavigationObject>().GetGridIndex();
            grid[(int)mineIndex.y, (int)mineIndex.x].GetComponent<TileScript>().SetStatus(TileStatus.IMPASSABLE);
        }
        // Set the tile under the ship to Start.
        GameObject ship = GameObject.FindGameObjectWithTag("Ship");
        Vector2 shipIndices = ship.GetComponent<NavigationObject>().GetGridIndex();
        grid[(int)shipIndices.y, (int)shipIndices.x].GetComponent<TileScript>().SetStatus(TileStatus.START);
        // Set the tile under the player to Goal.
        GameObject planet = GameObject.FindGameObjectWithTag("Planet");
        Vector2 planetIndices = planet.GetComponent<NavigationObject>().GetGridIndex();
        grid[(int)planetIndices.y, (int)planetIndices.x].GetComponent<TileScript>().SetStatus(TileStatus.GOAL);
    }
}

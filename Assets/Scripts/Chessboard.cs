using System;
using UnityEngine;
public class Chessboard : MonoBehaviour
{
    [Header("Art stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tilleSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;

    // LOGIC
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;
    private ChessPiece[,] chessPiece;

    private void Awake()
    {
        GenerateAllTiles(tilleSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPiece();
    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover")))
        {
            //Get the indexes of the tile i've hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // If we're hovering a tile after not hovering any tiles
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover"); 
            }

            // If we were already hoevering a tile, change the previous one
            else if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
        }
    }

    // Generate the board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY]; 
        for (int x =  0; x < tileCountX; x++) 
            for (int y = 0; y < tileCountY; y++)
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x+1) * tileSize, yOffset, y* tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
          
        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }

    //Spawing of the pieces
    private void SpawnAllPieces()
    {
        chessPiece = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
        chessPiece[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, 0);
        chessPiece[0, 1] = SpawnSinglePiece(ChessPieceType.Knight, 0);
        chessPiece[0, 2] = SpawnSinglePiece(ChessPieceType.Bishop, 0);
        chessPiece[0, 3] = SpawnSinglePiece(ChessPieceType.Queen, 0);
        chessPiece[0, 4] = SpawnSinglePiece(ChessPieceType.King, 0);
        chessPiece[0, 5] = SpawnSinglePiece(ChessPieceType.Bishop, 0);
        chessPiece[0, 6] = SpawnSinglePiece(ChessPieceType.Knight, 0);
        chessPiece[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, 0);
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPiece[1, i] = SpawnSinglePiece(ChessPieceType.Pawn, 0);
        }
        chessPiece[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, 1);
        chessPiece[7, 1] = SpawnSinglePiece(ChessPieceType.Knight, 1);
        chessPiece[7, 2] = SpawnSinglePiece(ChessPieceType.Bishop, 1);
        chessPiece[7, 3] = SpawnSinglePiece(ChessPieceType.Queen, 1);
        chessPiece[7, 4] = SpawnSinglePiece(ChessPieceType.King, 1);
        chessPiece[7, 5] = SpawnSinglePiece(ChessPieceType.Bishop, 1);
        chessPiece[7, 6] = SpawnSinglePiece(ChessPieceType.Knight, 1);
        chessPiece[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, 1);
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPiece[6, i] = SpawnSinglePiece(ChessPieceType.Pawn, 1);
        }

    }

    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team)
    {
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();
        cp.type = type;
        cp.team = team;
        cp.mesh.material = teamMaterials[team];
        cp.transform.localScale = new Vector3(cp.transform.localScale.x * cp.desiredScale.x, cp.transform.localScale.y * cp.desiredScale.y, cp.transform.localScale.z * cp.desiredScale.z);
        return cp;

    }

    private void PositionAllPiece()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (chessPiece[x, y] != null)
                    PositionSinglePiece(x, y, true);
    }
    private void PositionSinglePiece(int x, int y, bool force = false)
    {
        chessPiece[x, y].currentX = x;
        chessPiece[x, y].currentX = y;
        chessPiece[x, y].transform.position = GetTileCenter(x,y);
    }

    private Vector3 GetTileCenter(int x, int y) => new Vector3(x* tilleSize,yOffset,y* tilleSize) - bounds + new Vector3(tilleSize/2,0,tilleSize/2);

    // Operations
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++) 
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one; // Invalid
    }
}

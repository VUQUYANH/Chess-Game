using UnityEngine;

public enum ChessPieceType
{
    Nome = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6,

}

public class ChessPiece : MonoBehaviour
{
    public int team;
    public int currentX;
    public int currentY;
    public MeshRenderer mesh;
    public ChessPieceType type;

    public Vector3 desiredPosition;
    public Vector3 desiredScale;
}
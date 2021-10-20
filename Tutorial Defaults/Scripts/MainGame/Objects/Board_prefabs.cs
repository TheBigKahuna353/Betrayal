using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board_prefabs : MonoBehaviour
{
    // vars
    public GameObject prefab;
    public GameObject basement;
    public GameObject ground;
    public GameObject upper;
    public static Tile[][][] board = new Tile[3][][];
    public int spread;

    // Awake is called before Start
    void Awake()
    {
        //set the basement tiles parent to the basement object
        GameObject t = basement;

        // create all the tiles and set them to Inactive
        for (int k = 0; k < 3; k++)
        {
            //if creating ground floor tiles, set the paren to the ground floor object
            if (k == 1) t = ground;
            if (k == 2) t = upper;
            board[k] = new Tile[10][];
            for (int i = 0; i < 9; i++)
            {
                board[k][i] = new Tile[10];
                for (int j = 0; j < 9; j++)
                {
                    Vector3 pos = new Vector3(i - 4, 0, j - 5 + ((k-1)*spread));
                    Vector3Int pos2d = new Vector3Int(i, j,k);
                    GameObject tile = Instantiate(prefab, pos, Quaternion.identity);
                    tile.transform.parent = t.transform;
                    board[k][i][j] = tile.GetComponentInChildren<Tile>();
                    board[k][i][j].Setup(pos2d, "Inactive");

                }
            }
        }

    }

    // Gizmos draw in scene view for debugging
    //draws the outline of the floors tiles
    void OnDrawGizmos()
    {
        int i = -1;

        Gizmos.DrawLine(new Vector3(-4.5f, 0.5f, -4.5f + (i * spread)), new Vector3(-4.5f, 0.5f, 4.5f + (i * spread)));
        Gizmos.DrawLine(new Vector3(-4.5f, 0.5f, -4.5f + (i * spread)), new Vector3(4.5f, 0.5f, -4.5f + (i * spread)));
        Gizmos.DrawLine(new Vector3(4.5f, 0.5f, 4.5f + (i * spread)), new Vector3(-4.5f, 0.5f, 4.5f + (i * spread)));
        Gizmos.DrawLine(new Vector3(4.5f, 0.5f, 4.5f + (i * spread)), new Vector3(4.5f, 0.5f, -4.5f + (i * spread)));

        i++;

        Gizmos.DrawLine(new Vector3(-4.5f, 0.5f, -4.5f + (i * spread)), new Vector3(-4.5f, 0.5f, 4.5f + (i * spread)));
        Gizmos.DrawLine(new Vector3(-4.5f, 0.5f, -4.5f + (i * spread)), new Vector3(4.5f, 0.5f, -4.5f + (i * spread)));
        Gizmos.DrawLine(new Vector3(4.5f, 0.5f, 4.5f + (i * spread)), new Vector3(-4.5f, 0.5f, 4.5f + (i * spread)));
        Gizmos.DrawLine(new Vector3(4.5f, 0.5f, 4.5f + (i * spread)), new Vector3(4.5f, 0.5f, -4.5f + (i * spread)));

        i++;

        Gizmos.DrawLine(new Vector3(-4.5f, 0.5f, -4.5f + (i * spread)), new Vector3(-4.5f, 0.5f, 4.5f + (i * spread)));
        Gizmos.DrawLine(new Vector3(-4.5f, 0.5f, -4.5f + (i * spread)), new Vector3(4.5f, 0.5f, -4.5f + (i * spread)));
        Gizmos.DrawLine(new Vector3(4.5f, 0.5f, 4.5f + (i * spread)), new Vector3(-4.5f, 0.5f, 4.5f + (i * spread)));
        Gizmos.DrawLine(new Vector3(4.5f, 0.5f, 4.5f + (i * spread)), new Vector3(4.5f, 0.5f, -4.5f + (i * spread)));


    }

    //sides
    //0  down
    //1 left
    //2 up
    //3 right

    //true if neighbour has door facing tile for every neighbour
    public List<bool> getNeighborsDoors(int k, int x, int y)
    {
        List<bool> sides = new List<bool>() {false, false,false,false };
        if (y > 0 && board[k][x][y - 1].active == true) sides[0] = board[k][x][y - 1].doorLocations[2];
        if (y < 8 && board[k][x][y + 1].active == true) sides[2] = board[k][x][y + 1].doorLocations[0];
        if (x > 0 && board[k][x - 1][y].active == true) sides[1] = board[k][x - 1][y].doorLocations[3];
        if (x < 8 && board[k][x + 1][y].active == true) sides[3] = board[k][x + 1][y].doorLocations[1];
        return sides;
    }

    //if neighbour is active
    public bool GetNeighborActive(int k, int x, int y, int side)
    {
        if (side == 0 && y > 0) return board[k][x][y - 1].active;
        if (side == 1 && x > 0) return board[k][x - 1][y].active;
        if (side == 2 && y < 8) return board[k][x][y + 1].active;
        if (side == 3 && x < 8) return board[k][x + 1][y].active;
        return false;
    }


    //if there is a neighbor that is active
    public bool NeighbourTileActive(int k,int x, int y)
    {
        if (board[k][x][y].GetComponent<Tile>().active == true) return true;
        if (x < 8 && board[k][x+1][y].active == true) return true;
        if (x > 0 && board[k][x-1][y].active == true) return true;
        if (y < 8 && board[k][x][y+1].active == true) return true;
        if (y > 0 && board[k][x][y-1].active == true) return true;
        if (board[k][x][y].IsLanding) return true;
        return false;
    }

    //reset all tiles to unavailble to move to and set waiting to true so doesnt look dull
    public void ResetAvailabilities()
    {
        //int floor = start.z;
        for (int floor = 0; floor < 3; floor++)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    board[floor][i][j].available = false;
                    board[floor][i][j].waiting = true;
                }
            }
        }
    }

    public int GetDoorNeighbourSide(Vector3Int pos)
    {
        List<bool> otherdoors = getNeighborsDoors(pos.z,pos.x,pos.y);
        for (int i = 0;i < 4;i++)
        {
            if (otherdoors[i])
            {
                return i;
            }
        }
        return -1;
    }

    public Tile GetTile(Vector3Int pos)
    {
        return board[pos.z][pos.x][pos.y];
    }

    public void TilesWait()
    {
        for (int floor = 0; floor < 3; floor++)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    board[floor][i][j].waiting = !board[floor][i][j].waiting;
                }
            }
        }
    }


    public void GetAllAvailabilities(int floor2Check = -1, bool checkActive = true)
    {
        int startFloor = floor2Check;
        int endFloor = floor2Check + 1;
        if (floor2Check == -1)
        {
            startFloor = 0; 
            endFloor = 3; 
        }
        for (int floor = startFloor; floor < endFloor; floor++)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    board[floor][i][j].waiting = false;
                    if (!checkActive && board[floor][i][j].active) continue;
                    if (NeighbourTileActive(floor, i, j))
                    {
                        List<Tile> path = GameObject.Find("Manager").GetComponent<GameManager>().GetPath(new Vector3Int(4,5,floor), new Vector3Int(i, j, floor));
                        if (path != null)
                        {
                            board[floor][i][j].available = true;
                            Debug.Log(floor);
                        }
                        else
                        {
                            board[floor][i][j].available = false;
                        }
                    }
                }
            }
        }
    }

    //starting from the charachters location
    //if there is a path within the movement speed
    //set it as available to move to
    public void CheckAvailabilities(Vector3Int start, int maxdistance)
    {
        //int floor = start.z;
        for (int floor = 0; floor < 3; floor++)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    board[floor][i][j].waiting = false;
                    if (NeighbourTileActive(floor, i, j))
                    {
                        List<Tile> path = GameObject.Find("Manager").GetComponent<GameManager>().GetPath(start, new Vector3Int(i, j, floor));
                        if (path != null && path.Count - 1 <= maxdistance)
                        {
                            board[floor][i][j].available = true;
                        }
                        else
                        {
                            board[floor][i][j].available = false;
                        }
                    }
                }
            }
        }
    }
}

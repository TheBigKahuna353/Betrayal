using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navagation
{
    //vars
    private List<Tile> OpenList;
    private List<Tile> ClosedList;

    Tile[][][] board = Board_prefabs.board;

    //main function to call to get path from one pos to another
    //used a* pathfinding
    public List<Tile> FindPath(Vector3Int startPos, Vector3Int endPos)
    {
        //each tile is a 'node'
        //gCost is cost from starting node to current node
        //hCost is cost straight to end node, ignoring obstacles
        //fCost is the sum of hCost and gCost


        //get start and end node
        Tile startNode = board[startPos.z][startPos.x][startPos.y];
        Tile endNode = board[endPos.z][endPos.x][endPos.y];

        if (startNode == null || endNode == null)
        {
            // Invalid Path
            Debug.Log("Invalid");
            return null;
        }

        //openList is a list of nodes to check
        //closed is list of checked nodes
        OpenList = new List<Tile> { startNode };
        ClosedList = new List<Tile>();

        //set every node to high gCost
        //set Camefrom to null to 'reset' old path
        for (int k = 0; k < 3; k++)
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    Tile pathNode = board[k][x][y].GetComponent<Tile>();
                    pathNode.gCost = 99999999;
                    pathNode.Camefrom = null;
                }
            }
        }

        //set starting nodes gCost and hCost
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);

        //while there is tiles to check
        while (OpenList.Count > 0)
        {
            //get the node with the lowest fCost
            //which is the smallest cost to get to and smallest cost to the end
            Tile currentNode = GetLowestFCostNode(OpenList);
            
            if (currentNode == endNode)
            {
                // Reached final node
                return CalculatePath(endNode);
            }
            //checked current node, so add to Closed list
            OpenList.Remove(currentNode);
            ClosedList.Add(currentNode);

            //if the current node is not active, dont add neighbors to Open list
            if (!currentNode.active) continue;

            //check every neighbor
            foreach (Tile neighbourNode in GetNeighbourList(currentNode))
            {
                //if already checked the neighbor, dont check it again
                if (ClosedList.Contains(neighbourNode)) continue;

                //calculate gCost (cost to get to current tile from start)
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);

                //if the calculated gCost of neighbor is lower than its gCost, chnage its path to this one
                if (tentativeGCost < neighbourNode.gCost)
                {
                    //this is to calculate path, tell neighbor node that the path came from current node
                    neighbourNode.Camefrom = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);

                    //if the neighbor is not in the Openlist, add it
                    if (!OpenList.Contains(neighbourNode))
                    {
                        OpenList.Add(neighbourNode);
                    }
                }
            }
        }

        // Out of nodes on the openList
        return null;
    }

    //get the cost to move from current tile to new tile
    private int CalculateDistanceCost(Tile a, Tile b)
    {
        int xDistance = Mathf.Abs(a.pos.x - b.pos.x);
        int yDistance = Mathf.Abs(a.pos.y - b.pos.y);
        return xDistance + yDistance;
    }

    //return the node with the lowest fCost, which is probable to be end node or nearer
    private Tile GetLowestFCostNode(List<Tile> pathNodeList)
    {
        Tile lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost() < lowestFCostNode.fCost())
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    //Once the end node has been found, see what node the path came from and keep looping until the node is the start node
    private List<Tile> CalculatePath(Tile endNode)
    {
        List<Tile> path = new List<Tile>();
        Tile currentNode = endNode;
        while (currentNode.Camefrom != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.Camefrom;
        }
        path.Add(currentNode);
        //reverse the list so it starts at the start node
        path.Reverse();
        return path;
    }

    //sides
    //0  down
    //1 left
    //2 up
    //3 right

    //get the neigbour nodes of current node
    public List<Tile> GetNeighbourList(Tile currentNode)
    {
        List<Tile> neighbourList = new List<Tile>();


        // if the node is a landing, get the other landings on other floors
        if (currentNode.IsLanding)
        {
            if (currentNode.pos.z == 1)
            {
                neighbourList.Add(board[2][4][5]);
            }
            if (currentNode.pos.z == 2)
            {
                neighbourList.Add(board[1][4][3]);
            }
        }
        //do same for down, put down first so i prioritizes down over left and right
        if (currentNode.pos.y - 1 >= 0 && currentNode.doorLocations[0])
        {
            Tile t = board[currentNode.pos.z][currentNode.pos.x][currentNode.pos.y - 1];
            if (!t.active || t.doorLocations[2])
            {
                neighbourList.Add(board[currentNode.pos.z][currentNode.pos.x][currentNode.pos.y - 1]);
            }
        }
        //if the node is not on the edge and has a door in that direction and ( other node is not active or is active and has a door facing current node)
        if (currentNode.pos.x - 1 >= 0 && currentNode.doorLocations[1])
        {
            Tile t = board[currentNode.pos.z][currentNode.pos.x - 1][currentNode.pos.y];
            if (!t.active || t.doorLocations[3])
            {
                neighbourList.Add(board[currentNode.pos.z][currentNode.pos.x - 1][currentNode.pos.y]);
            }
        }
        //do same for right
        if (currentNode.pos.x + 1 <= 8 && currentNode.doorLocations[3])
        {
            Tile t = board[currentNode.pos.z][currentNode.pos.x + 1][currentNode.pos.y];
            if (!t.active || t.doorLocations[1])
            {
                neighbourList.Add(board[currentNode.pos.z][currentNode.pos.x + 1][currentNode.pos.y]);
            }
        }
        //do same for up
        if (currentNode.pos.y + 1 <= 8 && currentNode.doorLocations[2])
        {
            Tile t = board[currentNode.pos.z][currentNode.pos.x][currentNode.pos.y + 1];
            if (!t.active || t.doorLocations[0])
            {
                neighbourList.Add(board[currentNode.pos.z][currentNode.pos.x][currentNode.pos.y + 1]);
            }
        }


        return neighbourList;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLandings : MonoBehaviour
{


    // Start is called before the first frame update
    //create landings
    void Start()
    {
        CreateGroundFloorLandings();
        CreateBasement();
        CreateUpperLanding();
    }

    //create the basement tile
    void CreateBasement()
    {
        List<bool> doors = new List<bool>()
        {
            true,true,true,true
        };

        Board_prefabs.board[0][4][5].Activate("Basement Landing", false, -1, null, doors);
        Board_prefabs.board[0][4][5].IsLanding = true;

    }

    //create the upper landing
    void CreateUpperLanding()
    {
        List<bool> doors = new List<bool>()
        {
            true,true,true,true
        };

        Board_prefabs.board[2][4][5].Activate("Upper Landing", false, -1, null, doors);
        Board_prefabs.board[2][4][5].IsLanding = true;

    }

    //create the ground floor landings
    void CreateGroundFloorLandings()
    {
        List<bool> entrance = new List<bool>()
        {
            false,true,false,true
        };

        List<bool> movableDoor = new List<bool>()
        {
            true,true,false,true
        };
        //be able to move in all direction but not show doors
        List<bool> movableLanding = new List<bool>()
        {
            true,true,true,true
        };
        List<bool> stairs = new List<bool>()
        {
            false,false,false,false
        };


        //create the starting tiles
        Board_prefabs.board[1][4][6].Activate(" ", false, -2, null, stairs);
        Board_prefabs.board[1][4][5].Activate("Front Door", false, -2, null, entrance);
        Board_prefabs.board[1][4][5].doorLocations = movableDoor;
        Board_prefabs.board[1][4][4].Activate("Corridor", false, -2, null, entrance);
        Board_prefabs.board[1][4][4].doorLocations = movableLanding;
        Board_prefabs.board[1][4][3].Activate("Staircase", false, -1, null, stairs);
        Board_prefabs.board[1][4][3].doorLocations[2] = true;
        Board_prefabs.board[1][4][3].IsLanding = true;
    }
}

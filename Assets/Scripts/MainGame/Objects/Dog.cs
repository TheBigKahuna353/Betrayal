using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    Charachter master;
    Vector3Int pos;
    Vector3 World_pos;
    bool follow = true;

    public void Setup (Charachter charachter, Vector3 pos)
    {
        master = charachter;
        World_pos = pos;
        this.pos = master.pos;
        transform.eulerAngles = new Vector3(90,0,0);
    }

    public void update()
    {
        if (follow)
        {
            World_pos = master.GetWorldPos();
        }

        transform.position = World_pos;
    }



}

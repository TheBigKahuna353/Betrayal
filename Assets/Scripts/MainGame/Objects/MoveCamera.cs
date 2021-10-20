using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    //vars
    private bool isholding = false;
    Vector3 start;
    private Vector3 start_pos;
    public float speed = 0.003f;
    public int floor = 1;
    Vector3 Pos = new Vector3(0,4,-1.5f);

    float scroll = 0f;

    // Update is called once per frame
    void Update()
    {
        //if the user scrolls, move the camera up or down
        if (scroll != 0)
        {
            Pos = new Vector3(Pos.x, Pos.y - scroll * 3, Pos.z);
            if (Pos.y < 1.5f)
            {
                Pos = new Vector3(Pos.x, 1.5f, Pos.z);
            }
            if (Pos.y > 5f)
            {
                Pos = new Vector3(Pos.x, 5f, Pos.z);
            }
        }

        // check if user is holding down right click
        if (Input.GetMouseButtonDown(1))
        {
            isholding = true;
            Cursor.visible = false;
            start = Input.mousePosition;
            start_pos = transform.position;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isholding = false;
            Cursor.visible = true;
        }

        //if user is holding down right click, move camera in direction of cursor motion
        if (isholding)
        {
            if (Mathf.Abs(Pos.x) <= 15)
            {
                Vector3 new_pos = Input.mousePosition;
                new_pos = new Vector3((start.x - new_pos.x) * speed * start_pos.y, 0, (start.y - new_pos.y) * 1.2f * speed * start_pos.y);
                Pos = start_pos + new_pos;
            }
            else
            {
                Vector3 new_pos = Pos;
                new_pos.x = 15 * (new_pos.x - (new_pos.x - 1));
                transform.position = new_pos;
                start = Input.mousePosition;
                start_pos = transform.position;
            }
        }
        //if the '+' key is pressed, move camera up one floor
        if (Input.GetKeyDown(KeyCode.Equals) && floor < 2)
        {
            floor += 1;
            Pos.z += 10;
        }
        //if the '-' key is pressed, move camera down one floor
        if (Input.GetKeyDown(KeyCode.Minus) && floor > 0)
        {
            floor -= 1;
            Pos.z -= 10;
        }

        //make sure camera cant go off table
        Pos.x = Mathf.Clamp(Pos.x,-15,15);

        //update positon
        transform.position = Pos;
    }

    public void Move()
    {
        floor = 0;
        Pos.z = -11.5f;
    }

    //see if the user is using the touchpad scroll
    private void OnGUI()
    {
        if (Event.current.type == EventType.ScrollWheel)
        {
            scroll = -Event.current.delta.y / 100;
        }
        else
        {
            scroll = 0f;
        }
    }
}
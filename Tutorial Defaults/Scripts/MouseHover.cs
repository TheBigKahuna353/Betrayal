using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHover : MonoBehaviour
{
    //vars
    public bool Over;

    //when the mouse enters the object
    public void Enter()
    {
        Over = true;
    }

    //when the mouse exits the object
    public void Exit()
    {
        Over = false;
    }
}

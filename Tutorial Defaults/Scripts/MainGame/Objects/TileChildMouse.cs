using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileChildMouse : MonoBehaviour
{


    private void OnMouseEnter()
    {
        transform.parent.GetComponent<Tile>().OnEnter();
    }

    private void OnMouseExit()
    {
        transform.parent.GetComponent<Tile>().OnExit();
    }

    private void OnMouseOver()
    {
        transform.parent.GetComponent<Tile>().OnOver();
    }
}

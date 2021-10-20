using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HauntCounter : MonoBehaviour
{
    //vars
    public GameObject hauntSlider;
    public int HauntCount;

    //add one to the haunt counter
    public Coroutine Add()
    {
        HauntCount++;
        return StartCoroutine("MoveSlider");
    }

    //animate the slider going up one
    IEnumerator MoveSlider()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        for (float i = 0; i < 80; i+= 0.5f)
        {
            hauntSlider.transform.localPosition +=  new Vector3(0.5f,0,0);
            yield return new WaitForSeconds(0.001f);
            if (Input.GetMouseButtonDown(0))
            {
                hauntSlider.transform.localPosition += new Vector3(0.5f * 80 - i, 0, 0);
                break;
            }
        }
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
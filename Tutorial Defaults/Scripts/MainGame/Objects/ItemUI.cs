using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUI : MonoBehaviour
{
    //vars
    Animator anim;
    public bool Over;
    public bool Selected;
    public bool clicked;

    //start is called once when game starts
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    //when the mouse enters the object
    //animate the card to pop up
    public void OnEnter()
    {
        Over = true;
        if (!Selected)
        {
            float currentTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) currentTime = 1;
            currentTime = Mathf.Min(currentTime, 1);
            anim.Play("PopUp", -1, 1 - currentTime);
        }
    }

    //when the mouse exits the object
    //animate the card to pop back down
    public void OnExit()
    {
        Over = false;
        if (!Selected)
        {
            float currentTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            currentTime = Mathf.Min(currentTime, 1);
            anim.Play("PopDown", -1, 1 - currentTime);
        }
    }


    public void PopDown()
    {
        Selected = false;
        Debug.Log("not selected");
        float currentTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        currentTime = Mathf.Min(currentTime, 1);
        anim.Play("PopDown", -1, 1 - currentTime);
    }

    public void PopUp()
    {
        Selected = true;
        Debug.Log("Selected");
    }
}

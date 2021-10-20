using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{

    public GameObject button;
    public GameObject button2;

    List<string> buttons = new List<string>();

    public List<string> clicked = new List<string>();

   

    public void Open(string text, bool overide = false)
    {
        buttons.Add(text);
        Debug.Log(text);
        if (buttons.Count < 3)
        {
            SetButtons();
        }
        if (overide)
        {
            button2.SetActive(false);
        }
    }

    public bool ButtonOpen(string name)
    {
        return buttons.Contains(name);
    }

    void SetButtons()
    {
        int num = buttons.Count;
        if (num > 0)
        {
            button.name = buttons[num -1];
            button.transform.GetChild(0).GetComponent<Text>().text = buttons[num - 1];
            button.SetActive(true);
            if (num > 1)
            {
                button2.SetActive(true);
                button2.name = buttons[num - 2];
                button2.transform.GetChild(0).GetComponent<Text>().text = buttons[num - 2];
            }
            else
            {
                button2.SetActive(false);
            }
        }
        else
        {
            button.SetActive(false);
        }
    }

    public void Close(string name)
    {
        if (buttons.Contains(name))
        {
            if (clicked.Contains(name))
            {
                clicked.Remove(name);
            }
            buttons.Remove(name);
            SetButtons();
        }
    }


    public void OnClick()
    {
        clicked.Add(button.name);

    }

    public void OnClick2()
    {
        clicked.Add(button2.name);
    }

    public void CloseAll()
    {
        clicked.Clear();
        buttons.RemoveRange(Mathf.Max(buttons.Count - 2,0),Mathf.Min(buttons.Count,2));
        SetButtons();
    }

}

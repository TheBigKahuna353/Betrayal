using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemManager : MonoBehaviour
{
    //vars
    public Charachter player;
    public GameObject prefab;
    int dist = 110;
    bool check;
    int selected = -1;
    public Button button;

    //open/make visible the cards for the player
    public void Open()
    {
        check = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        StartCoroutine("Check");
    }

    //close the cards
    public void Close()
    {
        check = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        selected = -1;
    }

    public void OnButtonClick()
    {
        Debug.Log("button clicked");
        Debug.Log(selected);
        button.Close("Use");
        button.Close("Drop");
        player.DropCard(selected);
        Destroy(transform.GetChild(selected).gameObject);
        selected = -1;
    }

    //check if a card is clicked
    IEnumerator Check()
    {
        while (check)
        {
            yield return null;
            bool OverCard = false;
            int over = 0;
            while (!OverCard)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).GetComponent<ItemUI>().Over)
                    {
                        OverCard = true;
                        over = i;
                        break;
                    }
                }
                if (OverCard) break;
                if (Input.GetMouseButtonUp(0))
                {
                    if (selected != -1)
                    {
                        if (button.clicked.Contains("Drop"))
                        {
                            OnButtonClick();
                        }
                        else if (button.clicked.Contains("Use"))
                        {
                            player.ItemClicked(selected);
                            button.Close("Use");
                        }
                        else
                        {
                            transform.GetChild(selected).GetComponent<ItemUI>().PopDown();
                            selected = -1;
                            button.Close("Use");
                            button.Close("Drop");
                        }
                    }
                }
                yield return null;
            }
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log(selected + " " + over);
                if (selected != -1)
                {
                    transform.GetChild(selected).GetComponent<ItemUI>().Selected = false;
                    if (over == selected)
                    {
                        selected = -1;
                        button.Close("Use");
                        button.Close("Drop");
                        Debug.Log("Unselect");
                        continue;
                    }
                }
                Debug.Log("select");
                selected = over;
                Debug.Log(selected + " is selected");
                transform.GetChild(selected).GetComponent<ItemUI>().PopUp();
                if (!player.cards[over].passive && player.cards[over].num > 0)
                {
                    button.Open("Drop");
                    button.Open("Use");
                }
                else
                {
                    button.Open("Drop",true);
                }
            }
        }
    }

    //add a card
    public void Add(Data data)
    {
        Debug.Log(data);
        GameObject obj = Instantiate(prefab, transform);
        obj.transform.GetChild(3).GetComponent<Text>().text = data.title;
        obj.transform.GetChild(2).GetComponent<Text>().text = data.body;
        for (int i = 0; i < 3; i++)
        {
            string txt = "";
            if (i < data.FXtxt.Length) txt = data.FXtxt[i];
            obj.transform.GetChild(4 + i).GetComponent<TextMeshProUGUI>().text = txt;
        }
        if (data.type == "item") obj.transform.GetChild(1).gameObject.SetActive(true);
        if (data.type == "omen") obj.transform.GetChild(0).gameObject.SetActive(true);
        float start = -((transform.childCount - 1) * dist) / 2;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localPosition = new Vector3(start + (i * dist), -200, 0);
        }
    }

    //is the mouse over
    public bool MouseOver()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<ItemUI>().Over) return true;
        }
        return false;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventController : MonoBehaviour
{
    //vars
    public float vel = 5;
    public float speed = 0.0001f;

    public bool Showing;
    bool spinning;

    Data currentEventData;

    public GameObject EventImg;
    public GameObject OmenImg;
    public GameObject ItemImg;
    GameObject panel;
    public GameObject Panel;
    GameObject EventPanel;
    GameObject ItemPanel;
    GameObject OmenPanel;
    GameObject Image;
    public DiceManager dice;
    public GameManager manager;
    public HauntCounter HauntCounter;

    public GameObject TileImage;
    public GameObject TileImageBack;


    private void Start()
    {
        EventPanel = Panel.transform.GetChild(0).gameObject;
        ItemPanel = Panel.transform.GetChild(2).gameObject;
        OmenPanel = Panel.transform.GetChild(1).gameObject;
    }

    public Coroutine Tile (string tile)
    {
        return StartCoroutine(TileEffect(tile));
    }

    IEnumerator TileEffect(string tile) 
    {
        Showing = true;
        Sprite sprite = Resources.Load<Sprite>("UIRooms/" + tile);
        TileImage.GetComponent<Image>().sprite = sprite;
        Image = TileImageBack;
        panel = TileImage;
        StartCoroutine("Spin");
        while (spinning) 
        { 
            if (Input.GetMouseButtonDown(0)) { spinning = false; }
            yield return null; 
        }
        while (!Input.GetMouseButtonDown(0)) { yield return null; }
        Close();
    }


    //start the event
    //animates card appearing
    public void StartEvent(string type, Data data)
    {
        Showing = true;
        Panel.SetActive(true);
        Panel.transform.GetChild(4).GetComponent<Text>().text = data.title;
        Panel.transform.GetChild(3).GetComponent<Text>().text = data.body;
        for (int i = 0; i < 3; i++)
        {
            string txt = "";
            if (i < data.FXtxt.Length) txt = data.FXtxt[i];
            Panel.transform.GetChild(5 + i).GetComponent<TextMeshProUGUI>().text = txt;
        }
        currentEventData = data;
        panel = Panel;
        if (type == "item")
        {
            Image = ItemImg;
            ItemPanel.SetActive(true);
            OmenPanel.SetActive(false);
            EventPanel.SetActive(false);
        }
        if (type == "omen")
        {
            Image = OmenImg;
            OmenPanel.SetActive(true);
            EventPanel.SetActive(false);
            ItemPanel.SetActive(false);
        }
        if (type == "event")
        {
            Image = EventImg;
            EventPanel.SetActive(true);
            ItemPanel.SetActive(false);
            OmenPanel.SetActive(false);
        }
        StartCoroutine("Spin");
        if (type == "event") StartCoroutine("Event");
        else if (type == "item") StartCoroutine("Item");
        else StartCoroutine("Omen");
    }

    //effects
    //1 - gain might
    //2 - gain speed
    //3 - gain sanity
    //4 - gain knowledge
    //5 - physical
    //6 - mental
    //1 - 1
    //2 - 2
    //3 - 3
    //4 - 4
    //5 - 1 die
    //6 - 2 die
    //7 - 3 die
    //8 - 4 die

    //when an item is clicked, use it
    public void ItemClicked(Data item)
    {
        Debug.Log(currentEventData.trait);
        if (currentEventData.trait != "None")
        {
            Debug.Log(dice.WaitForRoll + " " + dice.Rolled);
            if (dice.WaitForRoll && !dice.Rolled)
            {
                item.die--;
                foreach (int f in item.ItemEffect(currentEventData.trait))
                {
                    Debug.Log(f);
                    if (Mathf.Abs(f) < 10)
                    {
                        dice.AddDice(f, Vector3.up * 2);
                    }
                    else
                    {
                        int d = Mathf.Abs(f) % 10;
                        if (f < 0) d *= -1;
                        manager.StatsChange(Mathf.Abs(f), d);
                    }
                }
            }
        }
    }

    //close the card on the screen
    void Close()
    {
        panel.SetActive(false);
        Image.SetActive(false);
    }

    //make the card visible
    void Open()
    {
        panel.SetActive(true);
    }

    //animation to make card appear
    IEnumerator Spin()
    {
        int count = 0;
        vel = 17.9335793367f;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        spinning = true;
        while (spinning) 
        {
            count++;
            vel -= 0.09908054882154696f;
            transform.eulerAngles = new Vector3(0,transform.eulerAngles.y + vel,0);
            transform.localScale += new Vector3(speed,speed,speed);
            yield return new WaitForSeconds(0.01f);
            if (transform.eulerAngles.y > 90 && transform.eulerAngles.y < 270)
            {
                panel.SetActive(true);
                Image.SetActive(false);
            }
            else
            {
                panel.SetActive(false);
                Image.SetActive(true);
            }
            if (transform.localScale.x >= 1)
            {
                spinning = false;
            }
        }
        panel.SetActive(true);
        Image.SetActive(false);
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    //steps for when you pick up an item
    IEnumerator Item()
    {
        while (spinning) 
        {
            if (Input.GetMouseButtonDown(0)) spinning = false;
            yield return null;
        }

        while (!Input.GetMouseButtonDown(0)) { yield return null; }

        Close();

        foreach (int effect in currentEventData.ItemEffect("start"))
        {
            yield return StartCoroutine(manager.ApplyEffects(effect));
        }

        manager.AddCard(currentEventData);
        manager.FinishedEvent();
    }

    //steps for when you pick up an omen
    IEnumerator Omen()
    {
        while (spinning)
        {
            if (Input.GetMouseButtonDown(0)) spinning = false;
            yield return null;
        }

        while (!Input.GetMouseButtonDown(0)) { yield return null; }
        Close();

        foreach (int effect in currentEventData.ItemEffect("start"))
        {
            yield return StartCoroutine(manager.ApplyEffects(effect));
        }

        manager.AddCard(currentEventData);

        //haunt roll
        Debug.Log("roll Die");
        dice.RollDice(Vector3.up * 2);
        while (dice.WaitForRoll) { yield return null; }

        if (dice.Sum >= HauntCounter.HauntCount)
        {
            yield return HauntCounter.Add();
        }
        else
        {
            Debug.Log("Haunt Started");
        }

        manager.FinishedEvent();
    }

    //steps for when you pick up an event
    IEnumerator Event()
    {
        while (spinning)
        {
            if (Input.GetMouseButtonDown(0)) spinning = false;
            yield return null;
        }

        while (!Input.GetMouseButtonDown(0)) { yield return null; }
        if (currentEventData.die > 0)
        {
            Close();
            dice.RollDice(Vector3.up * 2, currentEventData.die);
        }
        else
        {
            Close();
            Showing = false;
            yield break;
        }
        while (dice.WaitForRoll) { yield return null; }

        int[] effect = currentEventData.GetEffect(dice.Sum);
        Open();
        panel.transform.GetChild(currentEventData.FXnum + 5).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        yield return null;

        while (!Input.GetMouseButtonDown(0)) { yield return null; }

        panel.transform.GetChild(currentEventData.FXnum + 5).GetComponent<TextMeshProUGUI>().fontStyle ^= FontStyles.Bold;
        Close();

        foreach (int f in effect)
        {
            yield return StartCoroutine(manager.ApplyEffects(f));
        }

        manager.FinishedEvent();
    }

}

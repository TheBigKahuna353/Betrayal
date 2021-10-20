using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Charachter : MonoBehaviour
{
    //vars
    public GameObject board;

    public new string name;
    int[][] stats = new int[4][];
    public int[] StatsIndex = new int[4];
    Text[] statsUI = new Text[4];
    string[] statsOrder;
    Color col;
    public List<Data> cards = new List<Data>();
    Vector3 World_pos = new Vector3(0, 0.4f, 0);
    public Vector3Int pos = new Vector3Int(4, 5, 1);
    public bool IsTurn = false;
    public bool StartedTurn = false;
    private bool StartScreen;
    private System.Random rnd = new System.Random();
    public int CharID;
    GameManager manager;
    int tileSide = -1;
    public bool crossing;

    public Animator anim;
    public ItemManager ItemManager;

    public GameObject Message;

    Dog Dog;
    public GameObject DogPrefab;

    // Start is called before the first frame update
    //basically sets up the chrachter
    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<GameManager>();

        this.SetStats();

        statsOrder = new string[4]
        {
            "Might: ","Speed: ","Sanity: ","Knowledge: "
        };
        transform.GetComponentInChildren<MeshRenderer>().material.color = this.col;
        for (int i = 0; i < 4; i++)
        {
            statsUI[i] = GameObject.Find(statsOrder[i].Trim(new char[2] { ' ', ':' })).GetComponent<Text>();
        }

    }

    public void DropCard(int card)
    {
        manager.DropItem(cards[card]);
        cards.RemoveAt(card);
        Debug.Log("dropped card");
    }

    public int GetStat(int stat)
    {
        return this.stats[stat][this.StatsIndex[stat]];
    }

    //checks all items to see if any have any passive effects (like add 1 die to all trait rolls)
    //returns the amount of dice that should be added
    public int CheckItemsPassive(string state)
    {
        int diceAdded = 0;
        foreach (Data item in cards)
        {
            if (item.type == "Item")
            {
                if (item.passive)
                {
                    foreach (int effect in item.ItemEffect(state))
                    {

                        diceAdded += effect;
                    }
                }
            }
        }
        return diceAdded;
    }

    public Vector3 GetWorldPos()
    {
        return this.World_pos;
    }

    //add an item to the charachter
    public void AddItem(Data data)
    {
        cards.Add(data);
        ItemManager.Add(data);
        ItemManager.Open();
        if (data.title == "Dog")
        {
            Dog = Instantiate(DogPrefab, Vector3.zero, Quaternion.identity).GetComponent<Dog>();
            Dog.Setup(this, this.World_pos);
        }
    }

    //add an omen to the charachter
    public void AddOmen(Data omen)
    {
        cards.Add(omen);
        ItemManager.Add(omen);
    }


    //if an card was clicked, check to see if it was an item or omen and notify game manager
    public void ItemClicked(int index)
    {
        Data item = cards[index];
        if (item.type == "Item") manager.ItemClicked(item);
        manager.OmenClicked(item);
    }

    //assigns default stats
    void SetStats()
    {
        // get stats
        string[] Info = manager.stats(this.CharID); //Info = name, stats, colour
        this.name = Info[0];
        string[] rawStats = Info[1].Split('-'); //rawStats = might, speed, sanity, knowlege
        string col = Info[2].Remove(Info[2].Length-1);
        if (col == "white") this.col = Color.white;
        else if (col == "purple") this.col = Color.Lerp(Color.blue, Color.red, 0.5f);
        else if (col == "red") this.col = Color.red;
        else if (col == "blue") this.col = Color.blue;
        else if (col == "orange") this.col = Color.Lerp(Color.red, Color.yellow, 0.5f);
        else if (col == "green") this.col = Color.green;
        else HandyFuncs.PrintArr(col.ToCharArray());
        for (int i = 0; i < 4; i++)
        {
            string[] stat = rawStats[i].Split('_'); //stat = 0, 1, 2...
            //HandyFuncs.PrintArr(stat);
            this.stats[i] = new int[stat.Length];
            for (int j = 0; j < stat.Length; j++)
            {
                //Debug.Log(stat[j]);
                if (stat[j].Length == 2)
                {
                    stat[j] = stat[j][1].ToString();
                    this.StatsIndex[i] = int.Parse(stat[j]);
                }
                this.stats[i][j] = int.Parse(stat[j]);
            }
            //HandyFuncs.PrintArr(stats[i]);
        }
    }

    //update the ui with stats
    public void UpdateUI()
    {
        for (int i = 0; i < 4; i++)
        {
            statsUI[i].text = statsOrder[i];
            int start = Mathf.Max(StatsIndex[i] - 2,0);
            if (start + 4 > stats[i].Length) start -=  1;
            for (int stat = start; stat < start + 4; stat++)
            {
                if (stat == StatsIndex[i]) statsUI[i].text += "<color=yellow>";
                statsUI[i].text += stats[i][stat].ToString() + " ";
                if (stat == StatsIndex[i]) statsUI[i].text += "</color>";
            }
        }
    }

    //sides
    //0  down
    //1 left
    //2 up
    //3 right

    //get what direction/side the tile is to another tile
    int GetSide(Tile t1, Tile t2)
    {
        Vector3Int dif = t2.pos - t1.pos;
        if (dif.z != 0) return dif.z + 5;
        if (dif.x != 0) return dif.x + 2;
        return dif.y + 1;
    }

    public void ActivateTile(Tile t, int side)
    {
        string[] data = manager.GetName(t.pos.z);
        t.Activate(data[0], true, side, data);
    }

    //animate the charachter model to move to selected tile
    //this also activates tile
    public IEnumerator Animate(List<Tile> path)
    {
        bool checkEvent = false;
        Tile tile = null;
        if (path != null)
        {
            int count = 0;
            foreach (Tile t in path)
            {
                //activate tile if inactive
                if (!t.active)
                {
                    ActivateTile(t, GetSide(t, path[count - 1]));
                    yield return new WaitForSeconds(1f);
                    checkEvent = true;
                    tile = t;
                }
                float time = 1f;
                //if not first tile in path
                if (count != 0)
                {
                    int side = GetSide(path[count - 1], t);
                    tileSide = side;
                    anim.SetTrigger(side.ToString());
                    if (side > 3) time = 3f;
                }
                if (count < path.Count - 1)
                {
                    int side = GetSide(t,path[count + 1] );
                    if (tileSide != -1)
                    {
                        crossing = tileSide == side;
                    }
                }

                yield return new WaitForSeconds(time);
                Move(t.pos);
                if (count != 0) manager.CheckPlayers();
                if ((t.effect == "now" && count != 0) || t.effect == "cross") yield return manager.TileEffect(t.name);
                else if (t.effect == "end" && count == path.Count - 1) { manager.TileEffect(t.name); Debug.Log("end"); }
                else if (t.effect == "exit" && count < path.Count - 1) yield return manager.TileEffect(t.name);
                if (manager.TileEffectAction() == "stop")
                {
                    manager.FinishedMoving(checkEvent, tile);
                    Debug.Log("Stop");
                    yield break;
                }
                count++;
            }
        }
        manager.FinishedMoving(checkEvent,tile);
    }

    public void Move(Vector3Int pos)
    {
        Vector3 NewPos3 = new Vector3(pos.x - 4, 0.4f, pos.y - 5 + (10 * (pos.z - 1)));
        this.World_pos = NewPos3;
        this.pos = pos;
    }

    //move the character
    //used to move on tile
    public void MoveOntile(Vector3 spot)
    {
        this.World_pos = spot;
    }

    //if the mouse is over a card
    public bool MouseOverCards()
    {
        return ItemManager.MouseOver();
    }

    //end the turn
    //hide the cards and reset the items
    public void EndTurn()
    {
        ItemManager.Close();
        foreach (Data item in cards)
        {
            if (!item.passive)
            {
                item.num = 1;
            }
        }
    }

    //when its the start of this characters turn
    public void StartTurn()
    {
        StartCoroutine(StartMessage());
    }

    IEnumerator StartMessage()
    {
        StartScreen = true;
        Message.SetActive(true);
        Message.transform.GetChild(0).GetComponent<Text>().text = "Player " + CharID;

        while (!Input.GetMouseButtonDown(0)) { yield return null; }

        Message.SetActive(false);

        board.GetComponent<Board_prefabs>().CheckAvailabilities(pos, GetStat(1));
        UpdateUI();
        gameObject.GetComponentInChildren<Outline>().enabled = true;
        ItemManager.Open();
        manager.button.Open("End Turn");
        StartedTurn = true;
        StartScreen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Dog != null)
        {
            Dog.update();
        }
        //update the objects position
        transform.position = this.World_pos;
        if (IsTurn)
        {
            //start turn if it hasnt allready
            if (!StartedTurn && !StartScreen)
            {
                StartTurn();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                this.StatsIndex[1] += 1;
                UpdateUI();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                this.StatsIndex[1] -= 1;
                UpdateUI();
            }
        }
        else
        {
            StartedTurn = false;
        }
    }
}

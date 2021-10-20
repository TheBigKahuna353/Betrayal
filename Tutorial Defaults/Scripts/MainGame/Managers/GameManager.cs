 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// This script is for basic mechanics for the game

public class GameManager : MonoBehaviour
{
    //vars
    int Playerturn = 0;
    public Charachter player1;
    public Charachter player2;
    public Charachter player3;
    public Charachter player4;
    private Charachter[] players;
    private Vector3[] TileLocations;
    public TextAsset roomtxt;
    public TextAsset chartxt;
    Navagation Nav;
    public DiceManager dice;
    public EventController events;
    bool EventShowing;
    bool PlayerMoveing;
    public bool ClickToMove = false;
    public bool HauntStarted;
    public Board_prefabs board;
    List<string> TileNames = new List<string>();
    GameObject[] UIstats = new GameObject[4];
    public GameObject statChangePanel;
    string[] TileEffectNames;
    bool TileEffectsFinished = true;
    EventData eventData;

    public Button button;

    // Start is called before the first frame update
    void Awake()
    {
        //setup Arrays
        players = new Charachter[4]
        {
            player1, player2, player3, player4
        };
        TileLocations = new Vector3[4]
        {
            new Vector3(-0.3f, 0, -0.3f),
            new Vector3(-0.3f, 0, 0.3f), 
            new Vector3(0.3f, 0, -0.3f),
            new Vector3(0.3f, 0, 0.3f)
        };
        Nav = new Navagation();

        UIstats[0] = GameObject.Find("Might");
        UIstats[1] = GameObject.Find("Speed");
        UIstats[2] = GameObject.Find("Sanity");
        UIstats[3] = GameObject.Find("Knowledge");

        eventData = GetComponent<EventData>();

    }

    public void DropItem(Data card)
    {
        Tile tile = board.GetTile(player().pos);
        tile.AddDroppedCard(card);
    }

    public bool ShowingEvent()
    {
        return EventShowing;
    }

    //awake is called before start
    private void Start()
    {
        //move players to edge of tile so not all in each other
        this.CheckPlayers();

        player().AddOmen(eventData.getEvent("omen"));
        player().AddOmen(eventData.getEvent("item"));
    }

    //when a tile activates, move the character to that tile
    public void OnTileActivate(Vector3Int pos)
    {
        if (!MouseOverCards())
        {
            Charachter player = players[Playerturn];
            List<Tile> path = GetPath(player.pos, pos);
            board.ResetAvailabilities();
            PlayerMoveing = true;
            player.StartCoroutine(player.Animate(path));
        }
    }

    public Coroutine TileEffect(string tile)
    {
        int i = tile.IndexOf(' ');
        if (i >= 0) tile = tile.Remove(i,1);
        return GetComponent<TileEffects>().StartCoroutine(tile);

    }

    public string TileEffectAction()
    {
        return GetComponent<TileEffects>().effect;
    }

    public void TileEffectStart()
    {
        TileEffectsFinished = false;
    }

    public void StopMoving()
    {
        players[Playerturn].StopCoroutine("Animate");
    }

    public Vector3Int GetPos()
    {
        return player().pos;
    }

    //if the mouse is over a card
    bool MouseOverCards ()
    {
        return players[Playerturn].MouseOverCards();
    }

    //pathfinding
    public List<Tile> GetPath(Vector3Int start, Vector3Int end)
    {
        return Nav.FindPath(start, end);

    }

    public void TileClickedToMove(Vector3Int pos)
    {
        board.ResetAvailabilities();
        StartCoroutine("MovePlayer",pos);
        
    }

    public void TileEffectFinished()
    {
        TileEffectsFinished = true;
        Debug.Log("Tile finished");
    }

    IEnumerator MovePlayer(Vector3Int pos)
    {
        Charachter player = players[Playerturn];
        Tile t = board.GetTile(pos);
        player.ActivateTile(t,board.GetDoorNeighbourSide(pos));
        yield return new WaitForSeconds(1f);
        player.Move(pos);
        ClickToMove = false;
        Tile.ClickToMove = false;
        FinishedMoving(true, t);
    }

    //if the tile has a event/item/omen
    //gets called when tile is activated
    public void EventOnTile(string type)
    {
        if (type != "None")
        {
            Data data = eventData.getEvent(type);
            //if num of dice is < 0, it is a trait roll
            //else roll that num of dice
            if (data.die < 0)
            {
                data.die = GetDice(data.die * -1);
            }
            events.StartEvent(type, data);
            EventShowing = true;
        }
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

    //update stats after event
    public void FinishedEvent()
    {
        events.Panel.SetActive(false);
        EventShowing = false;
    }

    //how many dice you get for stat
    public int GetDice(int stat)
    {
        int num = players[Playerturn].GetStat(stat - 1);
        num += players[Playerturn].CheckItemsPassive(GetTraitName(stat * 10));
        return num;
    }

    //add a card to the current player
    public void AddCard(Data data)
    {
        players[Playerturn].AddItem(data);
    }

    //when an item is clicked/ use it
    public void ItemClicked(Data item)
    {
        Debug.Log("sending item to events");
        events.ItemClicked(item);
    }

    //when an omen is clicked/ use it
    public void OmenClicked(Data omen)
    {
        Debug.Log("Omen");
        if (!omen.passive)
        {
            if (omen.num > 0)
            {
                if (omen.title == "Mask") StartCoroutine("RollMask", omen); EventShowing = true;
                omen.num--;
            }
        }
    }

    //use mask
    IEnumerator RollMask(Data omen)
    {
        dice.RollDice(Vector3.up * 2, GetDice(3));
        while (dice.WaitForRoll) { yield return null; }
        if (dice.Sum >= 4)
        {
            if (omen.trait == "off") omen.trait = "on";
            else omen.trait = "off";
            foreach (int effect in omen.ItemEffect("on"))
            {
                ApplyEffects(effect);
            }
        }
        EventShowing = false;
    }

    //get the name of the trait
    public string GetTraitName(int trait)
    {
        trait = Mathf.Abs(trait);
        if (trait < 20) return "might";
        if (trait < 30) return "speed";
        if (trait < 40) return "sanity";
        return "knowledge";
    }

    //apply the effect of the event/item/omen to the current players stats
    public IEnumerator ApplyEffects(int effect)
    {
        if (Mathf.Abs(effect) < 10) yield break;
        int s = Mathf.Abs(effect) % 10;
        if (s >= 5)
        {
            dice.RollDice(Vector3.up * 2, s - 4);
            while (dice.WaitForRoll)
            {
                yield return null;
            }
            s = dice.Sum;
        }
        if (Mathf.Abs(effect) < 50)
        {
            if (effect < 0) s *= -1;
            StatsChange(Mathf.Abs(effect / 10), s);
        }
        else
        {
            yield return StartCoroutine(UserChangeStats(effect, s));
        }
    }

    //when the player gets to choose what stat to increase/decrease
    public IEnumerator UserChangeStats(int stat, int amount)
    {
        int start = amount;
        int change = 1;
        if (stat < 0) change = -1;
        statChangePanel.SetActive(true);
        string gainLose = "Gain";
        string phyicalMental = "Physical";
        if (Mathf.Abs(stat) > 60) phyicalMental = "Mental";
        if (change == -1) gainLose = "Lose";
        statChangePanel.GetComponentInChildren<Text>().text = gainLose + " " + amount.ToString() + " more " + phyicalMental;
        if (stat < 0) { stat *= -1; }
        while (amount > 0)
        {
            while (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)) { yield return null; }
            for (int i = 0; i < UIstats.Length; i++)
            {
                if (UIstats[i].GetComponent<MouseHover>().Over)
                {
                    if ((i < 2 && stat / 10 == 5) || (i > 1 && stat / 10 == 6))
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            StatsChange(i + 1, 1);
                            amount--;
                        }
                        else if (amount < start)
                        {
                            StatsChange(i + 1, -1);
                            amount++;
                        }
                        statChangePanel.GetComponentInChildren<Text>().text = gainLose + " " + amount.ToString() + " more " + phyicalMental;
                        break;
                    }
                }
            }
            yield return null;
        }
        statChangePanel.SetActive(false);
    }

    //change the current players stat
    public void StatsChange(int stat, int amount)
    {
        players[Playerturn].StatsIndex[stat-1] += amount;
        players[Playerturn].UpdateUI();
    }

    public Charachter player()
    {
        return players[Playerturn];
    }

    //gets a charachters stats
    //used to set up charachters
    public string[] stats(int i)
    {
        string[] ListOfChars = chartxt.text.Split('\n');
        System.Random rnd = new System.Random();

        return ListOfChars[(i*2)-1].Split(',');
    }

    //get a name for the room
    //used once tile is activated
    public string[] GetName(int floor)
    {
        string[] ListOfNames = roomtxt.text.Split('\n');
        string[] Room = new string[4];
        bool Right_floor = false;
        int i = 0;
        int count = 0;
        System.Random rnd = new System.Random();
        while (!Right_floor)
        {
            try
            {
                count++;
                if (count == 100) throw new System.Exception("Max iteration reached");
                if (count > 50)
                {
                    if (count == 51) i = -1;
                    i++;
                }
                else
                {
                    i = rnd.Next(1, ListOfNames.Length);
                }
                Room = ListOfNames[i].Split(',');
                //if (Room[0] != "Chapel") continue; 
                if (Room[floor + 1] == "True" && !TileNames.Contains(Room[0]))
                {
                    Right_floor = true;
                }
            }
            catch (System.Exception)
            {
                Debug.Log(i);
                HandyFuncs.PrintArr<string>(Room);
                throw;
            }
        }
        TileNames.Add(Room[0]);
        return Room;
    }

    //sides
    //0  down
    //1 left
    //2 up
    //3 right
       

    //gets neighbors that are active
    // used for tile activation
    public List<bool> GetCurrentDoors(Vector3Int p)
    {
        return board.getNeighborsDoors(p.z,p.x,p.y);
    }

    //return true if that neighbor is active
    public bool NeighborActive(Vector3Int p, int side)
    {
        return board.GetNeighborActive(p.z, p.x, p.y, side);
    }

    //once the player is finished moving
    //check for events on tile
    public void FinishedMoving(bool e, Tile t)
    {
        PlayerMoveing = false;
        if (e)
        {
            string type = t.Event();
            EventOnTile(type);
        }
    }

    //when the user finishes their turn, change player turns
    void EndTurn()
    {
        Debug.Log("end turn");
        button.CloseAll();
        Charachter player = (Charachter)players[Playerturn];
        player.IsTurn = false;
        GetComponent<TileEffects>().effect = "Null";
        GetComponent<TileEffects>().NewTurn();
        player.gameObject.GetComponentInChildren<Outline>().enabled = false;
        player.EndTurn();
        Playerturn++;
        if (Playerturn == 4)
        {
            Playerturn = 0;
        }
    }

    //checks all players to see if any are on same tile
    //if so, move so dont overlap
    public void CheckPlayers()
    {
        List<Vector3Int> poses = new List<Vector3Int>();

        for (int i = 0; i < 4;i++)
        {
            Charachter p = players[i];
            if (!poses.Contains(p.pos))
            {
                poses.Add(p.pos);
                MovePlayersOnTile(p.pos);
            }
        }
    }


    //move the players on a tile so dont overlap
    void MovePlayersOnTile(Vector3Int pos)
    {
        Charachter[] movees = new Charachter[4];
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            Charachter player = players[i];
            if (player.pos == pos)
            {
                movees[i] = player;
                count++;
            }
        }
        if (count > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                if (movees[i] != null)
                {
                    Vector3 Newspot;
                    if (count == 1)
                    {
                        Newspot = Vector3.zero;
                    }
                    else
                    {
                        Newspot = TileLocations[i];
                    }
                    Vector3 pos3 = new Vector3(movees[i].pos.x - 4, 0.4f, movees[i].pos.y - 5 + (10 * (movees[i].pos.z - 1)));
                    movees[i].MoveOntile(pos3 - Newspot);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if player hasnt started turn, start
        Charachter player = players[Playerturn];
        if (!player.IsTurn)
        {
            player.IsTurn = true;
            PlayerMoveing = false;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log(PlayerMoveing + " " + EventShowing + " " + ClickToMove + " " + TileEffectsFinished);
        }

        //if the player has finished moving and not waiting for an event, endturn
        if (!PlayerMoveing && !EventShowing && !ClickToMove && player.StartedTurn)
        {
            if (!button.ButtonOpen("End Turn")) button.Open("End Turn");
            if (button.clicked.Contains("End Turn"))
            {
                GetComponent<TileEffects>().EndofTurn();
                //Debug.LogWarning("Player moved and not showing event and not cling to move");
                if (TileEffectsFinished)
                {
                    EndTurn();
                }
            }
        }
        else
        {
            button.Close("End Turn");
        }


    }
} 

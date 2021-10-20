using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //vars
    public Vector3Int pos;
    public Material MainMat;
    public bool active = false;
    public bool available = true;
    public bool waiting = false;
    public List<bool> doorLocations;
    GameManager manager;

    public Color Available;
    public Color Idle;

    public Animator anim;

    //for pathfinding
    public int gCost;
    public int hCost;

    public Tile Camefrom;

    public bool IsLanding;

    private Transform parent;
    private GameObject piece;

    string cardType;
    public string effect;
    public static bool ClickToMove = false;

    private List<Data> dropped_cards = new List<Data>();
    private List<GameObject> dropped_cards_objs = new List<GameObject>();

    public GameObject OmenPrefab;
    public GameObject ItemPrefab;

    static System.Random rnd = new System.Random();

    //get the f cost of the tile
    //for pathfinding
    public int fCost()
    {
        return gCost + hCost;
    }


    //get the GameManagaer script
    private void Awake()
    {
        manager = GameObject.Find("Manager").GetComponent<GameManager>();
        piece = transform.GetChild(0).gameObject;
    }


    //Update is called every frame
    //if the tile is active and available/ waiting, lighten the color, else darken it
    private void Update()
    {
        if (this.active)
        {
            if (this.available || this.waiting)
            {
                piece.GetComponent<MeshRenderer>().material.color = Available;
            }
            else
            {
                piece.GetComponent<MeshRenderer>().material.color = Idle;
            }
        }

        if (pos == manager.GetPos())
        {
            if (dropped_cards.Count > 0)
            {
                if (manager.button.clicked.Contains("Pick Up"))
                {
                    foreach (Data card in dropped_cards)
                    {
                        manager.AddCard(card);

                    }
                    dropped_cards.Clear();
                    foreach (GameObject card in dropped_cards_objs)
                    {
                        Destroy(card);
                    }
                    dropped_cards_objs.Clear();
                    manager.button.Close("Pick Up");
                    Debug.Log("Close button");
                }
            }
        }
    }


    //this is called when the board is created
    //sets up the tile upon creation
    public void Setup(Vector3Int pos, string name)
    {
        this.pos = pos;
        if (name == "Inactive")
        {
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        } 
        else if (name == "Empty")
        {
            this.Activate(name,false,0,null,new List<bool>() {true,true,true,true});
        }
        else
        {
            piece.GetComponent<MeshRenderer>().material = MainMat;
            this.name = name;
            parent.GetComponentInChildren<TextMesh>().text = name;
            this.active = true;
        }
    }


    //when the user clicks on this tile, activate it 
    public void OnOver()
    {
        if (this.available)
        {
            if (!manager.ShowingEvent())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    {
                        if (!ClickToMove)
                        {
                            //Debug.Log(pos.ToString());
                            manager.OnTileActivate(this.pos);
                        }
                        else
                        {
                            manager.TileClickedToMove(this.pos);
                        }
                    }
                    else
                    {
                        Debug.Log("UI Click");
                        print(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject);
                    }
                }
            }
        }
    }

    //when the user puts the mouse over he tile, if its availbale to move there, highlight it
    public void OnEnter()
    {

        if (this.available)
        {
            if (!manager.ShowingEvent())
            {
                //if active, outline it instead of changing its colour
                if (this.active)
                {
                    piece.GetComponent<Outline>().enabled = true;
                    parent.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    piece.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }
    }


    //when printing this tile, it prints its pos
    public override string ToString()
    {
        return this.pos.ToString();
    }

    //when the user takes the mouse off, reverse effects
    public void OnExit()
    {
        if (this.available)
        {
            if (!manager.ShowingEvent())
            {
                if (this.active)
                {
                    piece.GetComponent<Outline>().enabled = false;
                    parent.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    piece.GetComponent<MeshRenderer>().enabled = false;

                }
            }
        }
    }

    //if the tile has a event/omen/tile
    public string Event()
    {
        return this.cardType;
    }

    //sides
    //0  down
    //1 left
    //2 up
    //3 right

    //given a list of sides, returns the list with opposite sides in same order
    List<bool> OppositeList(List<bool> l)
    {
        return new List<bool>()
        {
            l[2], l[3], l[0], l[1]
        };
    }

    //given a list of sides, rotates it
    List<bool>RotateList(List<bool> l)
    {
        List<bool> lis = new List<bool>();
        for (int i = 1; i < 5; i++)
        {
            lis.Add(l[(i) % 4]);
        }
        return lis;
    }

    //if 2 lists are the same
    bool TheSame(List<bool> l1, List<bool> l2)
    {
        bool same = true;
        for (int i = 0; i< l1.Count; i++)
        {
            if (l1[i] != l2[i])
            {
                same = false;
            }
        }
        return same;
    }

    //rotate the tile until the doors on the image line up with generated doors
    void RotateTile(string loc)
    {
        string[] locs = loc.Split('-');
        List<bool> AllowedDoorSides = OppositeList(new List<bool>()
            {
                locs[0] == "t", locs[1] == "t", locs[2] == "t", locs[3] == "t"
            });
        for (int x = 1; x <= 5; x++)
        {
            AllowedDoorSides = RotateList(AllowedDoorSides);
            
            if (TheSame(AllowedDoorSides,doorLocations))
            {
                piece.transform.Rotate(0,-90*x,0);
                break;
            }
        }
    }

    //given a list of door sides, transforms it to become in relation of a side
    List<bool> GetDoorSides(int side, List<bool> doorsides)
    {
        List<bool> newdoors = new List<bool>()
        {
            false, false, false, false
        };
        for (int i = 1; i < 4; i++)
        {
            if (doorsides[i])
            {
                newdoors[(i + side) % 4] = true;
            }
        }
        return newdoors;
    }

    //given the raw door side data (in string[] type), returns list of door sides
    List<bool> GetDoorLoc(int side, string[] data)
    {
        doorLocations = new List<bool>()
            {
                false,false,false,false
            };
        int doors = int.Parse(data[5]);
        doorLocations[side] = true;
        if (doors == 1) return doorLocations;
        if (doors == 4)
        {
            for (int i = 0; i < 4; i++) { doorLocations[i] = true; }
            return doorLocations;
        }
        string[] DoorsStr = data[6].Split('-');
        List<bool> OtherDoorSides = manager.GetCurrentDoors(pos);
        List<bool> AllowedDoorSides = GetDoorSides(side, new List<bool>() 
            {
                DoorsStr[0] == "t", DoorsStr[1] == "t", DoorsStr[2] == "t", DoorsStr[3] == "t"
            });
        for (int i = 0; i < doors - 1; i++)
        {
            bool exisingDoor = false;
            for (int index = 0; index < 4; index++)
            {
                if (AllowedDoorSides[index])
                {
                    if (OtherDoorSides[index])
                    {
                        exisingDoor = true;
                        doorLocations[index] = true;
                        AllowedDoorSides[index] = false;
                        break;
                    }
                }
            }
            if (!exisingDoor)
            {
                bool NotActive = false;
                for (int k = 0; k < 4; k++)
                {
                    if (AllowedDoorSides[k])
                    {
                        if (!doorLocations[k] && !manager.NeighborActive(pos,k))
                        {
                            doorLocations[k] = true;
                            NotActive = true;
                            break;
                        }
                    }
                }
                if (!NotActive)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        if (AllowedDoorSides[k])
                        {
                            if (!doorLocations[k])
                            {
                                doorLocations[k] = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
        return doorLocations;
    }


    public void AddDroppedCard(Data card)
    {
        GameObject obj;
        if (card.type == "omen")
        {
            obj = Instantiate(OmenPrefab,transform);;
        }
        else
        {
            obj = Instantiate(ItemPrefab, transform);
        }
        obj.transform.Translate(Vector3.up,Space.Self);
        obj.transform.Rotate(Vector3.up,rnd.Next(0,20) * 9);
        obj.transform.GetChild(0).GetComponent<TextMesh>().text = card.title.Replace(' ', '\n');
        dropped_cards_objs.Add(obj);
        dropped_cards.Add(card);
        if (dropped_cards.Count == 1)
        {
            manager.button.Open("Pick Up");
        }
    }

    public Data[] PickUpCards()
    {
        return new Data[0];
    }


    //waits 1 second while it animates to show name, then hides text
    IEnumerator WaitForActive()
    {
        yield return new WaitForSeconds(1f);
        parent.GetChild(1).gameObject.SetActive(false);
    }

    //when the tile becomes on the board, activate it so the user can see it
    public void Activate(string name, bool animate,int side = -1, string[] data = null, List<bool> doors = null)
    {
        this.parent = transform.parent;
        this.name = name;
        parent.gameObject.name = name;
        this.parent.GetComponentInChildren<TextMesh>().text = name.Replace(" ","\n");
        if (side < 0)
        {
            if (side == -1) IsLanding = true;
            doorLocations = doors;
        }
        else
        {
            doorLocations = GetDoorLoc(side,data);
            if (data[5] != "4")
            {
                RotateTile(data[7]);
            }
            cardType = data[4];
            this.effect = data[8].Remove(data[8].Length-1);
        }
        int count = 0;
        foreach (var item in GetComponentsInChildren<MeshRenderer>())
        {
            if (count < 5 && count > 0 && (bool)doorLocations[count - 1] == true)
            {
                item.enabled = true;
            }
            count++;
 
        }
        piece.GetComponent<MeshRenderer>().enabled = true;
        piece.GetComponent<MeshRenderer>().material = MainMat;
        Texture texture;
        if (name == " ")
        {
            texture = Resources.Load("Rooms/outside") as Texture;
            piece.GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Transparent");
        }
        else
        {
            texture = Resources.Load("Rooms/" + name) as Texture;
        }
        piece.GetComponent<MeshRenderer>().material.mainTexture = texture;
        piece.GetComponent<Outline>().enabled = false;
        this.active = true;
        if (animate)
        {
            anim.SetTrigger("Start");
            StartCoroutine("WaitForActive");
        }
        else
        {
            parent.GetChild(1).gameObject.SetActive(false);
        }
    }

}

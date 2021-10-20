using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    GameObject[] MainMenu = new GameObject[3];
    GameObject[] NewGame = new GameObject[3];
    GameObject[] Quit = new GameObject[3];
    GameObject[] controls = new GameObject[2];
    GameObject[] settings = new GameObject[2];
    GameObject[] All_GO = new GameObject[13];
    string state = "Main Menu";
    public SceneManagerObj sceneManager;

    private void Start()
    {
        MainMenu[0] = GameObject.Find("New Game");
        MainMenu[1] = GameObject.Find("Settings");
        MainMenu[2] = GameObject.Find("Exit");
        NewGame[0] = GameObject.Find("Quick Play");
        NewGame[1] = GameObject.Find("Tutorial");
        NewGame[2] = GameObject.Find("Back");
        Quit[0] = GameObject.Find("Are You Sure");
        Quit[1] = GameObject.Find("Quit Yes");
        Quit[2] = GameObject.Find("Quit No");
        settings[0] = GameObject.Find("Controls");
        settings[1] = GameObject.Find("Back Settings");
        controls[0] = GameObject.Find("Controls view");
        controls[1] = GameObject.Find("Back Controls");

        int count = 0;
        foreach (GameObject obj in NewGame)
        {
            obj.SetActive(false);
            All_GO[count] = obj;
            count++;
        }
        foreach (GameObject obj in controls)
        {
            obj.SetActive(false);
            All_GO[count] = obj;
            count++;
        }
        foreach (GameObject obj in MainMenu)
        {
            All_GO[count] = obj;
            count++;
        }
        foreach (GameObject obj in settings)
        {
            obj.SetActive(false);
            All_GO[count] = obj;
            count++;
        }
        foreach (GameObject obj in Quit)
        {
            obj.SetActive(false);
            All_GO[count] = obj;
            count++;
        }
    }

    public void Click(string button)
    {
        Debug.Log(button);
        state = button;
        if (state == "Back") state = "Main Menu";
        if (state == "Back Settings") state = "Main Menu";
        if (state == "Back Controls") state = "Settings";
        if (state == "Quit Yes") Application.Quit();
        if (state == "Quit No") state = "Main Menu";
        ChangeState();
    }


    void ChangeState()
    {
        if (state == "Main Menu")
        {
            foreach (GameObject obj in All_GO)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in MainMenu)
            {
                obj.SetActive(true);
            }
        }
        if (state == "New Game")
        {
            foreach (GameObject obj in All_GO)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in NewGame)
            {
                obj.SetActive(true);
            }
        }
        if (state == "Settings")
        {
            foreach (GameObject obj in All_GO)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in settings)
            {
                obj.SetActive(true);
            }
        }
        if (state == "Controls")
        {
            foreach (GameObject obj in All_GO)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in controls)
            {
                obj.SetActive(true);
            }
        }
        if (state == "Exit")
        {
            foreach (GameObject obj in All_GO)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in Quit)
            {
                obj.SetActive(true);
            }
        }
        if (state == "Quick Play") SceneManagerObj.Load("MainGame");
    }

}

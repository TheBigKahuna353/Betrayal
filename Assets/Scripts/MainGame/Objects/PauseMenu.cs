using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    bool Pause = false;

    public Board_prefabs board;

    
    public void NewGame()
    {
        SceneManagerObj.Load("MainGame");
    }

    public void Settings()
    {

    }

    public void MainMenu()
    {
        SceneManagerObj.Load("MainMenu");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause = !Pause;
            transform.GetChild(0).gameObject.SetActive(Pause);
            transform.GetChild(1).gameObject.SetActive(Pause);
            transform.GetChild(2).gameObject.SetActive(Pause);
            transform.GetChild(3).gameObject.SetActive(Pause);
            board.TilesWait();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuButton : MonoBehaviour
{

    Manager manager;

    private void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<Manager>();
    }

    public void OnOver()
    {
        gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
    }

    public void OnExit()
    {
        gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
    }

    public void OnClick()
    {
        gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
        manager.Click(gameObject.name);
    }

}

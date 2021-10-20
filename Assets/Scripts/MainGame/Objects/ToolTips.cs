using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTips : MonoBehaviour
{
    public GameObject panel;

    public TextAsset text;

    string GetText(string state)
    {
        string[] lines = text.text.Split('\n');
        foreach (string line in lines)
        {
            string[] items = line.Split('*');
            if (items[0] == state) return items[1];
        }
        return "None";
    }

    public Coroutine Display(string state)
    {
        panel.SetActive(true);
        panel.transform.GetChild(0).GetComponent<Text>().text = GetText(state);
        return StartCoroutine(_Display());
    }

    IEnumerator _Display()
    {
        yield return null;
        while (!Input.GetMouseButtonDown(0)) { yield return null; }
        panel.SetActive(false);
    }

}

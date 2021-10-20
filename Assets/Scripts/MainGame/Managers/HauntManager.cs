using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntManager : MonoBehaviour
{
    public TextAsset haunts;
    

    public int GetHauntNumber(string room, string omen)
    {
        string[] txt = haunts.text.Split('\n');
        string[] columns = txt[0].Split(',');
        int colNum = 0;
        for (int i = 0; i < columns.Length; i++)
        {
            if (columns[i] == omen)
            {
                colNum = i;
                break;
            }
        }
        for (int i = 1; i < txt.Length; i++)
        {
            string[] row = txt[i].Split(',');
            if (row[0] == room)
            {
                return int.Parse(row[colNum]);
            }
        }
        return 0;
    }

}

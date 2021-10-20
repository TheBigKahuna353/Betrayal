using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    //vars
    List<Dice> die = new List<Dice>();

    public GameObject prefab;

    public bool WaitForRoll;
    public bool Rolled;

    public int Sum = 0;

    //setup to rolldice
    public void RollDice(Vector3 pos, int Numdie = 8)
    {
        WaitForRoll = true;
        Rolled = false;
        for (int i = 0; i < Numdie; i++)
        {
            die.Add(Instantiate(prefab,pos,Quaternion.identity).GetComponent<Dice>());
            pos.y += 2;
        }
        StartCoroutine("Check");
    }

    //add dice while currently rolling dice
    public void AddDice(int num, Vector3 pos)
    {
        die.Add(Instantiate(prefab, pos, Quaternion.identity).GetComponent<Dice>());
    }

    //add forces to die to make roll
    void Roll()
    {
        foreach (Dice d in die)
        {
            d.Roll();
        }
    }

    //destroy all dice
    void getRidOfDie()
    {
        foreach (Dice d in die)
        {
            Destroy(d.gameObject);
        }
        die = new List<Dice>();
    }

    //check every die to see if they have landed/ stopped
    void CheckAllDiceStopped()
    {
        bool allDone = true;
        Sum = 0;
        foreach (Dice d in die)
        {
            if (d.rolling)
            {
                d.Check();
                allDone = false;
            }
            else
            {
                Sum += d.Value;
            }
        }
        if (allDone)
        {
            Rolled = true;
        }
    }

    //steps to follow when rolling dice
    IEnumerator Check()
    {
        yield return null;
        while (!Input.GetKeyDown(KeyCode.R)) {yield return null; }
        Roll();
        yield return null;
        while (!Rolled)
        {
            yield return null;
            CheckAllDiceStopped();
        }
        while (!Input.GetMouseButtonDown(0)) { yield return null; }
        getRidOfDie();
        WaitForRoll = false;
    }
}

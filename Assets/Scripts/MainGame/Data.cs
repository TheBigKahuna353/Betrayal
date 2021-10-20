using System.Collections.Generic;
using UnityEngine;
//efects
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

public class Data
{
    public string type;
    public string title;
    public bool passive;
    public int num;
    public string body;
    public string[] FXtxt;
    public int FXnum;
    public int die;
    public string trait;
    public bool droppable;
    private List<(string, int[])> effects = new List<(string, int[])>();

    public Data(string title, string body, string[] FXtxt, int die, string type, bool passive = false)
    {
        this.title = title;
        this.body = body;
        this.die = die;
        this.FXtxt = FXtxt;
        this.passive = passive;
        this.trait = "None";
        this.type = type;
        this.num = 1;
    }

    public void AddEffect(string value, int[] effect)
    {
        this.effects.Add((value, effect));
    }

    public bool CheckOmen(string state)
    {
        foreach ((string, int[]) effect in this.effects)
        {
            if (state == effect.Item1) return true;
        }
        return false;
    }

    public int[] ItemEffect(string state)
    {
        List<string> states = new List<string>() { "attack", "defend", "start", "dropped" };
        foreach ((string, int[]) effect in this.effects)
        {
            if (effect.Item1 == state) return effect.Item2;
            if (effect.Item1 == "allrolls")
            {
                if (state != "attack" && state != "defend") return effect.Item2;
            }
            if (effect.Item1 == "trait")
            {
                if (!states.Contains(state)) return effect.Item2;
            }

        }
        return new int[] { 0 };
    }

    public override string ToString()
    {
        return "Data object: " + this.title;
    }
    public int[] GetEffect(int result)
    {
        this.FXnum = 0;
        foreach ((string, int[]) effect in this.effects)
        {
            if (effect.Item1.Contains("-"))
            {
                string[] sides = effect.Item1.Split('-');
                if (int.Parse(sides[0]) <= result && result <= int.Parse(sides[1]))
                {
                    return effect.Item2;
                }
            }
            else if (effect.Item1.Contains("+"))
            {
                if (result >= int.Parse(effect.Item1.Remove(effect.Item1.Length - 1)))
                {
                    return effect.Item2;
                }
            }
            else
            {
                if (result == int.Parse(effect.Item1))
                {
                    return effect.Item2;
                }
            }
            this.FXnum++;
        }
        return new int[] { 0 };
    }
}

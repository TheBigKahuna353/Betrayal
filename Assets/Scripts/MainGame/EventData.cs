using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventData : MonoBehaviour
{
     System.Random rnd = new System.Random();

    public TextAsset Cardata;

    public  Data getEvent(string type)
    {
        int i = rnd.Next(1, 101);
        //i = 3;
        if (type == "event")
        {
            return Event();
        }
        else if (type == "item")
        {
            if (i <= 50) return Item1();
            return Item2();
        }
        else
        {
            if (i <= 50) return Omen1();
            return Omen2();
        }
    }


    //title, body, Num dice, values, effects 
     Data Event()
    {
        string[] events = Cardata.text.Split('\n');
        string[] card = events[rnd.Next(0, 2)].Split('|');
        events = null;
        Data data = new Data(card[0], card[1], card[2].Split('_'), int.Parse(card[3]), "Event");
        data.trait = card[4];
        string[] effects = card[5].Split('@');
        foreach (string fx in effects)
        {
            string[] parts = fx.Split('!');
            string[] secondstr = parts[1].Split('&');
            int[] secondint = new int[secondstr.Length];
            for (int i = 0; i < secondstr.Length; i++)
            {
                secondint[i] = int.Parse(secondstr[i]);
            }
            data.AddEffect(parts[0], secondint);
        }
        return data;
    }

     Data Item1()
    {
        string title = "Spear";
        string body = "A long stick with a sharp rock at one end";
        string[] effectsText = new string[1] { "Once per turn, Add 1 die to trait rolls" };
        //die is num per turn
        int die = 1;
        Data data = new Data(title, body, effectsText, die, "Item");
        //can be used during trait rolls
        data.AddEffect("trait", new int[1] { 1 });
        data.droppable = true;
        return data;
    }

     Data Item2()
    {
        string title = "Effigy";
        string body = "Hand made with care, the doll is dressed up in a copy of your own clothes";
        string[] effectsText = new string[2] { "Add 1 die to every trait roll, except when attacking or defending", "When dropped, lose 1 in every trait" };
        int die = 0;
        // the true at the end means passive, so checks every trait roll
        Data data = new Data(title, body, effectsText, die, "Item", true) ;
        data.AddEffect("dropped",new int[4] { -11, -21, -31, -41 });
        data.AddEffect("trait", new int[1] { 1 });
        data.droppable = true;
        return data;
    }

     Data Omen1()
    {
        string title = "Mask";
        string body = "A Somber mask to hide your intentions";
        string[] effectsText = new string[3] { "Once per turn, you can attempt a sanity roll to use the mask", "4+  You can put on or take off the mask\nIf you put on the mask, gain 2 knowledge and lose 2 sanity", "If you take the mask off, lose 2 knowledge and gain 2 sanity\n0-3 You cant use the mask" };
        int die = -3;
        Data data = new Data(title, body, effectsText, die, "Omen");
        data.trait = "off";
        data.AddEffect("on", new int[2] { 42, -32 });
        data.AddEffect("off", new int[2] { -42, 32 });
        data.droppable = true;
        return data;
    }

     Data Omen2()
    {
        string title = "Book";
        string body = "An old book";
        string[] effectsText = new string[2] {"Gain 2 knowledge now","Lose 2 knowledge when you lose the book"};
        int die = 0;
        Data data = new Data(title, body, effectsText, die, "Omen");
        data.passive = true;
        data.AddEffect("start", new int[1] { 42 });
        data.AddEffect("lost", new int[1] { -42 });
        data.droppable = true;
        return data;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEffects : MonoBehaviour
{
    public EventController Event;
    DiceManager diceManager;
    GameManager manager;

    public string effect;

    private void Start()
    {
        diceManager = GetComponent<DiceManager>();
        manager = GetComponent<GameManager>();
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
    public bool clicked = false;

    bool EndOfTurn = false;

    public void EndofTurn()
    {
        EndOfTurn = true;
    }

    public void NewTurn()
    {
        EndOfTurn = false;
    }


    public IEnumerator CollapsedRoom()
    {
        Debug.Log("Collapsed Room");

        yield return Event.Tile("Collapsed Room");

        diceManager.RollDice(Vector3.up * 2, manager.GetDice(2));

        while (diceManager.WaitForRoll) { yield return null; }

        int Sum = diceManager.Sum;
        if (Sum >= 5)
        {
            yield break;
        }
        manager.board.ResetAvailabilities();
        manager.board.GetAllAvailabilities(0, false);
        manager.StopMoving();
        GameObject.Find("Main Camera").GetComponent<MoveCamera>().Move();
        Tile.ClickToMove = true;
        manager.ClickToMove = true;
        manager.TileEffectFinished();
    }

    public IEnumerator Attic()
    {
        yield return Event.Tile("Attic");

        diceManager.RollDice(Vector3.up * 2, manager.GetDice(2));

        while (diceManager.WaitForRoll) { yield return null; }

        int Sum = diceManager.Sum;
        if (Sum < 3)
        {
            manager.StatsChange(2, 1);
        }
        manager.TileEffectFinished();
    }

    public IEnumerator CoalChute()
    {
        yield return Event.Tile("Coal Chute");

        manager.player().Move(new Vector3Int(4, 5, 0));

        effect = "stop";
        manager.TileEffectFinished();
    }

    public IEnumerator Catacombs()
    {
        if (manager.player().crossing == false) yield break;
        yield return Event.Tile("Attic");

        diceManager.RollDice(Vector3.up * 2, manager.GetDice(3));

        while (diceManager.WaitForRoll) { yield return null; }

        int Sum = diceManager.Sum;

        if (Sum < 6)
        {
            effect = "stop";
        }
        manager.TileEffectFinished();
    }

    public IEnumerator Chapel()
    {
        manager.TileEffectStart();
        while (!EndOfTurn) { yield return null; }

        Debug.Log("end of chapel");

        yield return Event.Tile("Chapel");

        manager.StatsChange(3, 1);

        EndOfTurn = false;
        manager.TileEffectFinished();
    }

    public IEnumerator Chasm()
    {
        yield return Event.Tile("Chasm");

        diceManager.RollDice(Vector3.up * 2, manager.GetDice(2));

        while (diceManager.WaitForRoll) { yield return null; }

        int Sum = diceManager.Sum;
        if (Sum < 3)
        {
            effect = "stop";
        }
        manager.TileEffectFinished();
    }

    public IEnumerator Crypt()
    {
        manager.TileEffectStart();
        while (!EndOfTurn) { yield return null; }

        yield return Event.Tile("Crypt");

        manager.ApplyEffects(-61);

        EndOfTurn = false;
        manager.TileEffectFinished();
    }

    public IEnumerator FurnaceRoom()
    {
        manager.TileEffectStart();
        while (!EndOfTurn) { yield return null; }

        yield return Event.Tile("Furnace Room");

        manager.ApplyEffects(-55);

        EndOfTurn = false;
        manager.TileEffectFinished();
    }

    public IEnumerator Graveyard()
    {
        yield return Event.Tile("Graveyard");

        diceManager.RollDice(Vector3.up * 2, manager.GetDice(3));

        while (diceManager.WaitForRoll) { yield return null; }

        int Sum = diceManager.Sum;
        if (Sum < 4)
        {
            manager.ApplyEffects(-41);
        }
        manager.TileEffectFinished();
    }

    public IEnumerator Gymnasium()
    {
        manager.TileEffectStart();

        while (!EndOfTurn) { yield return null; }

        yield return Event.Tile("Gymnasium");

        manager.ApplyEffects(21);

        EndOfTurn = false;
        manager.TileEffectFinished();
    }

    public IEnumerator JunkRoom()
    {
        yield return Event.Tile("Junk Room");

        diceManager.RollDice(Vector3.up * 2, manager.GetDice(1));

        while (diceManager.WaitForRoll) { yield return null; }

        int Sum = diceManager.Sum;
        if (Sum < 3)
        {
            manager.ApplyEffects(-21);
        }
        manager.TileEffectFinished();
    }

    public IEnumerator Larder()
    {
        manager.TileEffectStart();
        while (!EndOfTurn) { yield return null; }

        yield return Event.Tile("Larder");

        manager.ApplyEffects(11);

        EndOfTurn = false;
        manager.TileEffectFinished();
    }

    public IEnumerator Library()
    {
        manager.TileEffectStart();

        Debug.Log(EndOfTurn);

        while (!EndOfTurn) { yield return null; }

        Debug.Log("Library finished");

        yield return Event.Tile("Library");

        manager.ApplyEffects(41);

        EndOfTurn = false;
        manager.TileEffectFinished();
    }

    public IEnumerator MysticElevator()
    {
        yield return Event.Tile("Mystic Elevator");
        manager.TileEffectFinished();
    }

    public IEnumerator PentagramChamber()
    {
        yield return Event.Tile("Pentagram Chamber");

        diceManager.RollDice(Vector3.up * 2, manager.GetDice(4));

        while (diceManager.WaitForRoll) { yield return null; }

        int Sum = diceManager.Sum;
        if (Sum < 4)
        {
            manager.ApplyEffects(-31);
        }
        manager.TileEffectFinished();
    }

    public IEnumerator Tower()
    {
        yield return Event.Tile("Tower");

        diceManager.RollDice(Vector3.up * 2, manager.GetDice(1));

        while (diceManager.WaitForRoll) { yield return null; }

        int Sum = diceManager.Sum;
        if (Sum < 3)
        {
            effect = "stop";
        }
        manager.TileEffectFinished();
    }
}

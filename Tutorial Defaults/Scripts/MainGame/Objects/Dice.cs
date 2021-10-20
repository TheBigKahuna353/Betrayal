using UnityEngine;
using System.Collections;

public class Dice : MonoBehaviour
{
    //vars
    public bool rolling = false;

    public int Value;

    Rigidbody rigid;

    //awake is called when object is created
    void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
    }


    //check if the dice is moving
    public void Check()
    {
        if (rigid.velocity.magnitude < 0.1f)
        {
            rolling = false;
            Value = WhichSide();
        }
    }

    //roll the die
    public void Roll()
    {
        RollDice();
    }

    //add forces to make dice roll
    void RollDice()
    {
        rolling = true;
        Vector3 force = Vector3.forward * Random.Range(10f, 100f) + Vector3.up * Random.Range(100.0f, 250.0f) + Vector3.right * Random.Range(-100.0f, 100.0f);
        rigid.AddForce(force,ForceMode.Impulse);
        rigid.AddTorque(force * 10,ForceMode.VelocityChange);

        Vector3 v = Vector3.forward;
        v = Random.rotation * v;
        rigid.AddTorque(v * 25.0f);
    }

    //determine what side is facing up
    //some weird maths from the internet
    //if it is more than 60% facing that direction
    //its that direction
    public int WhichSide()
    {
        if (Vector3.Dot(transform.forward, Vector3.up) > 0.6f)
            return 1;
        if (Vector3.Dot(-transform.forward, Vector3.up) > 0.6f)
            return 1;
        if (Vector3.Dot(transform.up, Vector3.up) > 0.6f)
            return 2;
        if (Vector3.Dot(-transform.up, Vector3.up) > 0.6f)
            return 2;
        if (Vector3.Dot(transform.right, Vector3.up) > 0.6f)
            return 0;
        if (Vector3.Dot(-transform.right, Vector3.up) > 0.6f)
            return 0;
        return -1;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerObj : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //this stops the object from getting destroyed between frames, letting data be carried through.
        DontDestroyOnLoad(gameObject);
    }

    //load a new scene by name
    public static void Load(string name)
    {
        SceneManager.LoadScene(name);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandyFuncs
{
    
    public static void PrintList<T>(List<T> obj)
    {
        if (obj != null)
        {
            string str = "";
            foreach (T item in obj)
            {

                str += item.ToString() + ", ";

            }
            Debug.Log(str);
        }
        else
        {
            Debug.Log(null);
        }
    }


    public static void print(object obj)
    {
        Debug.Log(obj.ToString());
    }

    public static void PrintArr<T>(T[] obj)
    {
        if (obj != null)
        {
            string str = "";
            foreach (T item in obj)
            {
                str += item.ToString() + ", ";
            }
            Debug.Log(str);
        }
        else
        {
            Debug.Log(null);
        }
    }






}

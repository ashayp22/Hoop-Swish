using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour {


    public void setLocation(Vector2 pos) //set location
    {
        transform.position = pos;
    }

    public Vector2 getPos() //get the pos
    {
        return transform.position;
    }

    
}

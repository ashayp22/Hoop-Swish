using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

    public void setLocation(Vector2 pos) //set location
    {
        transform.position = pos;
    }

    public Vector2 getPos() //get pos
    {
        return transform.position;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    public Rigidbody2D rb;
    public bool holding = false; //is holding onto the ball and firing

    public bool inAir = true; //if the ball is in the air


	// Use this for initialization
	void Start () {

    }

    // Update is called once per frame
    void Update () {
        if (inAir == false) //if the ball is resting you can click it
        {
            onClick();
        }
        else if (inAir == true) //if the ball is in the air it rotates
        {
            rotate();
        }
	}

    public void rotate() //rotates ball 5 degrees every frame
    {
        if(inAir)
        {
            this.transform.Rotate(0, 0, 5);
        }
    }

    public void setLocation(Vector2 pos) //set location
    {
        transform.position = pos;
    }

    public Vector2 getPos() //get pos
    {
        return transform.position;
    }

    public Vector2 original; //where you started dragging
    private Vector2 after; //picked up drag


    private void hideDots()
    {
        for (int i = 0; i < 5; i++) //hides all of them dots
        {
            GameObject dot = this.transform.GetChild(i).gameObject; //gets actual physical plane of gameobject
            dot.active = false;
        }
    }


    private void onClick() //click the ball
    {
        Vector2 ballCoor = Camera.main.WorldToScreenPoint(this.transform.position); //screen position of the ball
        Vector2 mouseCoor = Input.mousePosition; //mouse position
        //check if mouse Coor is within circle radius using circle conic formula, with x and y being the mouse coordinates and center being the ball coordinates
        bool isWithin = Mathf.Pow(mouseCoor.x - ballCoor.x, 2) + Mathf.Pow(mouseCoor.y - ballCoor.y, 2) <= Mathf.Pow(75, 2);
        if (Input.GetMouseButtonDown(0) && isWithin && holding == false) //clicked on button
        {
            holding = true; //the person is holding the ball
            original = mouseCoor; //initial mouse coordinate
        } else if(Input.GetMouseButtonUp(0) && holding == true) //firing shot here/let go
        {
            hideDots(); //hides all dots
            //makes ball able to move
            rb.isKinematic = false;
            holding = false;
            after = mouseCoor;
            //direction of Vector
            
            float xDif = original.x - after.x;
            float yDif = original.y - after.y;

            if(original.y - after.y >= 200) //makes sure the y direction isn't maxed out
            {
                yDif = 199.9f;
            } else if(original.y - after.y <= -200)
            {
                yDif = -199.9f;
            }

            if (original.x - after.x >= 200) //makes sure the x direction isn't maxed out
            {
                xDif = 199.9f;
            }
            else if (original.x - after.x <= -200)
            {
                xDif = -199.9f;
            }

            Debug.Log(xDif);
            Debug.Log(yDif);

            rb.AddForce(new Vector2(xDif/1, yDif/1), ForceMode2D.Impulse); //adds a force
            inAir = true; //ball is now in the air

        } else if(holding == true) //draw the dots here
        {
            hideDots();

            //find distance between original point and current point
            float differenceY = Mathf.Pow(original.y - Input.mousePosition.y, 2);
            float differenceX = Mathf.Pow(original.x - Input.mousePosition.x, 2);
            float difference = Mathf.Sqrt(differenceX + differenceY);

            if (difference >= 200) //makes sure difference isn't maxed out
            {
                difference = 199.9f; 
            }

            float numDots = Mathf.Floor(difference / 40) + 1; //gets number of dots

            for(int i = 0; i < numDots; i++) //shows necessary dots
            {
                GameObject dot = this.transform.GetChild(i).gameObject; //gets actual physical plane of gameobject
                dot.active = true;
            }

        }
    }
}

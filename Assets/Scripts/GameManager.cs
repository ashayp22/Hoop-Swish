using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    //prefabs
    public Ball ballPrefab;
    public Basket basketPrefab;
    public Line linePrefab;
    public AI aiPrefab;


    //ball
    private Ball ball;

    //ai 
    private AI ai;

    //basket and line lists
    public List<Basket> basketList = new List<Basket>(); //0 index is current hoop, 1 index is target
    public List<Line> lineList = new List<Line>();


    private int currentBasket = -1; //basket it is on/score

    private Rigidbody2D ballRb; //rigidbody of the ball
    private Rigidbody2D aiRb; //rigidbody of the ball


    public Camera cam; //camera

    public Text scoreText; //score text

    private bool alreadyRemoved = true; //basket has already been removed, true if it can remove false if it can't
    private int stuck = 0;

    private int turn = 3; //1 is player, 2 is ai, 3 is both

    // Use this for initialization
    void Start () {
        
        //adds a basket
        basketList.Add(Instantiate(basketPrefab) as Basket);
        basketList[0].setLocation(new Vector2(Random.Range(-10, 10), 0));

        //adds a ball
        ball = Instantiate(ballPrefab) as Ball;
        ball.setLocation(new Vector2(basketList[0].getPos().x, 4));

        ai = Instantiate(aiPrefab) as AI;
        ai.setLocation(new Vector2(basketList[0].getPos().x, 4));

        Debug.Log(Random.Range(0, 1));

        for (int i = 0; i < 8; i++) //adds 4 lines both sides
        {
            lineList.Add(Instantiate(linePrefab) as Line);
            if(i % 2 == 0)
            {
                lineList[i].setLocation(new Vector2(-15, (i/2) * 15 - 15));
            } else
            {
                lineList[i].setLocation(new Vector2(15, Mathf.Floor(i/2) * 15 - 15));
            }
        }


        ballRb = ball.GetComponent<Rigidbody2D>(); //gets rigidbody of ball
        ball.holding = false; //makes it so that the ball isn't being held onto
        aiRb = ai.GetComponent<Rigidbody2D>();

        //ball.setLocation(new Vector2(-10, 0));
        //ai.setLocation(new Vector2(-10, 0));
        //basketList[0].setLocation(new Vector2(-10, 0));


    }



    public void rotateBasket(int player)
    {
        if(player == 1 && ball.holding == true) //if the ball is being held onto
        {
            Vector2 origin = ball.original; //balls position
            Vector2 current = Input.mousePosition; //mouse position
            float theta = Mathf.Atan2((current.y - origin.y), (current.x - origin.x)); //find angle theta
            theta = theta * (180 / Mathf.PI); //convert to degrees

            if(theta < 0) //since quadrants 3 and 4 go from -180 - 0, 360 is added to get its coterminal positive value
            {
                theta += 360; //add 360
            }

            theta -= 90; //adjusts for the way unity has degrees set up, with (0, 1) being 0

            theta += 180; //gets opposite direction, since you are dragging away

            basketList[0].transform.rotation = Quaternion.Euler(0, 0, theta); //rotates the basket
            ball.transform.rotation = Quaternion.Euler(0, 0, theta); //rotates the ball
        } else if(player == 2)
        {
            float theta = Mathf.Atan2(ai.vector.y, ai.vector.x); //find angle theta
            theta = theta * (180 / Mathf.PI); //convert to degrees

            if (theta < 0) //since quadrants 3 and 4 go from -180 - 0, 360 is added to get its coterminal positive value
            {
                theta += 360; //add 360
            }

            theta -= 90; //adjusts for the way unity has degrees set up, with (0, 1) being 0

            basketList[0].transform.rotation = Quaternion.Euler(0, 0, theta); //rotates the basket
        }
    }

    public void ResetBasket()
    {
        basketList[0].transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private bool checkMadeHoop(int player) //checks if you made the shot
    {
        //gets x and y position of the ball
        float ballY = ball.transform.position.y;
        float ballX = ball.transform.position.x;

        float aiX = ai.transform.position.x;
        float aiY = ai.transform.position.y;

        //checks if the ball is within a certain range of the basket
        //checks with one above because 0 index hoop hasn't been destroyed yet
        if (player == 1 && currentBasket != -1)
        {
            if (ballY <= basketList[1].getPos().y - 0.9 && ballY >= basketList[1].getPos().y - 1 && ballX >= basketList[1].getPos().x - 0.2 && ballX <= basketList[1].getPos().x + 0.2)
            {
                //freezes the ball
                ballRb.isKinematic = true;
                ballRb.velocity = Vector3.zero;
                //the ball isn't in the air anymore
                ball.inAir = false;


                turn = 2;
                return true; //returns true for hoop being made
            } else if(ballY <= basketList[0].getPos().y - 0.9 && ballY >= basketList[0].getPos().y - 1 && ballX >= basketList[0].getPos().x - 0.2 && ballX <= basketList[0].getPos().x + 0.2)
            {
                ballRb.isKinematic = true;
                ballRb.velocity = Vector3.zero;
                resetGame();
                Debug.Log("in wrong hoop");
                return true;
            }
            return false;
        //player goes back into original hoop
        } else if(player == 1 && currentBasket == -1 && ballY <= basketList[0].getPos().y - 0.9 && ballY >= basketList[0].getPos().y - 1 && ballX >= basketList[0].getPos().x - 0.2 && ballX <= basketList[0].getPos().x + 0.2) {
                ballRb.isKinematic = true;
                ballRb.velocity = Vector3.zero;
                //the ball isn't in the air anymore
                ball.inAir = false;

                return true; //returns true for hoop being made
            
        } else if(player == 2 && aiY <= basketList[0].getPos().y - 0.9 && aiY >= basketList[0].getPos().y - 1 && aiX >= basketList[0].getPos().x - 0.2 && aiX <= basketList[0].getPos().x + 0.2)
        {
            aiRb.isKinematic = true;
            aiRb.velocity = Vector3.zero;
            //the ball isn't in the air anymore
            ai.inAir = false;

            currentBasket += 1;
            setText(); //sets the text

            //creates a new basket
            basketList.Add(Instantiate(basketPrefab) as Basket);
            basketList[1].setLocation(new Vector2(Random.Range(-10, 10), ((currentBasket + 1) * 10)));
            //makes sure it isn't directly above the previous basket
            while (basketList[1].getPos().x > basketList[0].getPos().x - 7 && basketList[1].getPos().x < basketList[0].getPos().x + 7)
            {
                basketList[1].setLocation(new Vector2(Random.Range(-10, 10), ((currentBasket + 1) * 10)));
            }


            //adds two new lines
            lineList.Add(Instantiate(linePrefab) as Line);
            lineList.Add(Instantiate(linePrefab) as Line);
            lineList[lineList.Count - 2].setLocation(new Vector2(-15, (currentBasket * 15) + 45));
            lineList[lineList.Count - 1].setLocation(new Vector2(15, (currentBasket * 15) + 45));

            turn = 1;

            //basketList[1].setLocation(new Vector2(10, 10));


            return true;
        }
        return false; //returns false for hoop not being made
    }
    
    private bool checkMissHoop(int player) //checks if the person missed the shot
    {
        if(player == 1 && ball.getPos().y <= currentBasket * 10 - 5) //ball is below original hoop
        {
            ballRb.isKinematic = true;
            ballRb.velocity = Vector3.zero;
            resetGame();
            return true;
        } else if(player == 1 && stuck >= 600) //ball is stuck
        {
            ballRb.isKinematic = true;
            ballRb.velocity = Vector3.zero;
            resetGame();
            return true;
        }

        if(player == 2 && ai.getPos().y <= currentBasket * 10 - 5)
        {
            aiRb.isKinematic = true;
            aiRb.velocity = Vector3.zero;
            resetGame();
            return true;
        }

        return false;
    }

    private void resetGame()
    {
        //resets all of GameManager variables
        currentBasket = -1;
        stuck = 0;

        for(int i = 0; i < basketList.Count; i++)
        {
            Destroy(basketList[i].gameObject);
        }

        for (int i = 0; i < lineList.Count; i++)
        {
            Destroy(lineList[i].gameObject);
        }

        basketList = new List<Basket>(); 
        lineList = new List<Line>();

        scoreText.text = "0"; //sets the score to be 0

        alreadyRemoved = true;
        turn = 3;

        //resets all of the balls variables
        ball.holding = false; 
        ball.inAir = true;

        //creates everything else
        //basket
        basketList.Add(Instantiate(basketPrefab) as Basket);
        basketList[0].setLocation(new Vector2(Random.Range(-10, 10), 0));
        //ball

        ball.setLocation(new Vector2(basketList[0].getPos().x, 4));
        ai.setLocation(new Vector2(basketList[0].getPos().x, 4));

        //line
        for (int i = 0; i < 8; i++) //adds 4 lines both sides
        {
            lineList.Add(Instantiate(linePrefab) as Line);
            if (i % 2 == 0)
            {
                lineList[i].setLocation(new Vector2(-15, (i / 2) * 15 - 15));
            }
            else
            {
                lineList[i].setLocation(new Vector2(15, Mathf.Floor(i / 2) * 15 - 15));
            }
        }

        cam.transform.position = new Vector3(0, 5, -10);//resets the camera

        ballRb.isKinematic = false; //lets the ball fall
        aiRb.isKinematic = false;

    }

    private void moveCamera() //move the camera based off of the ball
    {
        cam.transform.position = new Vector3(0, ball.transform.position.y + 3, -10);
    }

    

    private void removeBelow() //removes the basket from below
    {
        if(alreadyRemoved && currentBasket != -1) //makes sure it isn't the first basket and it hasn't been removed yet
        {
            alreadyRemoved = false; //so it won't remove any other baskets
            Destroy(basketList[0].gameObject); //destroys the basket
            //basketList[0].gameObject.active = false; 
            basketList.RemoveAt(0); //removes from list

        }
    }

    // Update is called once per frame
    void Update () {
        if (turn == 1)
        {
            if (ball.inAir == true) //ball is moving in the air, starts with this
            {
                ResetBasket();
                if (checkMadeHoop(1) == false && checkMissHoop(1) == false) //as long as the ball isn't in the basket yet
                {
                    //moves the camera and removes the hoop below
                    moveCamera();
                    stuck += 1;
                }
            }
            else if (ball.inAir == false) //ball is resting
            {
                rotateBasket(1); //rotates the basket
                stuck = 0;
            }
        } else if(turn == 2)
        {
            if(ai.inAir == true)
            {
                //decide who goes last, because based off of that the remove below would be added
                //also add stuck
                if(checkMadeHoop(2) == false && checkMissHoop(2) == false) //ball is in the air
                {
                    removeBelow();
                }
            } else if(ai.inAir == false)
            {
                //here you would add code for getting vector
                aiRb.isKinematic = false;

                ai.calcVector(basketList[1].transform.position.x - basketList[0].transform.position.x, 1);
                rotateBasket(2);
                aiRb.AddForce(ai.vector, ForceMode2D.Impulse);

                ai.inAir = true;
                alreadyRemoved = true; //sets that the lower basket can be removed once released

                

            }
        } else if(turn == 3)
        {
            if(checkMadeHoop(1) && checkMadeHoop(2))
            {
                turn = 1;
            }
        }

    }


    private void setText() //sets the text
    {
        scoreText.text = currentBasket.ToString(); ;
    }
    
}

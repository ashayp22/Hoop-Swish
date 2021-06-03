using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

    public bool inAir = true;
    public Vector2 vector;
    public Vector2 velocity;


    private void Start()
    {
        vector = new Vector2(0, 0);
    }

    private void Update()
    {
        if (inAir == true)
        {
            rotate();
        } else if (inAir == false)
        {

        }
    }

    public void rotate() //rotates ball 5 degrees every frame
    {
        if (inAir)
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

    public void calcVector(float xDif, int method)
    {

        Debug.Log("x " + xDif);
        if (method == 1) {
            vector.x = (float)(2.34 * xDif); //2.67

            float inside1 = 1 - ((Mathf.Pow(xDif, 2) / 36));
            Debug.Log(inside1);
            float inside2 = inside1 * (-2916 / 91); //or -364, -64
            Debug.Log(inside2);
            inside2 = Mathf.Sqrt(inside2);
            inside2 += 154;
            Debug.Log(inside2);
            vector.y = inside2;
        } else if (method == 2)
        {

        }

        vector.x = vector.x / 1;
        vector.y = vector.y / 1;


        Population pop = new Population();
        pop.setUp(vector.x, vector.y);
        
        for(int i = 0; i < 10; i++)
        {
            pop.generation();
        }
        
        pop.sort();

        List<float> top = pop.topPos();

        float temp1 = top[0];
        float temp2 = top[1];

        Debug.Log(vector.x + " vs " + temp1);
        Debug.Log(vector.y + " vs " + temp2);

        vector.x = temp1;
        vector.y = temp2;

        
    }



    private class Chromosome {
        private float cost;
        private float x = 0;
        private float y = 0;

        public void setUp()
        {
            x = Random.Range(-199, 199);
            y = Random.Range(0, 199);
        }


        public void calcCost(float targetX, float targetY)
        {
            cost = 0;
            cost += (400 - Mathf.Abs(targetX - x)) * 5;
            cost += (400 - Mathf.Abs(targetY - y)) * 5;
        }

        public List<float> mate()
        {
            List<float> pos = new List<float>();
            pos.Add(x);
            pos.Add(y);
            return pos;
        }

        public void mutate()
        {
            if(Random.Range(0, 100) < 30)
            {
                x += Random.Range(-0.5f, 0.5f);
                y += Random.Range(-0.5f, 0.5f);
                if(x >= 198)
                {
                    x = 199.99f;
                } else if(x <= -198)
                {
                    x = -199.99f;
                }

                if(y >= 198)
                {
                    y = 199.99f;
                }
            }
        }
    
        public float getCost()
        {
            return cost;
        }

        public void setX(float newx)
        {
            x = newx;
        }

        public void setY(float newy)
        {
            y = newy;
        }

    }


    private class Population
    {

        private float bestX;
        private float bestY;
        private List<Chromosome> chromeList = new List<Chromosome>();
        private int genNum = 0;

        public void setUp(float x, float y)
        {
            bestX = x;
            bestY = y;
            for(int i = 0; i < 10; i++)
            {
                Chromosome chrome = new Chromosome();
                chrome.setUp();
                chromeList.Add(chrome);
            }
        }

        public void sort()
        {
            chromeList.Sort((a, b) => (a.getCost().CompareTo(b.getCost())));
        }
        
        public void generation()
        {
            for(int i = 0; i < chromeList.Count; i++)
            {
                chromeList[i].calcCost(bestX, bestY);
            }

            sort();

            List<float> pos1 = chromeList[chromeList.Count - 1].mate();
            List<float> pos2 = chromeList[chromeList.Count - 1].mate();

            chromeList[0].setX(pos1[0]);
            chromeList[0].setY(pos1[1]);

            chromeList[1].setX(pos2[0]);
            chromeList[1].setY(pos2[1]);

            for (int i = 0; i < chromeList.Count; i++)
            {
                chromeList[i].mutate();
                chromeList[i].calcCost(bestX, bestY);
            }
            genNum += 1;
        }

        public List<float> topPos() {
            return chromeList[chromeList.Count-1].mate();
        }

        
    }


}

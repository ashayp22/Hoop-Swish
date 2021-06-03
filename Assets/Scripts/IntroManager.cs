using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
        if(PlayerPrefs.HasKey("coins") == false)
        {
            PlayerPrefs.SetInt("coins", 0);
            PlayerPrefs.SetInt("level", 1); //multiply times three

        }

    }
	
}

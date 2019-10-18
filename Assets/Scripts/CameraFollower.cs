using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour {
    public GameObject playerOne;
    public GameObject playerTwo;

	// Use this for initialization
	void Start () {
        playerOne = GameObject.FindWithTag("PlayerOne");
        playerTwo = GameObject.FindWithTag("PlayerTwo");
        //myObj = gameObj.transform;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 targetPos;
        if (playerOne.transform.position.y > playerTwo.transform.position.y) // if player one is ahead of player two, follow player one 
            targetPos = playerOne.transform.position;
        else
            targetPos = playerTwo.transform.position;
        targetPos.z = transform.position.z;
        transform.position = targetPos;
	}
}

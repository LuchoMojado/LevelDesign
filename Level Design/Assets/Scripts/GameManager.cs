using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager gameManager;
    public Player player;
    public Vector3 checkPointPos;
    public float countObjects = 0;
    void Start()
    {
        checkPointPos = player.transform.position;
        gameManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.transform.position.y < -75)
        {
            player.transform.position = checkPointPos;
        }
    }

    public void takeObject()
    {
        countObjects++;
        if(countObjects >= 9)
        {
            Debug.Log("GANASTE");
        }
    }
}

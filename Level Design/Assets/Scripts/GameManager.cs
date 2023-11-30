using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public Player player;
    public Vector3 checkPointPos;
    public float countObjects = 0;
    public Rewind[] rewinds;
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
        float countDownSave=0;
        countDownSave += Time.deltaTime;
        if(countDownSave >= 300f)
        {
            foreach(var item in rewinds)
            {
                item.Save();
            }
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

    public void LoadGame()
    {
        foreach (var item in rewinds)
        {
            item.Load();
        }
    }
}

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
    void Awake()
    {
        if (gameManager == null) gameManager = this;
        else Destroy(this);
    }
    void Start()
    {
        checkPointPos = player.transform.position;
        gameManager = this;
        StartCoroutine(CoroutineSave());
    }

    // Update is called once per frame
    void Update()
    {
        if(player.transform.position.y < -75)
        {
            player.transform.position = checkPointPos;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (var item in rewinds)
            {
                item.Load();
            }
        }
        //float countDownSave=0;
        //countDownSave += Time.deltaTime;
        //if(countDownSave >= 300f)
        //{
        //    foreach(var item in rewinds)
        //    {
        //        item.Save();
        //    }
        //    countDownSave = 0;
        //}
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
    IEnumerator CoroutineSave()
    {
        var WaitForSeconds = new WaitForSeconds(0.01f);

        while (true)
        {
            foreach (var item in rewinds)
            {
                item.Save();
            }

            yield return WaitForSeconds;
        }
    }
}

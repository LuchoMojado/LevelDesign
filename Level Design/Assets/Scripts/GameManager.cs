using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public System.Action Pause;
    public Player player;
    public Vector3 checkPointPos;
    public float countObjects = 0;
    public Rewind[] rewinds;
    public string sceneToLoad;
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }
    void Start()
    {
        checkPointPos = player.transform.position;
        instance = this;
        StartCoroutine(CoroutineSave());
    }

    // Update is called once per frame
    void Update()
    {
        if(player.transform.position.y < -75)
        {
            Respawn();
        }
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    foreach (var item in rewinds)
        //    {
        //        item.Load();
        //    }
        //}
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

    public void Respawn()
    {
        player.transform.position = checkPointPos;
    }

    public void takeObject()
    {
        countObjects++;
        if(countObjects >= 10)
        {
            Debug.Log("GANASTE");
            SceneManager.LoadScene(sceneToLoad);
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
    public void PauseGame()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UIManager.instance.SetPauseMenu(true);
    }
    public void UnpauseGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UIManager.instance.SetPauseMenu(false);
        Pause();
    }
}

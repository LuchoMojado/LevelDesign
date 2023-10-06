using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyO : MonoBehaviour, IObserver
{
    public PlayerO player;

    //CUANDO EL PLAYER CAE A SIERTO RANGO (CERCA POR EL SIGILO) ESTOS SE ENTEREN
    public void Notify(Vector3 pos)
    {
        Debug.Log("Te escuche y te persigo");
    }

    // Start is called before the first frame update
    void Start()
    {
        player.Subscribe(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

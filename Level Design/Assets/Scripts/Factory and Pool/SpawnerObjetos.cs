using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerObjetos : MonoBehaviour
{
    public ImportantObjects objPrefab;
    public Factory<ImportantObjects> factory;
    ObjectPool<ImportantObjects> myPool;
    public List<GameObject> listSpots = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        factory = new Factory<ImportantObjects>(objPrefab);
        myPool = new ObjectPool<ImportantObjects>(factory.GetObject, ImportantObjects.TurnOff, ImportantObjects.TurnOn, 10);
        for(int i = 0; i < listSpots.Count; i++)
        {
            var x = myPool.Get();
            x.Initialize(myPool);
            x.transform.position = listSpots[i].transform.position;
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3End : MonoBehaviour
{
    [SerializeField] string _nextLevel;
    private void OnTriggerEnter(Collider other)
    {
        GameManager.instance.SkipLevel(_nextLevel);
    }
}

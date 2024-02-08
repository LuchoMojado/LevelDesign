using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePoolManager : MonoBehaviour
{
    public GameObject particlePrefab;
    public int poolSize = 10;
    public List<Transform> spawnPoints = new List<Transform>();

    private Queue<GameObject> particlePool = new Queue<GameObject>();

    void Start()
    {
        InitializePool();
        ActivateParticles();
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            particle.SetActive(false);
            particlePool.Enqueue(particle);
        }
    }

    void ActivateParticles()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject particle = GetParticleFromPool();
            if (particle != null)
            {
                particle.transform.position = spawnPoint.position;
                particle.SetActive(true);
            }
        }
    }

    GameObject GetParticleFromPool()
    {
        if (particlePool.Count > 0)
        {
            GameObject particle = particlePool.Dequeue();
            return particle;
        }
        else
        {
            Debug.LogWarning("Particle pool is empty. Consider increasing the pool size.");
            return null;
        }
    }

    public void ReturnParticleToPool(GameObject particle)
    {
        particle.SetActive(false);
        particlePool.Enqueue(particle);
    }

}

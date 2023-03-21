using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSpawnManager : MonoBehaviour
{
    [SerializeField] GameObject fireball;
    [SerializeField] private float startDelay = 0.5f;
    [SerializeField] private float spawnInterval = 3;
    [SerializeField] private float spawnRangeXNegative = -800.0f;
    [SerializeField] private float spawnRangeX = -600;

    private Vector3 spawnpos;


    

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnFireball", startDelay, spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnFireball()
    {
        spawnpos = new Vector3(Random.Range(spawnRangeXNegative, spawnRangeX), -1, -15);
        
        Instantiate(fireball, spawnpos, fireball.transform.rotation);
    }
}

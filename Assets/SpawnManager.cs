using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] int spawnCount = 10;
    void Start()
    {
        var spawnPoints = GetComponentsInChildren<SpawnPoint>(true);   
    }

    void Update()
    {
        
    }
}

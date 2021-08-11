using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    GameObject monsterGo;
    [SerializeField] int spawnCount = 10;

    void Start()
    {
        monsterGo = (GameObject)Resources.Load("Zombie");

        var spawnPoints = GetComponentsInChildren<SpawnPoint>(true);
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            Instantiate(monsterGo, spawnPoint, Quaternion.identity);
        }
    }
}

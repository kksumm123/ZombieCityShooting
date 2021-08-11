using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : SingletonMonoBehavior<SpawnManager>
{
    [SerializeField] int currentWave = 0;
    [SerializeField] List<WaveInfo> waves;

    public float nextWaveTime;

    IEnumerator Start()
    {
        LightManager LightManager = FindObjectOfType<LightManager>();
        var spawnPoints = GetComponentsInChildren<SpawnPoint>(true);
        foreach (var currentWaveInfo in waves)
        {
            currentWave++;
            int spawnCount = currentWaveInfo.spawnCount;
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
                Instantiate(currentWaveInfo.monsterList
                                            .OrderBy(x => Random.Range(0, x.ratio))
                                            .Last().monster
                          , spawnPoint, Quaternion.identity);
            }
            nextWaveTime = Time.time + currentWaveInfo.time;

            while (Time.time < nextWaveTime)
                yield return null;

            LightManager.ToggleDayLight();
        }
    }
    [System.Serializable]
    public class WaveInfo
    {
        public List<ReGenMonsterInfo> monsterList;
        public int spawnCount = 10;
        public float time;
    }
    [System.Serializable]
    public class ReGenMonsterInfo
    {
        public GameObject monster;
        public float ratio;
    }


}

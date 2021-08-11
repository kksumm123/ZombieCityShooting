using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] int currentWave = 0;
    [SerializeField] List<WaveInfo> waves;

    IEnumerator Start()
    {
        LightManager LightManager = FindObjectOfType<LightManager>();
        var spawnPoints = GetComponentsInChildren<SpawnPoint>(true);
        foreach (var item in waves)
        {
            currentWave++;
            int spawnCount = item.spawnCount;
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
                Instantiate(item.monsterGo, spawnPoint, Quaternion.identity);
            }

            yield return new WaitForSeconds(item.time);
            LightManager.ToggleDayLight();
        }
    }
    [System.Serializable]
    public class WaveInfo
    {
        public GameObject monsterGo;
        public int spawnCount = 10;
        public float time;
    }
    //[System.Serializable]
    //public class RegionInfo
    //{
    //    public GameObject monsterGo;
    //    public float ratio;
    //}


}

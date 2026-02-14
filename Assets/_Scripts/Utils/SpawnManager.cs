using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] objectsToSpawn;

    void Start()
    {
        List<int> usedIndexes = new List<int>();

        for (int i = 0; i < objectsToSpawn.Length; i++)
        {
            int randomIndex;

            do
            {
                randomIndex = Random.Range(0, spawnPoints.Length);
            }
            while (usedIndexes.Contains(randomIndex));

            usedIndexes.Add(randomIndex);

            Instantiate(
                objectsToSpawn[i],
                spawnPoints[randomIndex].position,
                Quaternion.identity
            );
        }
    }
}
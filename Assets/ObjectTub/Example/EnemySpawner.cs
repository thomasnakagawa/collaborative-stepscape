using ObjectTub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject EnemyPrefab = default;
    [SerializeField] private float SpawnRadius = 15f;
    [SerializeField] private float SpawnInterval = 3f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawnAtInterval(SpawnInterval));
    }

    private IEnumerator spawnAtInterval(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            float angle = Random.Range(0f, Mathf.PI * 2);
            float x = Mathf.Sin(angle) * SpawnRadius;
            float y = 0f;
            float z = Mathf.Cos(angle) * SpawnRadius;

            GameObject newEnemy = ObjectPool.TakeObjectFromTub(EnemyPrefab);
            newEnemy.transform.position = (transform.position + new Vector3(x, y, z));
        }
    }
}

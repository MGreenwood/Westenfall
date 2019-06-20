using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public enum EnemyTypes
    {
        Basic = 0,
        Hard = 1,
        Dangerous = 2,
        Rare = 3,
        MiniBoss = 4
    }

    List<Enemy> _possibleEnemies = new List<Enemy>();

    [SerializeField]
    EnemyTypes _spawnType = 0;

    [SerializeField]
    float _rareSpawnChance = 0f;

    List<Enemy> possible;

    public void SpawnEnemies(ref List<Enemy> possible_)
    {
        possible = possible_;
        StartCoroutine(DelaySpawn());
    }

    void Spawn()
    {
        for (int e = possible.Count - 1; e >= 0; e--)
        {
            if (possible[e]._enemyType == _spawnType)
            {
                _possibleEnemies.Add(possible[e]);
            }
        }

        if (_possibleEnemies.Count == 0) // this will never happen once enemies of each type are added to the generator
            return;

        int index = UnityEngine.Random.Range(0, _possibleEnemies.Count);

        Instantiate(_possibleEnemies[index],
            transform.position + new Vector3(0, _possibleEnemies[index].transform.localScale.y, 0),
            Quaternion.identity,
            transform);
    }

    IEnumerator DelaySpawn() // allow 
    {
        yield return new WaitForSeconds(1);

        Spawn();
    }
}

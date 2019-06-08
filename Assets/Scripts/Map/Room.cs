using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public struct Path
    {
        public Transform path;
        public bool used;

        public Path(Transform pathIn)
        {
            path = pathIn;
            used = false;
        }
    }

    [SerializeField]
    private Transform[] _transforms;
    private Path[] _paths;

    [SerializeField]
    EnemySpawner[] _spawners;

    public void Init()
    {
        _paths = new Path[_transforms.Length];

        for (int i = 0; i < _transforms.Length; i++)
        {
            _paths[i] = new Path(_transforms[i]);
        }
    }

    public void SpawnEnemies(ref List<Enemy> possible)
    {
        foreach(EnemySpawner spawner in _spawners)
        {
            spawner.SpawnEnemies(ref possible);
        }
    }

    public Path[] GetPaths()
    {
        return _paths;
    }
}

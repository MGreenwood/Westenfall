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

    public void Init()
    {
        _paths = new Path[_transforms.Length];

        for (int i = 0; i < _transforms.Length; i++)
        {
            _paths[i] = new Path(_transforms[i]);
        }
    }

    public Path[] GetPaths()
    {
        return _paths;
    }
}

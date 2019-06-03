using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    bool _debug;

    public bool Debug
    { get { return _debug; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

}

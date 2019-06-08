using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleSystemWhenComplete : MonoBehaviour
{
    [SerializeField]
    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, ps.main.duration);
    }
}

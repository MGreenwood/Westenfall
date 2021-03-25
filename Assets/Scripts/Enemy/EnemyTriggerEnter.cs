using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTriggerEnter : MonoBehaviour
{
    EnemyBehaviorManager.OnEnterAggroTrigger _method;

    public void SetDelegate(EnemyBehaviorManager.OnEnterAggroTrigger method)
    {
        _method = method;
    }

    private void OnTriggerEnter(Collider other)
    {
        _method?.Invoke(other);
    }
}

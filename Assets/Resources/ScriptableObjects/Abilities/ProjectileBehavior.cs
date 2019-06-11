using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    int damage;
    float maxRange;
    Effect.EffectType effect;
    Vector3 startPosition;
    GameObject _owner;
    
    public void SetVars(int damage_, float range, Effect.EffectType effect_, GameObject owner)
    {
        damage = damage_;
        maxRange = range;
        effect = effect_;
        _owner = owner;

        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if(Vector3.Distance(transform.position, startPosition) > maxRange)
        {
            Destroy(gameObject);
        }

        transform.position = new Vector3(transform.position.x, startPosition.y, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // determine if it was a crit , Use enemy resistance as well as player stats or vice versa TODO
        bool crit = false;

        if (gameObject.layer == LayerMask.NameToLayer("PlayerProjectile"))
        {
            if (other.tag == "Enemy")
            {
                other.gameObject.GetComponent<Enemy>().Damage(damage, effect, crit, _owner);
            }
        }
        else if (gameObject.layer == LayerMask.NameToLayer("EnemyProjectile"))
        {
            if (other.tag == "Player")
            {
                other.gameObject.GetComponent<Player>().Damage(damage, effect, crit, _owner);
            }
        }

        // destroy on collide with anything
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    float damage, maxRange;
    Effect.EffectType effect;
    Vector3 startPosition;

    public void SetVars(float damage_, float range, Effect.EffectType effect_)
    {
        damage = damage_;
        maxRange = range;
        effect = effect_;

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
        if(other.tag == "Enemy")
        {
            // determine if it was a crit , Use enemy resistance as well as player stats
            bool crit = false;

            other.gameObject.GetComponent<Enemy>().Damage(damage, effect, crit);
        }

        // destroy on collide with anything
        Destroy(gameObject);
    }
}

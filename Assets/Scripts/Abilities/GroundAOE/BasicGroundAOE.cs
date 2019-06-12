using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicGroundAOE : MonoBehaviour
{
    [SerializeField]
    Transform plane;

    List<Collider> _objectsInside;
    int playerMask = 12;
    int enemyMask = 13;
    int targetMask;

    GroundAOE _ability;

    private void Start()
    {
        _objectsInside = new List<Collider>();
        StartCoroutine(DealDamage());
        Destroy(gameObject, _ability.GetDuration());

        // detect any objects inside when spawned and add them if applicable
        GetComponent<Rigidbody>().AddForce(Vector3.zero);// = transform.position + Vector3.zero;
    }

    public void init(Sprite sprite, GroundAOE ability)
    {
        _ability = ability;

        // determine who this attack will effect
        if (ability.GetOwner().layer == playerMask)
            targetMask = enemyMask;
        else
            targetMask = playerMask;


        plane.GetComponent<MeshRenderer>().material.mainTexture = sprite.texture;
        plane.localScale = new Vector3(ability.GetRadius() * 0.1f, 1f, ability.GetRadius() * 0.1f);
        // resize collider as well
        GetComponent<SphereCollider>().radius = ability.GetRadius() / 2f;

    }

    private void OnTriggerEnter(Collider other)
    {
        if(!_objectsInside.Contains(other) && other.gameObject.layer == targetMask)
        {
            _objectsInside.Add(other);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        _objectsInside.Remove(other);
    }

    IEnumerator DealDamage()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.zero);
        List<Collider> toDestroy = new List<Collider>();

        float startTime = Time.fixedTime;

        while (startTime + _ability.GetDuration() > Time.fixedTime)
        {
            foreach (Collider p in _objectsInside)
            {
                if (p == null)
                    toDestroy.Add(p);

                p.GetComponent<IDamageable>().Damage(_ability.GetEffect().value, _ability.GetEffect(), false, _ability.GetOwner());
            }

            foreach(Collider p in toDestroy)
            {
                _objectsInside.Remove(p);
            }
            toDestroy.Clear();

            yield return new WaitForSeconds(1);
        }
    }
}

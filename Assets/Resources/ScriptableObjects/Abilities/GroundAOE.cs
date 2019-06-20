using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/GroundAOE")]
public class GroundAOE : Ability
{
    [SerializeField]
    float _radius = 1;
    [SerializeField]
    Sprite _groundSprite;
    [SerializeField]
    float _duration;
    [SerializeField]
    Effect.AbilityEffect _effect;
    [SerializeField]
    GameObject _prefab;

    public override bool Cast()
    {
        Vector3 spawnLocation;
        RaycastHit hit;

        // raycast down to ground for placement                                             // ignore all layers but ground
        if(Physics.Raycast(owner.transform.position, Vector3.down, out hit, Mathf.Infinity, ~LayerMask.NameToLayer("Ground")))
        {
            spawnLocation = hit.point;// + new Vector3(0,0.1f,0);
        }
        else
        {
            // cant find ground?
            spawnLocation = owner.transform.position;
        }

        // place under current location
        BasicGroundAOE aoe = Instantiate(_prefab, spawnLocation, Quaternion.identity).GetComponent<BasicGroundAOE>();
        aoe.init(_groundSprite, this);

        return true;
    }

    public float GetRadius() => _radius;
    public float GetDuration() => _duration;
    public Effect.AbilityEffect GetEffect() => _effect;
}

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
        // place under current location
        BasicGroundAOE aoe = Instantiate(_prefab, owner.transform.position, Quaternion.identity).GetComponent<BasicGroundAOE>();
        aoe.init(_groundSprite, this);

        return true;
    }

    public float GetRadius() => _radius;
    public float GetDuration() => _duration;
    public Effect.AbilityEffect GetEffect() => _effect;
}

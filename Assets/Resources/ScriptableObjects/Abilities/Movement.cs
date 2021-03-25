using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Movement")]
public class Movement : Ability
{
    [SerializeField]
    GameObject _movementAbility;

    public override bool Cast()
    {
       // create the ability object that will control the player
       GameObject ability = Instantiate(_movementAbility, owner.transform);
        // have the movement gameobject report back
       return true;
    }
}

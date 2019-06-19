using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass : ScriptableObject
{
    public enum ClassType { Warden, Knight, Arcanist, Shade, Ranger, Mystic, Any} // any is used for weapons, armor, and abilities that can be used by any class
    [SerializeField] ClassType Class;

    

    
}

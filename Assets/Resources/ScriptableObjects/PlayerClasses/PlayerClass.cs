using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass : ScriptableObject
{
    public enum ClassType { Warden, Knight, Arcanist, Shade, Ranger, Mystic}
    [SerializeField] ClassType Class;

    

    
}

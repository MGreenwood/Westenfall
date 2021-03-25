using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour
{
    Image _image;
    Ability _ability;
    PlayerAbilities _abilities;

    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
        _abilities = Player.instance.GetComponent<PlayerAbilities>();
        _abilities.D_AbilityChanged += UpdateSkillBar;
    }

    void UpdateSkillBar()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillTree : MonoBehaviour
{

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    Player player;

    [SerializeField]
    TextMeshProUGUI statsText;
    

    // Start is called before the first frame update
    void Start()
    {
        UIManager.instance._skillTreeCanvas = canvas;
    }

    public string Serialize()  // TODO will be used for storing player info
    {
        return "";
    }

    public void Deserialize(string data)
    {
        // unpack from a saved instance
    }
}

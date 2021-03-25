using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorDriver : MonoBehaviour
{

    public DungeonGenerator gen;

    public GameObject groundItem;
    public Item itemToSpawn, itemToSpawn2;

    bool _hasSpawned = false;
   
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            if (_hasSpawned)
            {
                // delete last map
                foreach(Transform t in gen.transform)
                {
                    Destroy(t.gameObject);
                }
            }
            else
                gen.SpawnPlayer();

            gen.Generate(7, 8, 25, _hasSpawned);

            _hasSpawned = true;
        }
    }
}

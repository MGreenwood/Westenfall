using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorDriver : MonoBehaviour
{

    public DungeonGenerator gen;

    public GameObject groundItem;
    public Item itemToSpawn, itemToSpawn2;

    bool _hasSpawned = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

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

        // this will put items on the ground
        /*
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 spawnPos = MouseManager.instance.GetMousePosition();// + new Vector3(0,1,0);
            if(spawnPos != null)
            {
                Item newItem = itemToSpawn;
                GroundItem item = Instantiate(groundItem, spawnPos, Quaternion.identity).GetComponent<GroundItem>();
                item.SetItem(newItem);
            }
        }*/
        /*
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 spawnPos = MouseManager.instance.GetMousePosition();
            if (spawnPos != null)
            {
                Item newItem = itemToSpawn2;
                GroundItem item = Instantiate(groundItem, spawnPos, Quaternion.identity).GetComponent<GroundItem>();
                item.SetItem(newItem);
            }
        }*/
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTable : MonoBehaviour
{
    [System.Serializable]
    public class Drop
    {
        public ScriptableObject _scriptableObject;
        public int _dropWeight; // weight defines the chance for an item to drop 1/dropWeight chance. 
                                // high number ==> lower chance
                                // 0 ==> always drops

        public Drop(ScriptableObject scriptableObject, int dropWeight)
        {
            _scriptableObject = scriptableObject;
            _dropWeight = dropWeight;
        }
    }

    [SerializeField]
    List<Drop> _possibleDrops; // all potential drops for this enemy
    [SerializeField] /// summary The number of drops possible after guarenteed drops.
    int _maxDrops;
    [SerializeField]
    int _nullWeight;

    const float _distanceToDropMax = 3f;

    [SerializeField]
    GameObject GroundItemPrefab;

    int poolTotal = 0;

    public void DropItems()
    {
        // drop all "always drop" items and remove them from the list
        for(int i = _possibleDrops.Count - 1; i >= 0; --i)
        {
            int dropWeight = _possibleDrops[i]._dropWeight;
            if(dropWeight == 0)
            {
                Item item = Instantiate(_possibleDrops[i]._scriptableObject as Item);
                SpawnItem(item);
                _possibleDrops.RemoveAt(i);
            }
        }

        // determine how many drops they get
        int numDrops = Random.Range(1, _maxDrops + 1);
        int numDropped = 0;

        while(numDropped < numDrops)
        {
            int[] chances = CalculatePool();

            int rand = Random.Range(0, poolTotal);
            for(int i = 0; i < _possibleDrops.Count; ++i)
            {
                rand -= chances[i];

                if(rand <= 0)
                {
                    // drop the item
                    Item item = Instantiate(_possibleDrops[i]._scriptableObject as Item);
                    SpawnItem(item);
                    numDropped++;

                    _possibleDrops.RemoveAt(i);
                    break;
                }
            }

        }
    }

    int[] CalculatePool()
    {
        poolTotal = 0;

        foreach(Drop d in _possibleDrops)
        {
            poolTotal += d._dropWeight;
        }

        int[] chances = new int[_possibleDrops.Count];
        for(int i = 0; i < _possibleDrops.Count; ++i)
        {
            chances[i] = poolTotal - _possibleDrops[i]._dropWeight;
        }

        poolTotal *= 2;
        return chances;
    }

    void SpawnItem(Item item)
    {
        // pick a nearby place to spawn, 
        // TODO make sure is ground
        Vector3 spawnPosition = Vector3.zero;
        bool picked = false;
        while(!picked)
        {
            Vector3 dir = new Vector3(Random.Range(-_distanceToDropMax, _distanceToDropMax), 0f, Random.Range(-_distanceToDropMax, _distanceToDropMax));
            spawnPosition = transform.position - new Vector3(0, GetComponent<Collider>().bounds.extents.y / 2, 0) + dir;

            picked = true;
        }
            
        Transform groundItem = Instantiate(GroundItemPrefab, spawnPosition, Quaternion.identity).transform;
        groundItem.GetComponent<GroundItem>().SetItem(item);
        GroundItemCanvas.instance.RegisterItem(item, groundItem.transform);
    }
}

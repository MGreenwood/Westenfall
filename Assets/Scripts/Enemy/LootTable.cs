using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTable : MonoBehaviour
{
    [System.Serializable]
    public class Drop
    {
        public ScriptableObject _scriptableObject;
        public int _dropWeight;

        public Drop(ScriptableObject scriptableObject, int dropWeight)
        {
            _scriptableObject = scriptableObject;
            _dropWeight = dropWeight;
        }
    }

    [SerializeField]
    List<Drop> _drops; // all potential drops for this enemy
    [SerializeField]
    int _minDrops, _maxDrops;
    [SerializeField]
    int _nullWeight;

    [SerializeField]
    GameObject GroundItemPrefab;

    private void Start()
    {

    }

    public void DropItems()
    {
        // choose items to drop
        Item item = Instantiate(_drops[0]._scriptableObject as Item);



        Vector3 spawnPosition = transform.position - new Vector3(0, GetComponent<Collider>().bounds.extents.y / 2, 0);

        GroundItem groundItem = Instantiate(GroundItemPrefab, spawnPosition, Quaternion.identity).GetComponent<GroundItem>();
        
        groundItem.SetItem(item);
    }
}

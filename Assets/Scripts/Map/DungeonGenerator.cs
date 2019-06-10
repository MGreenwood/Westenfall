using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private Room[] _rooms;
    [SerializeField] GameObject _player;
    [SerializeField] GameObject[] _possibleRooms;
    [SerializeField] GameObject _startingRoom;

    Transform start;

    List<GameObject> _availableRooms;

    [SerializeField]
    List<Enemy> _possibleEnemies;
    EnemySpawner _spawner;

    private void Start()
    {
        _rooms = new Room[1];
        _availableRooms = new List<GameObject>();

        ReloadRoomList();
    }

    public void ReloadRoomList()
    {
        _availableRooms.Clear();
        foreach (GameObject room in _possibleRooms)
            _availableRooms.Add(room);
    }
    
    // Difficulty is the higher player's level entering the dungeon * difficulty modifier (normal, heroic, mythic)
    public void Generate(int minRooms, int maxRooms, int difficulty, bool hasSpawned)
    {
        int numRooms = Random.Range(minRooms, maxRooms + 1);
        Transform currentExit;

        // this portion is for spawning multiple times in a row, will not be necessary
        if (!hasSpawned)
        {
            Room startingRoom = Instantiate(_startingRoom, transform.position, Quaternion.identity).GetComponent<Room>();
            startingRoom.Init();

            currentExit = startingRoom.GetPaths()[0].path;
            start = currentExit;
        }
        else
        {
            currentExit = start;
        }

        for(int i=0;i< numRooms; i++)
        {
            bool found = false;

            while(!found)
            {
                found = true;

                int index = Random.Range(0, _availableRooms.Count);

                // find a suitable room TODO
                GameObject roomOb = Instantiate(_availableRooms[index], currentExit.position, Quaternion.identity);
                
                Room room = roomOb.GetComponent<Room>();
                room.Init();
                Room.Path[] paths = room.GetPaths();

                roomOb.transform.position -= paths[0].path.localPosition;
                roomOb.transform.SetParent(transform);


                float angle =  Vector3.SignedAngle(paths[0].path.transform.forward, -currentExit.forward, Vector3.up);
                roomOb.transform.RotateAround(paths[0].path.position, Vector3.up, angle);


                // if after rotation, the exit has a lower or same z value than entrance, pick a new room
                if (paths[1].path.position.z < currentExit.position.z || paths[1].path.forward == Vector3.back)
                {
                    found = false;
                    Destroy(roomOb);
                }
                else
                {
                    _availableRooms.RemoveAt(index);
                    roomOb.isStatic = true;
                    currentExit = roomOb.GetComponent<Room>().GetPaths()[1].path;
                    room.SpawnEnemies(ref _possibleEnemies); // Spawn the enemies
                }
            }
        }

        ReloadRoomList(); // reload list for more spawns
    }

    public void SpawnPlayer()
    {
        Instantiate(_player, new Vector3(0, 1, 0), Quaternion.identity);
    }

    public Room[] GetRooms()
    {
        return _rooms;
    }
}

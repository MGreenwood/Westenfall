using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum State { Login, MainMenus, Town, Dungeon }
    public State currentState = State.Login;

    string loggedInUser;

    [SerializeField]
    int dungeonScene;

    [SerializeField]
    bool _debug;

    public bool Debug
    { get { return _debug; } }

    Transform _player;

    [SerializeField]
    Canvas _loadingScreen;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void LoggedIn(string username)
    {
        loggedInUser = username;

        currentState = State.Town;
        SceneManager.LoadScene("Town");
    }

    public void RegisterPlayer(Transform playerIn)
    {
        _player = playerIn;
    }

    public void LoadDungeon(int minRooms, int maxRooms, int difficulty)
    {
        StartCoroutine(LoadingDungeon(minRooms, maxRooms, difficulty));
    }
    IEnumerator LoadingDungeon(int minRooms, int maxRooms, int difficulty)
    {
        _loadingScreen.enabled = true;
        currentState = State.Dungeon;
        yield return SceneManager.LoadSceneAsync("Dungeon");
        
        yield return new WaitForFixedUpdate();

        GameObject gen = GameObject.FindGameObjectWithTag("Generator");

        if (_player == null)
            gen.GetComponent<DungeonGenerator>().SpawnPlayer();
        else
            gen.GetComponent<DungeonGenerator>().MovePlayer(_player);
        _player.GetComponent<PlayerController>().MovementPaused(true);

        gen.GetComponent<DungeonGenerator>().Generate(minRooms, maxRooms, difficulty, false);


        yield return new WaitForSeconds(2f);
        _player.GetComponent<PlayerController>().MovementPaused(false);
        _loadingScreen.enabled = false;
    }
}

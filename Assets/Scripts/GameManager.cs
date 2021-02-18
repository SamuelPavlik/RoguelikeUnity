using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay;
    public float turnDelay = .1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private int _level = 0;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;

    public void AddEnemyToList(Enemy enScript)
    {
        enemies.Add(enScript);
    }

    public void GameOver()
    {
        levelText.text = "After " + _level + " days, you starved.";
        levelImage.SetActive(true);
        enabled = false;
    }

    // Use this for initialization
    private void Awake ()
	{
	    if (instance == null)
	        instance = this;
        else if (instance != this)
            Destroy(gameObject);
            
        DontDestroyOnLoad(gameObject);
	    boardScript = GetComponent<BoardManager>();
        enemies = new List<Enemy>();
	    //InitGame();
	}

    private void InitGame()
    {
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + _level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(_level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    private IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count <= 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        foreach (var enemy in enemies)
        {
            enemy.MoveEnemy();
            yield return new WaitForSeconds(enemy.moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }

    // Update is called once per frame
	private void Update () {
		if (playersTurn || enemiesMoving || doingSetup)
            return;

	    StartCoroutine(MoveEnemies());
	}

    //This is called each time a scene is loaded.
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    { 
        //Add one to our level number.
        _level++;
        //Call InitGame to initialize our level.
        InitGame();
    }

    void OnEnable()
    { 
        //Tell our ‘OnLevelFinishedLoading’ function to start listening for a scene change event as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our ‘OnLevelFinishedLoading’ function to stop listening for a scene change event as soon as this script is disabled.
        //Remember to always have an unsubscription for every delegate you subscribe to!
          SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

}

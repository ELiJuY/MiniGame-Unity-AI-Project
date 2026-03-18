using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor.VersionControl;
//using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    #region Constants
    private const int POINT_PER_PART = 5;
    private const string YOU_WIN = "You Win!!!";
    private const string YOU_LOSE = "You Lose!!!";
    #endregion

    #region Game State
    private enum GameSatus { Alive, Win, Lose };
    private GameSatus status = GameSatus.Alive;
    #endregion

    #region Score & Collectibles
    private int score = 0;
    private int parts = 0; //number of parts to collecte
    private string textScoreBoard = "";
    #endregion

    #region Inspector - UI
    [SerializeField]
    private Text scoreBoard = null;

    [SerializeField]
    private Slider healthBar = null;

    [SerializeField]
    private Text message = null;
    #endregion

    #region Inspector - Enemy Spawning
    [SerializeField]
    private float spawnInterval = 30f;

    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private LevelPoints levelPoints;
    #endregion

    private Transform playerPos = null; //Needed to get the player position


    // Start is called before the first frame update
    void Start()
    {
        //Si se ha establecido el campo de texto se obtiene el texto
        textScoreBoard = scoreBoard?.text;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerController controller = player.GetComponent<PlayerController>();
        playerPos = player.transform;
        controller.OnPartCollected += PartCollected;
        parts = GameObject.FindGameObjectsWithTag("CollectMe").Length;

        EnemyController[] enemies = FindObjectsOfType<EnemyController>();

        foreach (EnemyController enemy in enemies)
        {
            enemy.Initialize(levelPoints.WaitPoints, false);
        }

        //Spawn de enemigos
        InvokeRepeating(nameof(SpawnEnemy), spawnInterval, spawnInterval);
    }

    // LateUpdate is called after frame is updated
    private void LateUpdate()
    {
        if (status == GameSatus.Alive && playerPos.position.y < 0)
        {
            status = GameSatus.Lose;
            EndGame();
        }
    }

    #region Score & Health Logic
    public void PartCollected()
    {
        score += GameController.POINT_PER_PART;
        //Debug.Log("Current Score: " + score);
        if (scoreBoard != null)
        {
            scoreBoard.text = textScoreBoard + score;
        }
        parts--;
        if (parts == 0 && status == GameSatus.Alive)
        {
            status = GameSatus.Win;
            EndGame();
        }
    }

    public void UpdateHealthMeter(int healt)
    {
        healthBar.value = healt;
        if (status == GameSatus.Alive && healthBar.value <= 0.0f)
        {
            status = GameSatus.Lose;
            EndGame();
        }
    }
    #endregion

    #region Enemy Spawning
    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, GetRandomSpawnPoint(), Quaternion.identity);

        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.Initialize(levelPoints.WaitPoints, true);
    }

    private Vector3 GetRandomSpawnPoint()
    {
        Transform[] sp = levelPoints.SpawnPoints;
        int index = Random.Range(0, sp.Length);
        return sp[index].position;
    }
    #endregion

    #region End Game
    private void EndGame()
    {
        switch (status)
        { //SW
            case GameSatus.Lose:
                message.gameObject.SetActive(true);
                message.text = YOU_LOSE;
                message.color = Color.red;
                Time.timeScale = 0;
                break;
            case GameSatus.Win:
                message.gameObject.SetActive(true);
                message.text = YOU_WIN;
                message.color = Color.green;
                Time.timeScale = 0;
                break;
        }//SW
    }
    #endregion

}

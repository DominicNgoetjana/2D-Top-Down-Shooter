using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOne : MonoBehaviour {
    [SerializeField]
    private float speed;
    private float rotSpeed = 180f;
    public GameObject bullet;
    public GameObject bulletSpawnPoint;
    private float maxShootWait = 0.5f, shootWait =0.5f;
    public float health = 100;
    public int numLives = 5;
    public int enemiesKilled = 0;
    public float coinsCollected = 0;
    public GameObject playerPrefab;
    public GameObject playerTwo;
    public GameObject gameOverPanel;
    public Text p1GameOverText;

    private void Start()
    {
        //Debug.Log("starting player one");
        playerTwo = GameObject.FindWithTag("PlayerTwo");
    }

    // Update is called once per frame
    void Update() {
        // rotate player
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            Quaternion rot = transform.rotation;
            float z = rot.eulerAngles.z;
            z -= Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
            rot = Quaternion.Euler(0, 0, z);
            transform.rotation = rot;

            // move player
            Vector3 pos = transform.position;
            Vector3 velocity = new Vector3(0, Input.GetAxis("Vertical") * speed * Time.deltaTime, 0);
            pos += rot * velocity;
            transform.position = pos;
        }

        // shoot
        if (shootWait < maxShootWait) shootWait += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && shootWait >= maxShootWait)
        {
            Shoot();         
        }

        // player died
        if (health <= 0)
        {
            if (numLives > 0)
            {
                SpawnPlayer("me");
            } else
            {
                GameOver("me");
            }
        }
    }

    public void GameOver(string from)
    {
        p1GameOverText.text = "PLAYER 1\n" +
                                "--------------------------------\n" +
                                "ENEMIES KILLED: " + enemiesKilled +
                                "\nCOINS COLLECTED: " + coinsCollected;
        if (from == "me")
        {
            playerTwo.GetComponent<PlayerTwo>().GameOver("playerOne");
            Time.timeScale = 0;
        }
        //Debug.Log("starting");
        gameOverPanel.SetActive(true);
        //Debug.Log("ending");
    }

    public void EndOverallGame()
    {
        p1GameOverText.text =   "GAME COMPLETE -- WELL DONE FIGHTERS\n" +
                                "-----------------------------------\n" +
                                "PLAYER 1\n" +
                                "--------------------------------\n" +
                                "ENEMIES KILLED: " + enemiesKilled +
                                "\nCOINS COLLECTED: " + coinsCollected;
        playerTwo.GetComponent<PlayerTwo>().EndOverallGame();
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);
    }

    public void SpawnPlayer(string from)
    {
        health = 100;
        transform.position = new Vector3(-1.17f, -2.91f, -0.06137657f);
        transform.rotation = Quaternion.identity;
        if (from == "me")
        {
            numLives--;
            playerTwo.GetComponent<PlayerTwo>().SpawnPlayer("playerOne");
        }
    }

    public void Shoot()
    {
        shootWait = 0;
        Vector3 offset = transform.rotation * new Vector3(0, 0.25f, 0);
        bullet.GetComponent<Bullet>().shotBy = "playerOne";
        Instantiate(bullet.transform, bulletSpawnPoint.transform.position + offset, transform.rotation);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // end overall game if both players have reached the end and have killed the last boss
        if (collision.gameObject.tag == "EndScene1" && GameObject.FindWithTag("Block3") != null && playerTwo.GetComponent<PlayerTwo>().reachedEndOfScene)
        {
            EndOverallGame();
        }
    }

    // collect coin
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Coin")
        {
            coinsCollected++;
            Destroy(collision.gameObject);
        }
    }

    // player one stage stats
    private void OnGUI()
    {
        if (numLives >= 0)
        {
            GUI.Label(new Rect(10, 10, 120, 150), "Player One\n" +
               "-----------------\n" +
                "Lives left: " + (numLives+1) +
                "\nHealth left: " + health +
                "\nEnemies killed: " + enemiesKilled +
                "\nCoins collected: " + coinsCollected);
        }
    }
}

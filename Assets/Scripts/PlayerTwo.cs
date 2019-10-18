using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTwo : MonoBehaviour {
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
    public GameObject playerOne;
    public bool reachedEndOfScene = false;
    public GameObject pauseMenuPanel, gameOverPanel;
    public Text p2GameOverText;

    private void Start()
    {
        //Debug.Log("starting player two");
        playerOne = GameObject.FindWithTag("PlayerOne");
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            // rotate player
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
        if (Input.GetKeyDown(KeyCode.RightAlt) && shootWait >= maxShootWait)
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

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale == 1)
            {
                // pause game
                pauseMenuPanel.SetActive(true);
                Time.timeScale = 0; 
            } else
            {
                ResumeGame();
            }
        }
    }

    public void GameOver(string from)
    {
        p2GameOverText.text = "PLAYER 2\n" +
                                "--------------------------------\n" +
                                "ENEMIES KILLED: " + enemiesKilled +
                                "\nCOINS COLLECTED: " + coinsCollected;
        if (from == "me")
        {
            playerOne.GetComponent<PlayerOne>().GameOver("playerTwo");
            Time.timeScale = 0;
            gameOverPanel.SetActive(true);
        }
    }

    public void EndOverallGame()
    {
        p2GameOverText.text = "GAME COMPLETE -- WELL DONE FIGHTERS\n" +
                                "-----------------------------------\n" +
                                "PLAYER 1\n" +
                                "--------------------------------\n" +
                                "ENEMIES KILLED: " + enemiesKilled +
                                "\nCOINS COLLECTED: " + coinsCollected;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void SpawnPlayer(string from)
    {
        health = 100;
        transform.position = new Vector3(1.19f, -2.89f, -0.06137657f);
        transform.rotation = Quaternion.identity;
        if (from == "me")
        {
            numLives--;
            playerOne.GetComponent<PlayerOne>().SpawnPlayer("playerTwo");
        }
    }

    public void Shoot()
    {
        shootWait = 0;
        Vector3 offset = transform.rotation * new Vector3(0, 0.25f, 0);
        bullet.GetComponent<Bullet>().shotBy = "playerTwo";
        Instantiate(bullet.transform, bulletSpawnPoint.transform.position + offset, transform.rotation);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EndScene1") reachedEndOfScene = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EndScene1") reachedEndOfScene = false;
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

    // player two stage stats
    private void OnGUI()
    {
        if (numLives >= 0)
        {
            GUI.Label(new Rect(Screen.width - 120, 10, 110, 150), "Player Two\n" +
                "-----------------\n" +
                "Lives left: " + (numLives+1) +
                "\nHealth left: " + health +
                "\nEnemies killed: " + enemiesKilled +
                "\nCoins collected: " + coinsCollected);
        }
    }
}

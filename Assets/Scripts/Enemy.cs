using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public Transform playerOne;
    public Transform playerTwo;
    [SerializeField] private float rotSpeed = 90f;
    private float fireDelay = 1f, cooldownTimer = 0;
    public GameObject bullet;
    public GameObject bulletSpawnPoint;
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform sightStart, sightEnd;
    public float health = 100;
    public string lastShotBy;

    private void Start()
    {
        if (this.gameObject.tag == "BossEnemy1" || this.gameObject.tag == "BossEnemy2" || this.gameObject.tag == "BossEnemy3") fireDelay = 2f;
    }

    // Update is called once per frame
    void Update () {
        if  (playerOne == null) // if player one is not linked
        {
            try
            {
                GameObject gameObj = GameObject.FindWithTag("PlayerOne");
                playerOne = gameObj.transform;
            } catch (System.Exception e) { Debug.LogException(e); }
        }
        if (playerOne == null) return; // do nothing, no player found

        if (playerTwo == null) // if player two is not linked
        {
            try
            {
                GameObject gameObj = GameObject.FindWithTag("PlayerTwo");
                playerTwo = gameObj.transform;
            }
            catch (System.Exception e) { Debug.LogException(e); }
        }
        if (playerTwo == null) return; // do nothing, no player found

        // player(s) found, so point at them
        if (Vector2.Distance(transform.position, playerOne.position) < Vector2.Distance(transform.position, playerTwo.position)) // if player one is closer to enemy
        {
            Vector3 dir = playerOne.position - transform.position;
            dir.Normalize();
            float zAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            Quaternion desiredRot = Quaternion.Euler(0, 0, zAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, rotSpeed * Time.deltaTime);

            // move towards / away from player
            //Debug.DrawLine(sightStart.position, playerOne.GetComponent<PlayerOne>().transform.position, Color.red);
            if (Vector3.Distance(transform.position, playerOne.transform.position) < 5)
            {
                Vector3 pos = transform.position;
                Vector3 velocity = new Vector3(0, -speed * Time.deltaTime, 0);
                pos += transform.rotation * velocity;
                transform.position = pos;
            }
            else if (Vector3.Distance(transform.position, playerOne.transform.position) < 7)
            {
                Vector3 pos = transform.position;
                Vector3 velocity = new Vector3(0, speed * Time.deltaTime, 0);
                pos += transform.rotation * velocity;
                transform.position = pos;
            }
        } else
        {
            Vector3 dir = playerTwo.position - transform.position;
            dir.Normalize();
            float zAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            Quaternion desiredRot = Quaternion.Euler(0, 0, zAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, rotSpeed * Time.deltaTime);

            // move towards / away from player
            //Debug.DrawLine(sightStart.position, playerTwo.GetComponent<playerTwo>().transform.position, Color.red);
            if (Vector3.Distance(transform.position, playerTwo.transform.position) < 5)
            {
                Vector3 pos = transform.position;
                Vector3 velocity = new Vector3(0, -speed * Time.deltaTime, 0);
                pos += transform.rotation * velocity;
                transform.position = pos;
            }
            else if (Vector3.Distance(transform.position, playerTwo.transform.position) < 7)
            {
                Vector3 pos = transform.position;
                Vector3 velocity = new Vector3(0, speed * Time.deltaTime, 0);
                pos += transform.rotation * velocity;
                transform.position = pos;
            }
        }

        // enemy died
        if (health <= 0)
        {
            if (lastShotBy == "playerOne") playerOne.GetComponent<PlayerOne>().enemiesKilled++;
            else if (lastShotBy == "playerTwo") playerTwo.GetComponent<PlayerTwo>().enemiesKilled++;
            if (this.gameObject.tag == "BossEnemy1")
            {
                GameObject bossenemy = GameObject.FindWithTag("Block1");
                Destroy(bossenemy);
            }
            else if (this.gameObject.tag == "BossEnemy2")
            {
                GameObject bossenemy = GameObject.FindWithTag("Block2");
                Destroy(bossenemy);
            }
            else if (this.gameObject.tag == "BossEnemy3")
            {
                GameObject bossenemy = GameObject.FindWithTag("Block3");
                Destroy(bossenemy);
            }
            Destroy(this.gameObject);
        }

            cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0)
        {
            cooldownTimer = fireDelay;

            if (Vector2.Distance(transform.position, playerOne.position) < Vector2.Distance(transform.position, playerTwo.position))
            {
                // raycast towards player one
                bool hitPlayerOne = Physics2D.Linecast(sightStart.position, playerOne.GetComponent<PlayerOne>().transform.position, 1 << LayerMask.NameToLayer("Player"));

                // raycast towards player and check obstacle inbetween 
                bool hitObstacle = Physics2D.Linecast(sightStart.position, playerOne.GetComponent<PlayerOne>().transform.position, 1 << LayerMask.NameToLayer("Obstacle"));

                if (Vector3.Distance(transform.position, playerOne.transform.position) <= 6 && !hitObstacle && hitPlayerOne)
                    Shoot();
            }
            else
            {
                // raycast towards player two
                bool hitPlayerTwo = Physics2D.Linecast(sightStart.position, playerTwo.GetComponent<PlayerTwo>().transform.position, 1 << LayerMask.NameToLayer("Player"));

                // raycast towards player and check obstacle inbetween 
                bool hitObstacle = Physics2D.Linecast(sightStart.position, playerTwo.GetComponent<PlayerTwo>().transform.position, 1 << LayerMask.NameToLayer("Obstacle"));

                if (Vector3.Distance(transform.position, playerTwo.transform.position) <= 6 && !hitObstacle && hitPlayerTwo)
                    Shoot();
            }
        }
    }

    public void Shoot()
    {
        Vector3 offset = transform.rotation * new Vector3(0, 0.25f, 0);
        if (this.gameObject.tag == "BossEnemy2") bullet.GetComponent<Bullet>().transform.localScale += new Vector3(0.25f, 0.25f, 0.25f);
        else if (this.gameObject.tag == "BossEnemy2") bullet.GetComponent<Bullet>().transform.localScale += new Vector3(0.4977397f, 0.4977397f, 0.4977397f);
        else
            bullet.GetComponent<Bullet>().shotBy = "enemy";
        Instantiate(bullet.transform, bulletSpawnPoint.transform.position + offset, transform.rotation);
    }
}

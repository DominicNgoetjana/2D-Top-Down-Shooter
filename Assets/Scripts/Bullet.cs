using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField] private float speed;
    [SerializeField] private float maxDistance;
    public GameObject playerOne;
    public GameObject playerTwo;
    public GameObject triggerObject;
    public string shotBy;

    // Use this for initialization
    void Start ()
    {
        playerOne = GameObject.FindWithTag("PlayerOne");
        playerTwo = GameObject.FindWithTag("PlayerTwo");
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.position;
        Vector3 velocity = new Vector3(0, speed * Time.deltaTime, 0);
        pos += transform.rotation * velocity;
        transform.position = pos;

        maxDistance += 1 * Time.deltaTime;

        if (maxDistance >= 5)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy" || other.tag == "BossEnemy1" || other.tag == "BossEnemy2" || other.tag == "BossEnemy3")
        {
            if (other.gameObject.GetComponent<Enemy>().health > 0) other.gameObject.GetComponent<Enemy>().health -= 40;
            if (shotBy == "playerOne") other.gameObject.GetComponent<Enemy>().lastShotBy = "playerOne";
            else if (shotBy == "playerTwo") other.gameObject.GetComponent<Enemy>().lastShotBy = "playerTwo";
        }
        else if (other.tag == "PlayerOne")
        {
            if (playerOne.GetComponent<PlayerOne>().health > 0) playerOne.GetComponent<PlayerOne>().health -= 20; // friendly fire allowed
        }
        else if (other.tag == "PlayerTwo")
        {
            if (playerTwo.GetComponent<PlayerTwo>().health > 0) playerTwo.GetComponent<PlayerTwo>().health -= 20; // friendly fire allowed
        }
        else if (other.tag == "Glass" || other.tag == "RedBox" && (shotBy == "playerOne" || shotBy == "playerTwo"))
        {
            Destroy(other.gameObject);
        }
        else if (other.tag == "GreenBox" && shotBy == "enemy")
        {
            Destroy(other.gameObject);
        }
        else if (other.tag == "Explode" && (shotBy == "playerOne" || shotBy == "playerTwo"))
        {
            // get array of objects colliding with 'explosion' object's box collider
            BoxCollider2D collider = (BoxCollider2D) other.gameObject.GetComponent<Collider2D>();
            Vector2 size = collider.size;
            //Vector3 centerPoint = new Vector3( collider.offset.x, collider.offset.y, 0f);    
            Vector3 worldPos = transform.TransformPoint ( collider.offset );
            Rect rect = new Rect(0f, 0f, size.x, size.y);
            rect.center = new Vector2(worldPos.x, worldPos.y);
            float posOffset = 1.5f;
            Vector3 topLeft = new Vector3( rect.xMin - posOffset, rect.yMax + posOffset, worldPos.z);
            Vector3 btmRight = new Vector3( rect.xMax + posOffset, rect.yMin - posOffset, worldPos.z);
            Collider2D[] colliders = Physics2D.OverlapAreaAll(topLeft, btmRight);

            Debug.Log("Collider hits: " + colliders.Length);

            foreach (Collider2D hit in colliders)
            {
                if (hit.gameObject.tag == "Glass" || hit.gameObject.tag == "Enemy")
                {
                    Destroy(hit.gameObject);
                }
            }
            Destroy(other.gameObject);
        }
        Destroy(this.gameObject);
    }
}

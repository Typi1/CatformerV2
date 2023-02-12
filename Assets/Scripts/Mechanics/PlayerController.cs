using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour

{
    private float jumpHeight = 5f;
    private float speed = 3.5f;
    private float grapple_speed = 7.0f;
    private float grapple_dist = 2.0f; // set distance after the grapple object to go

    public Vector3 RespawnPoint;

    public GameObject grapple_end;
    private SpriteRenderer sr;
    private Rigidbody2D body;
    private BoxCollider2D coll;
    public Health healthSystem;

    private float storedJumps; // how many jumps we have left, either 1 or 0
    private bool stuck; // true if currently touching a sticky object (with the "Sticky" tag)
    public float grapple_time; // time that a grapple should last.  if positive or 0, then we are currently in a grappling state

    [SerializeField] private LayerMask jumpableGround;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        stuck = false;
        grapple_time = -1;
        storedJumps = 0;

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            RespawnPoint = new Vector3(-10.12f, -2.38f, 0);
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            RespawnPoint = new Vector3(-7.48f, 3.82f, 0);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (grapple_time < 0) body.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, body.velocity.y);
        //grapple_end.GetComponent<Transform>().position = transform.position + new Vector3((sr.flipX ? -1 : 1) * Mathf.PingPong(Time.time * 5, 2), 0, 0);



        if (body.velocity.x < 0 && !stuck)
        {
            sr.flipX = true;
        }
        else if (body.velocity.x > 0 && !stuck)
        {
            sr.flipX = false;
        }

        if(stuck && grapple_time < 0)
        {
            body.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, Input.GetAxisRaw("Vertical") * speed);
        }

        // stored jump mechanic
        if (storedJumps == 0 && IsGrounded()) // reset jump when player touches ground (TO-DO: sticky wall?)
        {
            storedJumps = 1;
            sr.color = Color.yellow;
        }

        if (storedJumps == 1 && Input.GetButtonDown("Jump"))
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + .1f);
            body.velocity = new Vector2(body.velocity.x, jumpHeight);
            storedJumps = 0;
            sr.color = Color.white;
        }
        
 

        
        //if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        //{
        //    sr.color = Color.green;
        //}
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Sticky")
        {
            sr.flipX = other.transform.position.x - transform.position.x > 0;
            //if (other.transform.position.x - transform.position.x > 0) sr.color = Color.blue;
            stuck = true;

            grapple_time = -1; // end grappling
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Harmful"))
        {
            healthSystem.changeLives(-1);
            Respawn();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Sticky")
        {
            stuck = false;
            if (body.velocity.x == speed || body.velocity.y > 0) body.velocity = new Vector2(body.velocity.x, 0);

        }
    }

    private void Respawn() // kinda janky right now, need checkpoint stuff
    {
        transform.position = RespawnPoint;
    }

    IEnumerator AddGrappleForce(Collider2D collision)
    {
        float grapple_direction = 0;
        // negative grapple_time indicates we are not grappling atm. Change that
        if (grapple_time < 0)
        {
            // use distance to the collision object and grapple speed to determine the time needed to be spent in grapple state
            // then assign this to grapple time

            float grapple_pt_distance = (collision.transform.position.x - transform.position.x) ; // distance btwn player and object (signed)
            //print(grapple_dist / 2);
            grapple_direction = grapple_pt_distance > 0 ? 1 : -1;
            grapple_time = (Mathf.Abs(grapple_pt_distance) + grapple_dist) / grapple_speed; // add the distance past the grapple point desired, then divide by speed

            
        }
        // perform grappling maneuvering
        while (grapple_time >= 0)
        {
            transform.position = transform.position;
            body.velocity = new Vector2(grapple_speed * grapple_direction, 0);
            body.gravityScale = 0;
            grapple_time -= Time.deltaTime;
            yield return null;
        }

        // reset grapple state
        if (grapple_time <= 0)
        {
            grapple_time = -1;
            body.gravityScale = 1;
        }
    }

    private bool IsGrounded() 
    {

        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour

{
    private float jumpHeight = 5.5f;
    private float speed = 3.5f;

    public Vector3 RespawnPoint;

    public GameObject grapple_end;
    public GameObject trail;
    private SpriteRenderer sr;
    private Rigidbody2D body;
    private BoxCollider2D coll;
    public Health healthSystem;

    private float storedJumps; // how many jumps we have left, either 1 or 0
    private bool stuck; // true if currently touching a sticky object (with the "Sticky" tag)

    public float grapple_time; // time that a grapple should last.  if positive or 0, then we are currently in a grappling state
    private float grapple_speed = 7.0f; // speed of dash (for velocity based dash)
    private float grapple_dist = 2.0f; // set distance after the grapple object to go (for velocity-based dash)
    public bool grappleIsForce = false; // toggle for velocity-based grapple or force-based grapple
    private float grappleForce = 3f;

    [SerializeField] private LayerMask jumpableGround;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        stuck = false;
        grapple_time = -1;
        storedJumps = 0;
        trail.SetActive(false);

        if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 7)
        {
            RespawnPoint = new Vector3(-10.12f, -2.38f, 0);
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            RespawnPoint = new Vector3(-7.48f, 3.82f, 0);
        }
        else if (SceneManager.GetActiveScene().buildIndex == 8)
        {
            RespawnPoint = new Vector3(-10.75f, 3.56f, 0);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (grapple_time <= 0 && (Mathf.Abs(body.velocity.x) < speed)) trail.SetActive(false);
        float movement_direction = Mathf.Abs(body.velocity.x) / body.velocity.x;
        if (grapple_time <= 0 && (Mathf.Abs(body.velocity.x) < speed || movement_direction != Mathf.Abs(Input.GetAxisRaw("Horizontal")) / Input.GetAxisRaw("Horizontal")))
        {
            body.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, body.velocity.y);
            body.gravityScale = 1;
        }
        //grapple_end.GetComponent<Transform>().position = transform.position + new Vector3((sr.flipX ? -1 : 1) * Mathf.PingPong(Time.time * 5, 2), 0, 0);



        if (body.velocity.x < 0 && !stuck)
        {
            sr.flipX = true;
        }
        else if (body.velocity.x > 0 && !stuck)
        {
            sr.flipX = false;
        }

        if (stuck && grapple_time < 0)
        {
            body.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, Mathf.Max(Input.GetAxisRaw("Vertical") * speed, body.velocity.y));
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            grappleIsForce = !grappleIsForce;
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
            if(stuck)
            {
                transform.position = new Vector2(transform.position.x + (sr.flipX ? -1 : 1) * 0f, transform.position.y + .1f);
            }
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
        if (other.gameObject.tag == "Sticky")
        {
            sr.flipX = other.GetContact(0).point.x - transform.position.x > 0;
            //sr.flipX = other.transform.position.x - transform.position.x > 0;
            //if (other.transform.position.x - transform.position.x > 0) sr.color = Color.blue;
            stuck = true;

            grapple_time = -1; // end grappling
        }
        if (other.gameObject.tag == "Harmful" && gameObject.tag == "Avatar" && this.tag != "Untagged")
        {
            healthSystem.changeLives(-1);
            Respawn();
            StopAllCoroutines();
            body.velocity = Vector2.zero;
            grapple_time = -1;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Sticky")
        {
            stuck = false;
            //if (body.velocity.x == speed || (body.velocity.y > 0 && body.velocity.y < jumpHeight)) body.velocity = new Vector2(body.velocity.x, 0);

        }
    }

    private void Respawn() // kinda janky right now, need checkpoint stuff
    {
        transform.position = RespawnPoint;
    }

    IEnumerator AddGrappleForce(Collider2D collision)
    {
        float grapple_direction = 0;



        
        if (!grappleIsForce)
        {
            if (grapple_time < 0)
            {
                // use distance to the collision object and grapple speed to determine the time needed to be spent in grapple state
                // then assign this to grapple time
                float grapple_pt_distance = (collision.ClosestPoint(grapple_end.transform.position).x - transform.position.x); // distance btwn player and object (signed)

                grapple_direction = grapple_pt_distance > 0 ? 1 : -1;
                trail.transform.localScale = new Vector3(Mathf.Abs(trail.transform.localScale.x) * grapple_direction, trail.transform.localScale.y, trail.transform.localScale.z);
                trail.transform.localPosition = new Vector3(Mathf.Abs(trail.transform.localPosition.x) * grapple_direction * -1, trail.transform.localPosition.y, trail.transform.localPosition.z);
                grapple_time = (Mathf.Abs(grapple_pt_distance) + grapple_dist) / grapple_speed; // add the distance past the grapple point desired, then divide by speed
                trail.SetActive(true);

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
        }
        else
        {
            if (grapple_time < 0)
            {
                // use distance to the collision object and grapple speed to determine the time needed to be spent in grapple state
                // then assign this to grapple time
                float grapple_pt_distance = collision.ClosestPoint(grapple_end.transform.position).x - transform.position.x; // distance btwn player and object (signed)
                                                                                                                            //print(grapple_dist / 2);

                grapple_direction = grapple_pt_distance > 0 ? 1 : -1;


                trail.transform.localScale = new Vector3(Mathf.Abs(trail.transform.localScale.x) * grapple_direction, trail.transform.localScale.y, trail.transform.localScale.z);
                trail.transform.localPosition = new Vector3(Mathf.Abs(trail.transform.localPosition.x) * grapple_direction * -1, trail.transform.localPosition.y, trail.transform.localPosition.z);
                grapple_time = (Mathf.Abs(grapple_pt_distance) + grapple_dist / 4f) / grapple_speed; // get abs of the distance to the grapple point, then divide by speed
                trail.SetActive(true);

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
            if (grapple_time <= 0)
            {
                //grapple_time = 0.5f;
                //print(grapple_time);
                grapple_time = -1;
                body.gravityScale = 1f;
                body.AddForce(new Vector2(grappleForce * grapple_direction, 0), ForceMode2D.Impulse);
                //print(grappleForce * grapple_direction);
            }
            //while (grapple_time >= 0)
            //{
            //    print(grapple_time);
            //    body.AddForce(new Vector2(grappleForce * grapple_direction, 0), ForceMode2D.Impulse);
            //    grapple_time -= Time.deltaTime;
            //    if (Mathf.Abs(body.velocity.x) <= 0.3f) grapple_time = -1;
            //    print(grapple_time);
            //    yield return null;
            //}

        }
        // reset grapple state
        if (grapple_time <= 0 && grapple_time != -1)
        {
            grapple_time = -1;
            body.gravityScale = 1;
            trail.SetActive(false);
        }
    }


    private bool IsGrounded()
    {

        return (Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround) || stuck);

    }

}
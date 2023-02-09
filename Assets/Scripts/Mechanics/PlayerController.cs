using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour

{
    private float jumpHeight = 5f;
    private float speed = 3.5f;
    private float grapple_speed = 7.0f;
    private float grapple_dist = 2.0f; // set distance after the grapple object to go

    public GameObject grapple_end;
    private SpriteRenderer sr;
    private Rigidbody2D body;
    private BoxCollider2D coll;

    private bool isGrounded;
    private bool stuck; // true if currently touching a sticky object (with the "Sticky" tag)
    public float grapple_time; // time that a grapple should last.  if positive or 0, then we are currently in a grappling state
    
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        stuck = false;
        isGrounded = false;
        grapple_time = -1;
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

        if ((isGrounded || stuck) && Input.GetButtonDown("Jump"))
        {
            body.velocity = new Vector2(body.velocity.x, jumpHeight);
            isGrounded = false;
        }

        //if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        //{
        //    sr.color = Color.green;
        //}
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!isGrounded && other.gameObject.CompareTag("Ground")) isGrounded = true;

        if(other.gameObject.tag == "Sticky")
        {
            sr.flipX = other.transform.position.x - transform.position.x > 0;
            //if (other.transform.position.x - transform.position.x > 0) sr.color = Color.blue;
            stuck = true;

            grapple_time = -1; // end grappling
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Sticky")
        {
            stuck = false;
            if(body.velocity.x == speed || body.velocity.y > 0) body.velocity = new Vector2(body.velocity.x, 0);

        }
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

}

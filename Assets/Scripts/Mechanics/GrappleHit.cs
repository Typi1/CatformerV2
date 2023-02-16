using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GrappleHit : MonoBehaviour
{
    private bool active; // whether the grapple is actively searching for an object to latch onto
    private bool move_forwards; // whether the grapple is extending forwards or retracting
    private float prop_to_end; // a value from 0 to 1 that is how close the grapple is to the max entension point
    private bool grappling;  // whether grappling is currently happening
    private bool posFace; // true if the grapple is to a positive x direction, negative if not

    private CircleCollider2D coll;
    private LineRenderer line;

    private SpriteRenderer srParent; // parent's sprite renderer
    private Rigidbody2D rbParent; // parent's rigidbody2d
    private PlayerController pcParent; // parent's playercontroller function (to be used in doing the grappling through calling coroutines)

    private void Start()
    {
        active = false;
        move_forwards = true;
        srParent = transform.parent.GetComponent<SpriteRenderer>();
        rbParent = transform.parent.GetComponent<Rigidbody2D>();
        prop_to_end = 0;
        coll = GetComponent<CircleCollider2D>();
        grappling = false;
        pcParent = transform.parent.GetComponent<PlayerController>();
        posFace = true;

        line = this.GetComponent<LineRenderer>();
        line.startColor = Color.yellow;
        line.endColor = Color.yellow;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.enabled = false;
        line.sortingLayerName = "Default";
        line.sortingOrder = 2;
    }

    // Update is called once per frame
    private void Update()
    {
        line = GetComponent<LineRenderer>();
        if (pcParent.grapple_time >= 0) grappling = true;
        else grappling = false;

        if(!active && grappling == false && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)))
        {
            active = true;
            move_forwards = true;
            posFace = !srParent.flipX;
            //print(posFace);
        }

        if(active || prop_to_end > 0)
        {
            line.enabled = true;
            //transform.position = transform.parent.transform.position + new Vector3((GetComponentInParent<SpriteRenderer>().flipX ? -1 : 1) * Mathf.PingPong(Time.time * 5, 2), 0, 0);
            if (move_forwards && posFace != srParent.flipX)
            {
                prop_to_end += Time.deltaTime * 2;
                if (prop_to_end >= 0.8)
                {
                    prop_to_end = 1;
                    move_forwards = false;
                    transform.position = new Vector3((srParent.flipX ? -1 : 1) * 2 + transform.parent.transform.position.x, transform.parent.transform.position.y, transform.parent.transform.position.z);

                }
                //print(srParent.flipX == true);
                //srParent.color = Color.red;
                transform.position = Vector3.Lerp(transform.position, new Vector3((srParent.flipX ? -1 : 1) * 2 + transform.parent.transform.position.x, transform.parent.transform.position.y, transform.parent.transform.position.z), prop_to_end);

                
            }
            else
            {
                prop_to_end -= Time.deltaTime * 1;
                if (prop_to_end <= 0.35)
                {
                    prop_to_end = 0;
                    move_forwards = true;
                    active = false;
                    transform.position = transform.parent.position;
                    line.enabled = false;
                }
                transform.position = Vector3.Lerp(transform.parent.position, transform.position, prop_to_end);
                
            }
            if (line.enabled)
            {
                line.SetPosition(0, transform.position);
                line.SetPosition(1, transform.parent.transform.position);
            }
            
        }
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active && (collision.gameObject.tag == "Sticky" || collision.gameObject.tag == "Grapple Point"))
        {
            
            active = false;
            move_forwards = false;
            grappling = true;
            //float direction = collision.transform.position.x - transform.parent.position.x > 0 ? 1 : -1;
            //rbParent.AddForce(new Vector2(direction * 1000f, 0), ForceMode2D.Force);
            //StartCoroutine(AddGrappleForce(collision));
            pcParent.StartCoroutine("AddGrappleForce", collision);
            grappling = true;
        }
        //if (collision.gameObject.tag == "Harmful")
        //{
        //    active = false;
        //    move_forwards = false;
        //    grappling = false;
        //}
        if (!move_forwards && collision.gameObject.tag == "Avatar")
        {
            prop_to_end = 0;
            move_forwards = true;
            active = false;
            line.enabled = false;
            transform.position = transform.parent.position;
        }
    }

    //IEnumerator AddGrappleForce(Collider2D collision)
    //{
    //    while(grappling)
    //    {
    //        // add force towards grappled object
    //        float direction = collision.transform.position.x - transform.parent.position.x > 0 ? 1 : -1;
    //        rbParent.AddForce(new Vector2(direction * 10000f, 0), ForceMode2D.Impulse);
    //        yield return null;
    //    }
    //}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSprite : MonoBehaviour
{

    public float speed = 1;
    public Vector2 start;
    public Vector2 end;
    private bool dir;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = start;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, dir ? end : start, Time.deltaTime * speed);
        if (dir && Vector2.Distance(end, transform.position) < 0.05f)
        {
            dir = !dir;
        }
        else if (!dir && Vector2.Distance(start, transform.position) < 0.05f)
        {
            dir = !dir;
        }
    }
}

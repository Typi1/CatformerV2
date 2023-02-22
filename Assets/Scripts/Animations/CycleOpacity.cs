using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CycleOpacity : MonoBehaviour
{
    private bool incr;
    // Start is called before the first frame update
    void Start()
    {
        incr = false;
    }

    // Update is called once per frame
    void Update()
    {
        Tilemap tm = this.GetComponent<Tilemap>();
        
        if (tm.color.a >= 0.4f)
        {
            incr = false;
        }
        else if (tm.color.a <= 0.2f)
        {
            incr = true;
        }

        if (incr)
        {
            tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, tm.color.a + 1.5f/255f);
        }
        else
        {
            tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, tm.color.a - 1.5f / 255f);
        }
        //print(tm.color.a);
    }
}

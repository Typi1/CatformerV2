using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCycleOpacity : MonoBehaviour
{

    public float opacityMin;
    public float opacityMax;
    public float cycleSpeed;
    private bool incr;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {


        if (sr.color.a >= opacityMax)
        {
            incr = false;
        }
        else if (sr.color.a <= opacityMin)
        {
            incr = true;
        }

        if (incr)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a + cycleSpeed / 255f);
        }
        else
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - cycleSpeed / 255f);
        }
    }
}

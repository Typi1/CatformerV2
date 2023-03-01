using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextTrigger : MonoBehaviour
{

    public Text txtObj; 
    public int txtNum; // which text the object will switch the current displayed text to
    private int screenID;

    // Start is called before the first frame update
    void Start()
    {
        screenID = SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Avatar")
        {
            updateText();
        }
        
    }

    private void updateText()
    {
        switch (screenID)
        {
            case 4: // first screen (moving and jumping)
                if (txtNum == 0)
                {
                    txtObj.text = "Use  WASD  or  Arrow  Keys  to  move.";
                }
                else if (txtNum == 1)
                {
                    txtObj.text = "Press  Space  or  Z  to  jump.";
                }
                break;
            case 5: // second screen (stored jump)
                if (txtNum == 0)
                {
                    txtObj.text = "If  you  walk  off  a  ledge  without  jumping,  you  can  save  your  jump  to  use  midair.";
                }
                break;
            case 6: // third screen (grapple tutorial)
                if (txtNum == 0)
                {
                    txtObj.text = "Press  Shift,  J,  or  X  to  grapple  onto  and  past  orange  poles  at  high  speed.";
                }
                break;
            case 9: // fourth screen (wall cling/climb tutorial)
                if (txtNum == 0)
                {
                    txtObj.text = "While  sticking  to  a  green  wall,  press  W/Up  or  S/Down  to  move  vertically  along  the  wall.";
                }
                else if (txtNum == 1)
                {
                    txtObj.text = "Clinging  to  a  green wall  refreshes  your  jump.  Also,  you  can  grapple  to  green  walls.";
                }
                else if (txtNum == 2)
                {
                    txtObj.text = "";
                }
                break;
            default:
                break;
        }
    }

}

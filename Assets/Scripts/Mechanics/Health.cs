using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int lives;
    public Text lifeText;
    // Start is called before the first frame update
    void Start()
    {
        lifeText.text = ": " + lives;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeLives(int delta)
    {
        lives += delta;
        if (lives <= 0)
        {
            lives = 0;
            SceneManager.LoadScene(3);
        }
        lifeText.text = ": " + lives;
    }
}

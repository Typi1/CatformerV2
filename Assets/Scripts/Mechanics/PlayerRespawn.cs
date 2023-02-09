using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    //public int respawnScene;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D floor)
    {
        if (floor.CompareTag("Avatar"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

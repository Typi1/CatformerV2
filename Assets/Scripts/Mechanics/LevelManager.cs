using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public string nextScene;
    public GameObject fadeEffect;
    public PlayerController player;

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
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            GDTFadeEffect gdtFadeEffect = fadeEffect.GetComponent<GDTFadeEffect>();
            gdtFadeEffect.firstToLast = true;
            player.canMove = false;
            fadeEffect.SetActive(true);
            StartCoroutine(LoadNextScene(gdtFadeEffect.timeEffect));
        }
    }


    IEnumerator LoadNextScene(float time) 
    {

        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(nextScene);
    }
}

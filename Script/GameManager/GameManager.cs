using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool _isGameOver;

    // Update is called once per frame
    void Update()
    {
        //Check if R key was pressed
        //Restart current scene
        if (Input.GetKey(KeyCode.R) && _isGameOver == true)
        {
            //Load current Game scene with Player
            //SceneManager.LoadScene(SceneName);
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }
    }

    public void GameOver()
    {
        _isGameOver = true;
    }
    
    public void ExitMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public playerAddController controller;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Restart()
    {

        SceneManager.LoadScene("SinglePlayer");


    }

    public void MainMenu()
    {

        SceneManager.LoadScene("Main menu");
    }
}

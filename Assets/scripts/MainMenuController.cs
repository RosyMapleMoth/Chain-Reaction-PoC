using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void singlePlayer()
    {
        SceneManager.LoadScene("single player Modular");
    }

    public void exit()
    {
        Application.Quit();
    }

    public void multiPlayer()
    {
        SceneManager.LoadScene("add players");
    }

    public void settings()
    {
        SceneManager.LoadScene("Settings");
    }   


}

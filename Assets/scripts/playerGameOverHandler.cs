using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class playerGameOverHandler : MonoBehaviour
{
    public Text GameOverText;
    public PlayerInput player;
    public InputDevice myDevice;

    public GameObject GameOverUi;
    public GameObject GameOverbackroundUi;
    
    public int selected = 0;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void endGame(int place, int playerNum)
    {
        //GameOverUi.SetActive(true);

        string gameOverText;
        switch (place)
        {
            case 1: gameOverText = "1st Place"; break;
            case 2: gameOverText = "2nd Place"; break;
            case 3: gameOverText = "3rd Place"; break;
            case 4: gameOverText = "4th Place"; break;
            default: gameOverText = "UwU something brokeie wokie"; break;
        } 



        GameOverText.text = gameOverText;
        GameOverbackroundUi.SetActive(true);

        //GameOverUi.GetComponent<Animator>().SetTrigger("start");
        GameOverbackroundUi.GetComponent<Animator>().SetTrigger("start");

      
    }


    public void OnPickup()
    {
        // select
    }

    
    public void OnDrop()
    {
        // deselect
    }

    public void OnMoveleft()
    {
        if (selected > 0)
        {
            deselect(selected--);
            select(selected);
        }
    }

    public void OnMoveright()
    {
        if (selected < 2)
        {
            deselect(selected++);
            select(selected);
        }
    }

    public void deselect(int index)
    {
        gameObject.transform.GetChild(index).GetComponent<Animator>().SetTrigger("deselect");
    }

    public void select(int index)
    {
        gameObject.transform.GetChild(index).GetComponent<Animator>().SetTrigger("select");
    }
}

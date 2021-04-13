using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class playerGameOverHandler : MonoBehaviour
{
    public UnityEvent OptionSelected = new UnityEvent();
    public UnityEvent OptionDeselected = new UnityEvent();
    public Text GameOverText;
    public PlayerInput player;
    public InputDevice myDevice;

    public GameObject GameOverUi;
    public GameObject GameOverbackroundUi;
    
    /*
        What option is currently hovered
            0 -> Restart
            1 -> Char elect
            2 -> Main Menu
    */  
    public int hovered = 0;

    // if player has locked in a choice
    private bool lockedIn = true; 



    /// <summary>
    /// Makes Game Over UI show up.
    /// </summary>
    public void endGame(int place)
    {
        setEndGameText(place);
        GameOverbackroundUi.SetActive(true);
        GameOverbackroundUi.GetComponent<Animator>().SetTrigger("start");
    }



    /// <summary>
    /// Sets end game text to fit with the end place passed in.
    /// </summary>
    private void setEndGameText(int place)
    {
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
    }


    
    /// function Catchers for InputManger

    // Select 
    public void OnPickup()
    {
        lockedIn = true;
        uiSelect(hovered);
        OptionSelected.Invoke();
    }

    // Deselect
    public void OnDrop()
    {
        lockedIn = false;
        uiDeselect(hovered);
        OptionDeselected.Invoke();
    }

    public void OnMoveleft()
    {
        if (hovered > 0 && !lockedIn)
        {
            uiEndHighlight(hovered--);
            uiStartHighlight(hovered);
        }
    }

    public void OnMoveright()
    {
        if (hovered < 2 && !lockedIn)
        {
            uiEndHighlight(hovered++);
            uiStartHighlight(hovered);
        }
    }

    private void uiStartHighlight(int index)
    {
        gameObject.transform.GetChild(index).GetComponent<Animator>().SetBool("selected",true);
    }

    private void uiEndHighlight(int index)
    {
        gameObject.transform.GetChild(index).GetComponent<Animator>().SetBool("selected", false);
    }

    private void uiSelect(int index)
    {
        // TODO : add UI Juice 
            // maybe don't do this while UI is in porcess of chaning
    }
    private void uiDeselect(int index)
    {
        // TODO : add UI Juice 
            // maybe don't do this while UI is in porcess of chaning
    }





}

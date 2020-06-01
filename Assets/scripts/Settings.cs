using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Settings : MonoBehaviour
{


    public List<PlayerInput> Players;

    public KeyCode PlayerOneLeft;
    public KeyCode PlayerOneRight;
    public KeyCode PlayerOneUp;
    public KeyCode PlayerOneDown;
    public KeyCode PlayerOnePick;
    public KeyCode PlayerOneDrop;
    public KeyCode PlayerTwoLeft;
    public KeyCode PlayerTwoRight;
    public KeyCode PlayerTwoUp;
    public KeyCode PlayerTwoDown;
    public KeyCode PlayerTwoPick;
    public KeyCode PlayerTwoDrop;

    public static Settings Instance;

    public KeyCode getPlayerOneLeft()
    {
        return this.PlayerOneLeft;
    }

    public void setPlayerOneLeft(KeyCode PlayerOneLeft)
    {
        this.PlayerOneLeft = PlayerOneLeft;
    }

    public KeyCode getPlayerOneRight()
    {
        return this.PlayerOneRight;
    }

    public void setPlayerOneRight(KeyCode PlayerOneRight)
    {
        this.PlayerOneRight = PlayerOneRight;
    }

    public KeyCode getPlayerOneUp()
    {
        return this.PlayerOneUp;
    }

    public void setPlayerOneUp(KeyCode PlayerOneUp)
    {
        this.PlayerOneUp = PlayerOneUp;
    }

    public KeyCode getPlayerOneDown()
    {
        return this.PlayerOneDown;
    }

    public void setPlayerOneDown(KeyCode PlayerOneDown)
    {
        this.PlayerOneDown = PlayerOneDown;
    }

    public KeyCode getPlayerOnePick()
    {
        return this.PlayerOnePick;
    }

    public void setPlayerOnePick(KeyCode PlayerOnePick)
    {
        this.PlayerOnePick = PlayerOnePick;
    }

    public KeyCode getPlayerOneDrop()
    {
        return this.PlayerOneDrop;
    }

    public void setPlayerOneDrop(KeyCode PlayerOneDrop)
    {
        this.PlayerOneDrop = PlayerOneDrop;
    }

    public KeyCode getPlayerTwoLeft()
    {
        return this.PlayerTwoLeft;
    }

    public void setPlayerTwoLeft(KeyCode PlayerTwoLeft)
    {
        this.PlayerTwoLeft = PlayerTwoLeft;
    }

    public KeyCode getPlayerTwoRight()
    {
        return this.PlayerTwoRight;
    }

    public void setPlayerTwoRight(KeyCode PlayerTwoRight)
    {
        this.PlayerTwoRight = PlayerTwoRight;
    }

    public KeyCode getPlayerTwoUp()
    {
        return this.PlayerTwoUp;
    }

    public void setPlayerTwoUp(KeyCode PlayerTwoUp)
    {
        this.PlayerTwoUp = PlayerTwoUp;
    }

    public KeyCode getPlayerTwoDown()
    {
        return this.PlayerTwoDown;
    }

    public void setPlayerTwoDown(KeyCode PlayerTwoDown)
    {
        this.PlayerTwoDown = PlayerTwoDown;
    }

    public KeyCode getPlayerTwoPick()
    {
        return this.PlayerTwoPick;
    }

    public void setPlayerTwoPick(KeyCode PlayerTwoPick)
    {
        this.PlayerTwoPick = PlayerTwoPick;
    }

    public KeyCode getPlayerTwoDrop()
    {
        return this.PlayerTwoDrop;
    }

    public void setPlayerTwoDrop(KeyCode PlayerTwoDrop)
    {
        this.PlayerTwoDrop = PlayerTwoDrop;
    }


    public float Vol;




    void Awake ()   
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Players = new List<PlayerInput>();
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy (gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    


}

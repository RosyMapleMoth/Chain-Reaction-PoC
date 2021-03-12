using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationCatch : MonoBehaviour
{
    public Animator menu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void startMenuDrop()
    {
        menu.gameObject.SetActive(true);
        menu.SetTrigger("start");
    }
}

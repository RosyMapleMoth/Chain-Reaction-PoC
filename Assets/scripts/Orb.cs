﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    public OrbScriptable orbScript;
    public enum OrbState {Grabbed, Resting, Falling, Poping, Evaluating, ToEvaluate, ToPop}
    public OrbState curState = OrbState.Resting;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (curState == OrbState.Poping)
        {
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    public void LoadOrb(OrbScriptable tooSet)
    {
        orbScript = tooSet;
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = tooSet.orbColor;
    }

    public OrbType GetOrbType()
    {
        return orbScript.orbType;
    }

    public bool checkOrbType(OrbType check)
    {
        return check == orbScript.orbType;
    }


    public Vector2 GetRelPos()
    {
        return new Vector2(Mathf.FloorToInt(transform.localPosition.x), Mathf.Abs(Mathf.CeilToInt(transform.position.y) - 6));
    }



}

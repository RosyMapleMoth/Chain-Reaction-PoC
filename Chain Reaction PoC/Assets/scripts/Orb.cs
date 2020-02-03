using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    public OrbScriptable orbScript;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// TODO
    /// </summary>
    public void LoadOrb(OrbScriptable tooSet)
    {
        orbScript = tooSet;
        gameObject.GetComponent<SpriteRenderer>().sprite = tooSet.orbColor;
    }

    public OrbType GetOrbType()
    {
        return orbScript.orbType;
    }

    public bool checkOrbType(OrbType check)
    {
        return check == orbScript.orbType;
    }
}

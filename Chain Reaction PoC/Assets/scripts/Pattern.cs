using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct line
{
    public OrbType[] orbs;
}


[CreateAssetMenu(fileName = "Pattern", menuName = "Pattern/MagicalDrop" + "", order = 1)]
public class Pattern : ScriptableObject
{
    
    public line[] lines;
}


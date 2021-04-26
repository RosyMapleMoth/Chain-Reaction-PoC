using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Line", menuName = "OrbLine" + "", order = 1)]
public class Line : ScriptableObject
{
    public OrbScriptable[] orbs;
}

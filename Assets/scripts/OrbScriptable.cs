using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Orb", menuName = "Orbs/Types", order = 1)]
public class OrbScriptable : ScriptableObject
{
    public Sprite orbColor;
    public OrbType orbType; 
}

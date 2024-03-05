using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Win", menuName = "Winner")]
public class WinnerData : ScriptableObject
{
    public string id;
    public string playerName;
}

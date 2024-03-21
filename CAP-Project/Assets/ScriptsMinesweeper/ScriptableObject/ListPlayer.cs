using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New List", menuName = "List")]
public class ListPlayer : ScriptableObject
{
    public List<BasePlayer> listPlayers;
}

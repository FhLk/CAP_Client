using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action",menuName ="Action")]
public class PlayerAction : ScriptableObject
{
    public bool isHost = false;
    public bool isJoin = false;

    private void Awake()
    {
        isHost = false;
        isJoin = false;
    }
}

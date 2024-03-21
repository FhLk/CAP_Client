using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Data : MonoBehaviour
{
    int[] listPlayer = { 1, 2, 3};
    private int count = 1;

    public GameObject[] LIST;
    public Text countList;
    public GameObject prefabToJoin;
    public GameObject prefabToLeave;
    public GameObject LISTDisplay;

    public void OnMouseDown()
    {
        if (count < 4)
        {
            addPlayer(count++);
        }
    }

    public void addPlayer(int index)
    {
        countList.text = (index+1) + "/4";
        Transform childToDestroy = LISTDisplay.transform.GetChild(index);
        if (childToDestroy != null)
        {
            Vector3 positionToDestroy = childToDestroy.transform.position;

            Destroy(childToDestroy.gameObject);

            GameObject newPlayer = Instantiate(prefabToJoin, LISTDisplay.transform);
            newPlayer.transform.position = positionToDestroy;
            newPlayer.transform.SetSiblingIndex(index);
            LIST[index] = newPlayer;
        }
    }
    public void removePlayer()
    {
        this.count = 1;
        countList.text = "1/4";
        Transform[] childsToDestroy = { LISTDisplay.transform.GetChild(1), LISTDisplay.transform.GetChild(2), LISTDisplay.transform.GetChild(3) };
        for (int i = 0; i < childsToDestroy.Length; i++)
        {
            Transform child = childsToDestroy[i];
            if (child != null)
            {
                Vector3 positionToDestroy = child.transform.position;

                Destroy(child.gameObject);

                GameObject newPlayer = Instantiate(prefabToLeave, LISTDisplay.transform);
                newPlayer.transform.position = positionToDestroy;
                newPlayer.transform.SetSiblingIndex(i+1);
                LIST[i+1] = newPlayer;
            }
        }
    }
}

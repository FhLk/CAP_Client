using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseColor : MonoBehaviour
{
    private ColorTile previousChoose;
    [SerializeField] private Player player;
    public void selectColor(ColorTile s) 
    {
        ColorTile newChoose = s;
        newChoose.select = true;
        newChoose._selected.GetComponent<Image>().enabled =  true;
        player.playerRenderer.color = new Color(newChoose._baseColor.r, newChoose._baseColor.g, newChoose._baseColor.b);
        if (previousChoose != null && previousChoose != newChoose)
        {
            previousChoose.select = false;
            previousChoose._selected.GetComponent<Image>().enabled = false;
        }
        previousChoose = newChoose;
    }
}

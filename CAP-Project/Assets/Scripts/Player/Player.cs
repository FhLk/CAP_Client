using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BasePlayer
{
    private string[] colors = { "RED", "GREEN", "YELLOW", "BLUE" };
    private int currentColorIndex = 0;
    public SpriteRenderer playerRenderer;

    private void Start()
    {
        // Get the SpriteRenderer component attached to the player object
        playerRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeColorLeft()
    {
        currentColorIndex = (currentColorIndex - 1 + colors.Length) % colors.Length;
        ChangePlayerColor();
    }

    public void ChangeColorRight()
    {
        currentColorIndex = (currentColorIndex + 1) % colors.Length;
        ChangePlayerColor();
    }

    private void ChangePlayerColor()
    {
        // You can assign colors to your player based on the currentColorIndex
        string color = colors[currentColorIndex];
        switch (color)
        {
            case "RED":
                playerRenderer.color = Color.red;
                break;
            case "GREEN":
                playerRenderer.color = Color.green;
                break;
            case "YELLOW":
                playerRenderer.color = Color.yellow;
                break;
            case "BLUE":
                playerRenderer.color = Color.blue;
                break;
            default:
                playerRenderer.color = Color.white; // Default color
                break;
        }
    }
}

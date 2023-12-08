using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManger : MonoBehaviour
{
    [SerializeField] private GameObject sound;
    [SerializeField] private Sprite unmute;
    [SerializeField] private Sprite mute;

    public void Main()
    {
        SceneManager.LoadScene("Main");
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Game()
    {
        SceneManager.LoadScene("Board_Cell");
    }

    public void Lobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void LeaderBoard()
    {
        SceneManager.LoadScene("LeaderBoard");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void soundOnOff()
    {
        if(this.sound.GetComponent<Image>().sprite.name == "Unmute")
        {
            this.sound.GetComponent<Image> ().sprite = mute;
        }
        else
        {
            this.sound.GetComponent<Image>().sprite = unmute;
        }
    }
}

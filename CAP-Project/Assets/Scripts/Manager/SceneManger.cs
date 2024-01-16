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
    [SerializeField] private GameObject LIST;

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
        Transform[] childs = { LIST.transform.GetChild(1), LIST.transform.GetChild(2), LIST.transform.GetChild(3) };
        List<GameObject> list = new List<GameObject>();
        foreach (Transform child in childs)
        {
            if (child.childCount != 0)
            {
                list.Add(child.gameObject);
            }
        }
        if (list.Count == 3)
        {
            SceneManager.LoadScene("Board_Cell");
        }
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
        if(this.sound.GetComponent<SpriteRenderer>().sprite.name == "Unmute")
        {
            this.sound.GetComponent<SpriteRenderer> ().sprite = mute;
            this.sound.GetComponent<Image>().sprite = mute;
        }
        else
        {
            this.sound.GetComponent<SpriteRenderer>().sprite = unmute;
            this.sound.GetComponent<Image>().sprite = unmute;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private GameObject popupWindow;
    [SerializeField] private GameObject menuWindow;
    private bool isOpen;
    public void openWindow()
    {
        isOpen = true;
        popupWindow.SetActive(true);
        buttonOnOff(0);
    }

    public void closeWindow() 
    {
        isOpen = false;
        popupWindow.SetActive(false);
        buttonOnOff(1);
    }

    private void OnOffWindow()
    {
        if (isOpen)
        {
            closeWindow();
        }
        else
        {
            openWindow();
        }
    }

    private void buttonOnOff(int action)
    {
        Transform getAllChilds = menuWindow.GetComponentInChildren<Canvas>().transform;
        for (int i = 0; i < getAllChilds.childCount; i++)
        {
            Transform childTransform = getAllChilds.GetChild(i);
            Button childBTN = childTransform.GetComponent<Button>();
            if (childBTN != null)
            {
                if(action == 0)
                {
                    childBTN.enabled = false;
                }
                else
                {
                    childBTN.enabled = true;
                }
            }
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnOffWindow();
            }
        }
        else if (SceneManager.GetActiveScene().name == "Menu")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                closeWindow();
            }
        }

    }
}

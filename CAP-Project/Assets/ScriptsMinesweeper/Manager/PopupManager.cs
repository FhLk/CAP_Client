using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private GameObject popupWindow;
    [SerializeField] private GameObject menuWindow;
    public void openWindow()
    {
        popupWindow.SetActive(true);
        buttonOnOff(0);
    }

    public void closeWindow() 
    {
        popupWindow.SetActive(false);
        buttonOnOff(1);
    }

    private void buttonOnOff(int action)
    {
        Transform getAllChilds = menuWindow.transform;
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
}

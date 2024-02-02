using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetAPI : MonoBehaviour
{
    public string URL;

    public void GetData()
    {
        StartCoroutine(FetchData());
    }
    public IEnumerator FetchData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(URL))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Connected");
                //LobbyData data = new LobbyData();
                //data = JsonUtility.FromJson<LobbyData>(request.downloadHandler.text);
                //Debug.Log(request.downloadHandler.text);
                //lobby = JsonUtility.FromJson<LobbyData>(request.downloadHandler.text);
                //Debug.Log(lobby.randomID);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class NetworkHandler
{

    public static IEnumerator PostNewFootstep(Footstep footstep)
    {
        return PostRequest(
            Secrets.DB_URL + "/footsteps.json",
            JsonUtility.ToJson(footstep)
        );
    }

    private static IEnumerator PostRequest(string URL, string data)
    {
        Debug.Log("Posting " + data);
        UnityWebRequest postReq = new UnityWebRequest(URL, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(data);
        postReq.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        postReq.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        postReq.SetRequestHeader("Content-Type", "application/json");

        yield return postReq.SendWebRequest();

        if (postReq.isNetworkError)
        {
            Debug.Log("Error While Sending: " + postReq.error);
        }
        else
        {
            Debug.Log("Received: " + postReq.downloadHandler.text);
        }
    }

    public static IEnumerator GetRequest(string URL, Action<string> then)
    {
        UnityWebRequest getReq = new UnityWebRequest(URL, "GET");
        getReq.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return getReq.SendWebRequest();

        if (getReq.isNetworkError)
        {
            Debug.Log("Error While Getting: " + getReq.error);
        }
        else
        {
            Debug.Log("Received: " + getReq.downloadHandler.text);
            then.Invoke(getReq.downloadHandler.text);
        }
    }
}

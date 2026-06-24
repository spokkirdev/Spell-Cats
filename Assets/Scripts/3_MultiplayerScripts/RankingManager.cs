using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.Networking;

public class RankingManager : NetworkBehaviour
{
    private const string privateCode = "AbCdEfGhIjKlMn";

    private const string publicCode = "XyZ1234567890";
    private const string webUrl = "http://dreamlo.com/lb/";


    public void UploadScore(string username, int score)
    {
        StartCoroutine(UploadScoreCoroutine(username, score));
    }

    private IEnumerator UploadScoreCoroutine(string username, int score)
    {
        string url = $"{webUrl}{privateCode}/add/{UnityWebRequest.EscapeURL(username)}/{score}";

        
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error uploading score: {www.error}");
            }
            else
            {
                Debug.Log("Score uploaded successfully!");
            }
        }
    }

    public void DownloadScores()
    {
        StartCoroutine(DownloadScoresCoroutine());
    }

    private IEnumerator DownloadScoresCoroutine()
    {
        string url = $"{webUrl}{publicCode}/pipe";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error downloading scores: {www.error}");
            }
            else
            {
                Debug.Log("Scores downloaded successfully!");
            }
        }
    }

}

using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;


public class ScoreEntry
{
    public string playerName;
    public int score;
    public int rank;
}

public class RankingManager : NetworkBehaviour
{

    public List<ScoreEntry> scores = new List<ScoreEntry>();
    private const string privateCode = "a80CvFh820ihu-Mnngj-MgqQ0Q8rBbmUSm3Z9mBzIt4A";

    private const string publicCode = "6a423a628f40bb13184a7d86";

    private const string webUrl = "http://dreamlo.com/lb/";
    private void Awake()
    {
        GameServiceLocator.RegisterService<RankingManager>(this);
        DownloadScores();
    }


    private void OnDestroy()
    {
        GameServiceLocator.UnregisterService<RankingManager>();
    }
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
                Debug.Log($"Score uploaded successfully for {username} with score {score}");
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
            Debug.Log($"[RankingManager] DownloadScoresCoroutine: {www.downloadHandler.text}");

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error downloading scores: {www.error}");
            }
            else
            {
                string data = www.downloadHandler.text;

                Debug.Log($"[RankingManager] Downloaded scores: {data}");

                if (data.StartsWith("ERROR"))
                {
                    yield break;
                }

                scores.Clear();

                string[] lines = data.Split('\n');

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] fields = line.Split('|');

                    if (fields.Length < 2)
                    {
                        Debug.LogWarning("Línea inválida: " + line);
                        continue;
                    }

                    int scoreValue = 0;
                    int rankValue = 0;

                    int.TryParse(fields[1], out scoreValue);

                    if (fields.Length > 2)
                        int.TryParse(fields[2], out rankValue);

                    scores.Add(new ScoreEntry
                    {
                        playerName = fields[0],
                        score = scoreValue,
                        rank = rankValue
                    });
                }
            }
        }
    }

}

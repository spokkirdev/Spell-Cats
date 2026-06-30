using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class OnlineMenuManager : MonoBehaviour
{

    [Header("UI Elements")]
    [SerializeField] private TMPro.TMP_InputField playerNameInputField;

    [SerializeField] private TMPro.TMP_InputField joinCodeInput;

    [SerializeField] private  GameObject joinGameButton;
    [SerializeField] private  GameObject joinGameButton2;
    [SerializeField] private GameObject hostGameButton;
    [SerializeField] private GameObject RoomCodeText;
    [SerializeField] private GameObject RoomCodeField;
    [SerializeField] private TMPro.TMP_Text JoinGame_BackText;

    [Header("Ranking References")]
    [SerializeField] private TMP_Text Rank_1;
    [SerializeField] private TMP_Text Rank_2;
    [SerializeField] private TMP_Text Rank_3;
    


    private OnlineGameData _onlineGameData;
    private RelayManager _relayManager;

    private void Start()
    {
        _onlineGameData = GameServiceLocator.GetService<OnlineGameData>();
        _relayManager = GameServiceLocator.GetService<RelayManager>();
        playerNameInputField.text = "Player" + Random.Range(1000, 9999);
        UpdateRanking();
    }

    private void Update()
    {
        UpdateRankingPeriodically();
    }

    private void UpdateRankingPeriodically()
    {
        // Update the ranking every 5 seconds
        if (Time.frameCount % (5 * 60) == 0) // Assuming 60 FPS, this will be approximately every 5 seconds
        {
            UpdateRanking();
        }
    }

    private void UpdateRanking()
    {
        RankingManager rankingManager = GameServiceLocator.GetService<RankingManager>();
        Debug.Log($"[OnlineMenuManager] UpdateRanking: RankingManager instance: {rankingManager}");
        if (rankingManager != null)
        {
            rankingManager.DownloadScores();
            var scores = rankingManager.scores;
            Debug.Log($"[OnlineMenuManager] UpdateRanking: {scores.Count} scores downloaded.");
            Debug.Log($"[OnlineMenuManager] Scores: {string.Join(", ", scores.ConvertAll(s => $"{s.playerName}:{s.score}"))}");
            scores.Sort((a, b) => b.score.CompareTo(a.score)); // Sort descending

            Rank_1.text = scores.Count > 0 ? $"{scores[0].playerName}: {scores[0].score}" : "No Data";
            Rank_2.text = scores.Count > 1 ? $"{scores[1].playerName}: {scores[1].score}" : "No Data";
            Rank_3.text = scores.Count > 2 ? $"{scores[2].playerName}: {scores[2].score}" : "No Data";
        }
    }

    public async void Out_StartHost()
    {
        _onlineGameData.PlayerName = playerNameInputField.text;
        try
        {
            string joinCode = await _relayManager.CreateRoomAsync(4);        }
        catch
        {
            
        }
    }

    public async void Out_StartClient()
    {
        _onlineGameData.PlayerName = playerNameInputField.text;

        bool joined = await _relayManager.JoinRoomAsync(joinCodeInput.text);
    }

    public void Out_ShutDownOnline()
    {
        NetworkManager.Singleton.Shutdown();
    }
    public void Out_QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void Out_ToggleJoinGameButton()
    {
        if (JoinGame_BackText.text == "Join Game")
        {
            JoinGame_BackText.text = "Back";
        }
        else
        {
            JoinGame_BackText.text = "Join Game";
        }
        joinGameButton2.SetActive(!joinGameButton2.activeSelf);
        hostGameButton.SetActive(!hostGameButton.activeSelf);
        RoomCodeText.SetActive(!RoomCodeText.activeSelf);
        RoomCodeField.SetActive(!RoomCodeField.activeSelf);
        
    }

        
}

using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using UnityEngine.UI;

public class OnlineMenuManager : MonoBehaviour
{

    [Header("UI Elements")]
    [SerializeField] private TMPro.TMP_InputField playerNameInputField;

    [SerializeField] private TMPro.TMP_InputField joinCodeInput;

    [SerializeField] private  GameObject joinGameButton;
    [SerializeField] private  GameObject joinGameButton2;
    [SerializeField] private GameObject hostGameButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private GameObject RoomCodeText;
    [SerializeField] private GameObject RoomCodeField;
    [SerializeField] private TMPro.TMP_Text JoinGame_BackText;

    [Header("Ranking References")]
    [SerializeField] private TMP_Text Rank_1;
    [SerializeField] private TMP_Text Rank_2;
    [SerializeField] private TMP_Text Rank_3;

    private bool _updatingRanking = false;
    


    private OnlineGameData _onlineGameData;
    private RelayManager _relayManager;

    private void Start()
    {
        _updatingRanking = false;
        _onlineGameData = GameServiceLocator.GetService<OnlineGameData>();
        _relayManager = GameServiceLocator.GetService<RelayManager>();
        playerNameInputField.text = "Player" + Random.Range(1000, 9999);
        UpdateRanking();
    }

    private void Update()
    {
        if (!_updatingRanking)
        {
            StartCoroutine(UpdateRankingPeriodically());
        }
    }

    private System.Collections.IEnumerator UpdateRankingPeriodically()
    {
        _updatingRanking = true;
        UpdateRanking();
        yield return new WaitForSeconds(60f);
        _updatingRanking = false;
    }

    private void UpdateRanking()
    {
        RankingManager rankingManager = GameServiceLocator.GetService<RankingManager>();
        if (rankingManager != null)
        {
            rankingManager.DownloadScores();
            var scores = rankingManager.scores;

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
            newGameButton.interactable = false;
            string joinCode = await _relayManager.CreateRoomAsync(4);        }
        catch
        {
            newGameButton.interactable = true;
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

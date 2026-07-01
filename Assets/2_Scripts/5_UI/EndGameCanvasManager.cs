using UnityEngine;

public class EndGameCanvasManager : MonoBehaviour
{

    [SerializeField] private TMPro.TMP_Text pointsText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        pointsText.text = GameServiceLocator.GetService<NetworkPoints>().Points.Value.ToString();
        GameServiceLocator.GetService<CursorControllerSystem>().ShowCursor();
        Time.timeScale = 0f;
    }

    public void Out_GoToOnlineMenu()
    {
        GameServiceLocator.GetService<RelayManager>().CloseRoomCoroutine();
        GameManager.Instance.RequestOnlineMenuScene();
        Time.timeScale = 1f;
    }
}

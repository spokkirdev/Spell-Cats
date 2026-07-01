using System;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameAudioManager gameAudioManager;
    public void Out_RespawnPlayer()
    {
        Debug.Log("Requesting respawn from PauseMenuManager");
        GameServiceLocator.GetService<NetworkHealthComponent>().RequestRespawnServerRpc();
        this.gameObject.SetActive(false);
    }

    public void Out_ClosePauseMenu()
    {
        GameServiceLocator.GetService<CursorControllerSystem>().HideCursor();
        this.gameObject.SetActive(false);
    }

    public void Out_GoToOnlineMenu()
    {
        GameServiceLocator.GetService<RelayManager>().CloseRoomCoroutine();
        GameManager.Instance.RequestOnlineMenuScene();
    }

    public void Out_ToggleMuteMusic()
    {
        gameAudioManager.ToggleMuteMusic();
    }
}

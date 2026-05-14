using UnityEngine;

public class Linker_Canvas_GameScene : MonoBehaviour
{
    public static Linker_Canvas_GameScene Reference => GameCoreApplication.Instance.Linker_GameScene.Linker_Canvas_GameScene;

    [field: SerializeField, Header("HUD")] public HUD_MainGame HUD_MainGame { get; private set; }

}

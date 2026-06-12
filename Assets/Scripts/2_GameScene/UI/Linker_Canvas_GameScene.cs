using UnityEngine;

public class Linker_Canvas_GameScene : MonoBehaviour
{

    [field: SerializeField, Header("HUD")] public HUD_MainGame HUD_MainGame { get; private set; }

    private void Awake()
    {
        GameServiceLocator.RegisterService<Linker_Canvas_GameScene>(this);
    }

    private void OnDestroy()
    {
        GameServiceLocator.UnregisterService<Linker_Canvas_GameScene>();
    }

}

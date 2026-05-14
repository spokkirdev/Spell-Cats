using UnityEngine;

public class Linker_GameScene : MonoBehaviour
{
    public static Linker_GameScene Reference => GameCoreApplication.Instance.Linker_GameScene;
   

    [field: SerializeField, Header("Main Systems")] public GameScene_Starter GameScene_Starter { get; private set; }
    [field: SerializeField, Header("UI")] public Linker_Canvas_GameScene Linker_Canvas_GameScene { get; private set; }

}

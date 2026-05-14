using UnityEngine;

public class Linker_MenuScene : MonoBehaviour
{
    public static Linker_MenuScene Reference => GameCoreApplication.Instance.Linker_MenuScene;
   

    [field: SerializeField, Header("Main Systems")] public MenuScene_Starter MenuScene_Starter { get; private set; }
    [field: SerializeField, Header("UI")] public UIPanelMenu_MainMenu UIPanelMenu_MainMenu { get; private set; }


}

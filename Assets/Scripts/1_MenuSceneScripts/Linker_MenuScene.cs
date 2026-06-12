using UnityEngine;

public class Linker_MenuScene : MonoBehaviour
{   

    [field: SerializeField, Header("Main Systems")] public MenuScene_Starter MenuScene_Starter { get; private set; }
    [field: SerializeField, Header("UI")] public UIPanelMenu_MainMenu UIPanelMenu_MainMenu { get; private set; }

    private void Awake()
    {
        GameServiceLocator.RegisterService<Linker_MenuScene>(this);
    }

    private void OnDestroy()
    {
        GameServiceLocator.UnregisterService<Linker_MenuScene>();
    }


}

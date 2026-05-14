using UnityEngine;

// This class is used to link all the systems and execution scripts in the main scene, so that they can be easily accessed from other scripts. 
// It is also used to link the game scene and menu scene linkers, so that they can be accessed from the main scene.
public class Linker_MainScene : MonoBehaviour
{
    [field: SerializeField, Header("Systems")] public LoadScreen_System LoadScreen_System { get; private set; }
    [field: SerializeField] public SaveLoadSystem SaveLoadSystem { get; private set; }

    [field: SerializeField, Header("Execution")] public Execution_Initial Execution_Initial { get; private set; }
    [field: SerializeField] public Execution_MenuScene Execution_MenuScene { get; private set; }
    [field: SerializeField] public Execution_GameScene Execution_GameScene { get; private set; }

    [field: SerializeField, Header("Persistance")] public Persistance_Data Persistance_Data { get; private set; }

    [field: SerializeField, Header("Library")] public Linker_Library Linker_Library { get; private set; }
}

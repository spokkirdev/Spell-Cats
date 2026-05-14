using UnityEngine;

public class Linker_Library : MonoBehaviour
{
    public static Linker_Library Reference => GameCoreApplication.Instance.Linker_MainScene.Linker_Library;

    [field: SerializeField] public Library_General Library_General { get; private set; }

}

using UnityEngine;

public class Linker_Library : MonoBehaviour
{

    [field: SerializeField] public Library_General Library_General { get; private set; }

    private void Awake()
    {
        GameServiceLocator.RegisterService<Linker_Library>(this);
    }

    private void OnDestroy()
    {
        GameServiceLocator.UnregisterService<Linker_Library>();
    }

}

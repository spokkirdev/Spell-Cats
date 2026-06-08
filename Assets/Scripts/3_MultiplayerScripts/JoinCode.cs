using TMPro;
using UnityEngine;

public class JoinCode : MonoBehaviour
{

    private static JoinCode _instance;
    public static JoinCode Singleton => _instance;

    public TMP_Text joinCodeText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Debug.LogWarning("Multiple instances of JoinCode detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

}

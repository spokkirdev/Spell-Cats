using UnityEngine;

public class EndGameCanvasManager : MonoBehaviour
{

    [SerializeField] private TMPro.TMP_Text pointsText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        pointsText.text = GameServiceLocator.GetService<NetworkPoints>().Points.Value.ToString();
    }
}

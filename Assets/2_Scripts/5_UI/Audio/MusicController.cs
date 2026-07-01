using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private Sprite UnmutedIcon;
    [SerializeField] private Sprite MutedIcon;

    private bool _isMuted = false;



    public void OutToggleMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
            GetComponent<UnityEngine.UI.Image>().sprite = MutedIcon;
        }
        else
        {
            musicSource.Play();
            GetComponent<UnityEngine.UI.Image>().sprite = UnmutedIcon;
        }
    }

    public void Out_ToggleMusicUI()
    {
        if (_isMuted)
        {
            _isMuted = false;
            GetComponent<UnityEngine.UI.Image>().sprite = UnmutedIcon;
        }
        else
        {
            _isMuted = true;
            GetComponent<UnityEngine.UI.Image>().sprite = MutedIcon;
        }
    }
}

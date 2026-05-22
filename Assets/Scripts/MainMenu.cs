using LunarAnomaly;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    GameManager gm;

    void Awake()
    {
        gm = GameManager.Instance;        
    }

    void Start()
    {
        SoundManager.PlayMusic(SoundType.Music, 0.2f);
    }

    public void PlayGame()
    {
        SoundManager.StopMusic();
        SoundManager.PlaySound(SoundType.MenuClick, 1f, false);
        gm.PlayGame();
    }

    public void QuitGame()
    {
        SoundManager.PlaySound(SoundType.MenuClick, 1f, false);
        gm.QuitGame();
    }
}

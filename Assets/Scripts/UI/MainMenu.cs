using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Settings _settings;


    private void OnEnable()
    {
        Stats.Load();
    }


    public void OpenSettings()
    {
        _settings.gameObject.SetActive(true);
        _settings.OnOpen();
    }


    public void CloseSettings()
    {
        _settings.OnClose();
        _settings.gameObject.SetActive(false);
    }


    public void Launch()
    {
        SceneTransiter.TransiteTo( Scene.Game );
    }
}
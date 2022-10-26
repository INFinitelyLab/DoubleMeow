using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider _FPS;
    [SerializeField] private Slider _PostProcessing;


    public void OnOpen()
    {
        _FPS.value = Mathf.Ceil(Stats.TargetFPS / 60);
        _PostProcessing.value = Stats.IsEnablePostProcess ? 1 : 0;
    }


    public void SetFPS(float value)
    {
        switch(value)
        {
            case 0: Stats.TargetFPS = 30; break;
            case 1: Stats.TargetFPS = 60; break;
            case 2: Stats.TargetFPS = 120; break;
        }
    }


    public void SetGraphicsPreset(float value)
    {
        Stats.IsEnablePostProcess = value == 1;
    }


    public void OnClose()
    {
        Stats.Save();
    }
}
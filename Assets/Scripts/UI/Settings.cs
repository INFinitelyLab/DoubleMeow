using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider _FPS;
    [SerializeField] private Slider _PostProcessing;


    public void OnOpen()
    {
        _FPS.value = Mathf.Ceil(Stats.TargetFPS / 60);
        _PostProcessing.value = (int)Stats.TargetGraphics;
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
        Stats.TargetGraphics = (GraphicPreset)value;
    }


    public void OnClose()
    {
        Stats.Save();
    }
}
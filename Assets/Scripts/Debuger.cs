using UnityEngine;
using TMPro;
using System.Diagnostics;

public class Debuger : MonoBehaviour
{
    public TextMeshProUGUI text;

    public float updateInterval = 0.5F;
    private double lastInterval;
    private int frames;
    private float fps;
    private int level;


    void Start()
    {
        if(Application.platform == RuntimePlatform.Android) Application.targetFrameRate = 60;
        if (Application.platform == RuntimePlatform.WindowsPlayer) QualitySettings.vSyncCount = -1;

        Stats.LevelUped += OnLevelUped;
        level = Stats.Level;

        lastInterval = Time.realtimeSinceStartup;
        frames = 0;

        UpdateInfo();
    }


    void OnLevelUped(int level)
    {
        this.level = level;
    }


    void UpdateInfo()
    {
        string info = "FPS : " + fps + "\n";

        text.text = info;

        Invoke(nameof(UpdateInfo), 0.5f);
    }


    void Update()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;
        }
    }
}
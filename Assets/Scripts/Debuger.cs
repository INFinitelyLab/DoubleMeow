using UnityEngine;
using TMPro;
using System.Diagnostics;
using System.Text;

public class Debuger : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject _generation;

    public float updateInterval = 0.5F;
    private double lastInterval;
    private int frames;
    private float fps;

    private int level;
    private int deathCount;


    void Start()
    {
        if(Application.platform == RuntimePlatform.Android) Application.targetFrameRate = 120;
        if (Application.platform == RuntimePlatform.WindowsPlayer) QualitySettings.vSyncCount = -1;

        Resources.LoadAll<Material>("");

        Stats.LevelUped += OnLevelUped;

        level = Stats.Level;
        deathCount = Stats.DeathCount;

        lastInterval = Time.realtimeSinceStartup;
        frames = 0;

        UpdateInfo();

        Destroy(_generation);
    }


    void OnLevelUped(int level)
    {
        this.level = level;
    }


    void UpdateInfo()
    {
        StringBuilder info = new StringBuilder();

        info.Append("FPS : ");
        info.Append(fps);
        info.Append("\n");
        info.Append("Level : ");
        info.Append(level);
        info.Append("\n");
        info.Append("Difficulty : ");
        info.Append(Game.Difficulty);
        info.Append("\n");
        info.Append("DeathCount : ");
        info.Append(deathCount);
        info.Append("\n");
        info.Append("Jump Multiplier : ");
        info.Append(Stats.PerfectJumpCount);

        text.text = info.ToString();

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
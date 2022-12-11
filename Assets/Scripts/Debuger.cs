using UnityEngine;
using UnityEditor;
using TMPro;
using System.Text;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class Debuger : SingleBehaviour<Debuger>
{
    public string selectedSkin;

    public TextMeshProUGUI text;
    public GameObject _generation;

    public GameObject losePanel;

    public float updateInterval = 0.5F;
    private double lastInterval;
    private int frames;
    private float fps;

    private int level;
    private int deathCount;

    public UniversalRenderPipelineAsset _asset;


    public void OnGameEnd()
    {
        Invoke("_OnGameEnd", 1);
    }

    private void _OnGameEnd()
    {
        losePanel.SetActive(true);
    }


    public void OnRescale(float factor)
    {
        _asset.renderScale = factor;
    }

    [ContextMenu("Reselect Skin")]
    public void SelectSkin()
    {
        Stats.SelectSkin(selectedSkin);
    }


    void Start()
    {
        Resources.LoadAll<Material>("");

        Stats.LevelUped += OnLevelUped;

        level = Stats.Level;
        deathCount = Stats.DeathCount;

        lastInterval = Time.realtimeSinceStartup;
        frames = 0;

        UpdateInfo();

        Destroy(_generation);

        GraphicsSettings.useScriptableRenderPipelineBatching = true;
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
        /*info.Append("\n");
        info.Append("CPU : ");
        info.Append(FrameTimingManager.GetCpuTimerFrequency());
        info.Append("\n");
        info.Append("CPU : ");
        info.Append(FrameTimingManager.GetGpuTimerFrequency());*/

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


    private void OnDisable()
    {
        _asset.renderScale = 1;
    }

#if UNITY_EDITOR
    [MenuItem("Milks/Get Free 100 Milks!")]
    private static void GetFree100Coins() => GetFreeCoins(100);

    [MenuItem("Milks/Get Free 1000 Milks!")]
    private static void GetFree1000Coins() => GetFreeCoins(1000);

    [MenuItem("Milks/Get Free 10000 Milks!")]
    private static void GetFree10000Coins() => GetFreeCoins(10000);

    [MenuItem("Milks/Get Free 100000 Milks!")]
    private static void GetFree100000Coins() => GetFreeCoins(100000);


    private static void GetFreeCoins(int coinsCount)
    {
        Stats.Load();

        Stats.IncreaseCoin(coinsCount);

        Stats.Save();
    }
#endif
}
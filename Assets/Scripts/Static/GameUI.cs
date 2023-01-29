using UnityEngine;
using TMPro;

public class GameUI : SingleBehaviour<GameUI>
{
    [SerializeField] private TextMeshProUGUI _coinsBar;
    [SerializeField] private RectTransform _dronePanel;
    [SerializeField] private RectTransform _losePanel;

    protected override void OnActive()
    {
        OnCoinsCountChanged( Stats.Coins );

        Game.MilkChanged += OnCoinsCountChanged;
    }

    protected override void OnDisactive()
    {
        Game.MilkChanged -= OnCoinsCountChanged;
    }


    public void OnCoinsCountChanged(int value)
    {
        _coinsBar.text = value.ToString();
    }


    public static void OpenDronePanel()
    {
        if (Game.InMiniGames)
            Instance.Invoke(nameof(_OpenLosePanel), 1f);
        else
            Instance.Invoke(nameof(_OpenDronePanel), 1f);
    }

    public static void OpenLosePanel()
    {
        Instance.Invoke(nameof(_OpenLosePanel), 0f);
    }

    public static void RegenerateByHeart()
    {
        if (Heart.TryUse() == false) return;

        Game.Regenerate();

        Instance._dronePanel.gameObject.SetActive(false);
    }

    public static void RegenerateByAD()
    {
        Game.Regenerate();

        Instance._dronePanel.gameObject.SetActive(false);
    }

    private void _OpenDronePanel()
    {
        _dronePanel.gameObject.SetActive(true);
    }

    private void _OpenLosePanel()
    {
        _losePanel.gameObject.SetActive(true);

        LosePanel.Initialize();
    }
}
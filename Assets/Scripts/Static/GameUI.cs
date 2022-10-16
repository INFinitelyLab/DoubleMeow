using UnityEngine;
using TMPro;

public class GameUI : SingleBehaviour<GameUI>
{
    [SerializeField] private TextMeshProUGUI _coinsBar;

    protected override void OnActive()
    {
        OnCoinsCountChanged( Stats.Coins );

        Stats.CoinsCountChanged += OnCoinsCountChanged;
    }

    protected override void OnDisactive()
    {
        Stats.CoinsCountChanged -= OnCoinsCountChanged;
    }

    public void OnCoinsCountChanged(int value)
    {
        _coinsBar.text = value.ToString();
    }
}
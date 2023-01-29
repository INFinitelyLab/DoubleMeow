using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattlepassChunk : MonoBehaviour
{
    [SerializeField] private Image _regularItemIcon;
    [SerializeField] private Image _premiumItemIcon;
    [SerializeField] private Image _progressBar;
    [SerializeField] private TextMeshProUGUI _levelBar;

    private BattlepassCell _cell;


    public void Initialize(BattlepassCell cell, int level)
    {
        _regularItemIcon.sprite = cell.regularItem.Icon;
        _premiumItemIcon.sprite = cell.premiumItem.Icon;

        _levelBar.text = level.ToString();
        _cell = cell;

        UpdateStates( Battlepass.AvailableItems.Contains( cell.regularItem ), Battlepass.AvailableItems.Contains( cell.premiumItem ) );

        if (Battlepass.Level == level - 1)
            _progressBar.fillAmount = Mathf.Clamp01((float)Battlepass.BattleCoins / (float)Battlepass.Destination);
        else
            _progressBar.fillAmount = level - 1 < Battlepass.Level ? 1 : 0;
    }


    public void UpdateStates(bool isRegularAvailable, bool isPremiumAvailable)
    {
        _regularItemIcon.color = isRegularAvailable ? Color.green : Color.white;
        _premiumItemIcon.color = isPremiumAvailable ? Color.green : Color.white;
    }


    public void SelectRegular()
    {
        if (Battlepass.TryPickItem(_cell.regularItem))
        {
            if (Inventory.Has(_cell.regularItem) == false)
            {
                BattlepassUI.Instance.CreatePresenterItem( _regularItemIcon.rectTransform.sizeDelta, _regularItemIcon.rectTransform.position, _regularItemIcon.sprite );

                if (_cell.regularItem is LeatherBox)
                    _cell.regularItem.OnPurchased();
                else
                    Inventory.Recieve(_cell.regularItem);
            }
        }

        UpdateStates(Battlepass.AvailableItems.Contains(_cell.regularItem), Battlepass.AvailableItems.Contains(_cell.premiumItem));
    }


    public void SelectPremium()
    {
        if (Battlepass.TryPickItem(_cell.premiumItem))
        {
            if (Inventory.Has(_cell.premiumItem) == false)
            {
                BattlepassUI.Instance.CreatePresenterItem( _premiumItemIcon.rectTransform.sizeDelta, _premiumItemIcon.rectTransform.position, _premiumItemIcon.sprite);

                if (_cell.premiumItem is LeatherBox)
                    _cell.premiumItem.OnPurchased();
                else
                    Inventory.Recieve(_cell.premiumItem);
            }
        }

        UpdateStates(Battlepass.AvailableItems.Contains(_cell.regularItem), Battlepass.AvailableItems.Contains(_cell.premiumItem));
    }
}

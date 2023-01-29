using UnityEngine;
using TMPro;

public class CoinsBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textBar;

    private void OnEnable()
    {
        Bank.CoinsIncreased += UpdateInfo;
        Bank.CoinsDecreased += UpdateInfo;
    }

    private void OnDisable()
    {
        Bank.CoinsIncreased -= UpdateInfo;
        Bank.CoinsDecreased -= UpdateInfo;
    }

    private void UpdateInfo(int coins)
    {
        _textBar.text = coins.ToString();
    }
}

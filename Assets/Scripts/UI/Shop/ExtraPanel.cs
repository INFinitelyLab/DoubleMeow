using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExtraPanel : MonoBehaviour
{
    [field: SerializeField] public Button BuyButton { get; private set; }
    [field: SerializeField] public TextMeshProUGUI LevelBar { get; private set; }

    [SerializeField] private string _prefix;


    public void UpdateInfo(bool buttonEnabled, int level)
    {
        BuyButton.enabled = buttonEnabled;
        BuyButton.image.color = buttonEnabled ? Color.yellow : Color.green;

        LevelBar.text = buttonEnabled? (_prefix + level.ToString()) : (_prefix + "Max");
    }
}

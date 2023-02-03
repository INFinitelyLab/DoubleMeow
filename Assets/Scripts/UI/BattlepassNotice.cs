using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BattlepassNotice : MonoBehaviour
{
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();

        UpdateState();
    }


    private void OnEnable() => MainMenu.WindowAreOpenOrClosed += UpdateState;

    private void OnDisable() => MainMenu.WindowAreOpenOrClosed -= UpdateState;


    public void UpdateState()
    {
        _image.enabled = Battlepass.AvailableItems.Count > 0;
    }
}

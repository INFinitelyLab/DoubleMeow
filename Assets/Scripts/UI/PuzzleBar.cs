using UnityEngine;
using TMPro;

public class PuzzleBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _grayBar;
    [SerializeField] private TextMeshProUGUI _blueBar;
    [SerializeField] private TextMeshProUGUI _purpleBar;
    [SerializeField] private TextMeshProUGUI _goldBar;

    private void OnEnable()
    {
        Puzzle.Changed += UpdateInfo;

        UpdateInfo();
    }

    private void OnDisable()
    {
        Puzzle.Changed -= UpdateInfo;
    }

    private void UpdateInfo()
    {
        _grayBar.text = Puzzle.Gray.Count.ToString() + " Gray";
        _blueBar.text = Puzzle.Blue.Count.ToString() + " Blue";
        _purpleBar.text = Puzzle.Purple.Count.ToString() + " Purp";
        _goldBar.text = Puzzle.Gold.Count.ToString() + " Gold";
    }
}

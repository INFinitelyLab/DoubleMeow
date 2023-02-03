using UnityEngine;
using TMPro;

public class PuzzlePanel : MonoBehaviour
{
    [field: SerializeField] public TextMeshProUGUI _countBar { get; private set; }


    public void UpdateInfo( int count )
    {
        _countBar.text = count.ToString();
    }
}

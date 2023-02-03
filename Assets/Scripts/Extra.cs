using UnityEngine;

[CreateAssetMenu(fileName = "Бонусы")]
public class Extra : ScriptableObject
{
    [SerializeField] private int _magnetPrice;
    [SerializeField] private int _doublePrice;
    [SerializeField] private int _rocketPrice;
    [SerializeField] private int _ticketPrice;
    [SerializeField] private int _heartPrice;
    [SerializeField] private int _acceleratorPrice;
    [SerializeField] private int _puzzleGrayPrice;
    [SerializeField] private int _puzzleBluePrice;
    [SerializeField] private int _puzzlePurplePrice;
    [SerializeField] private int _puzzleGoldPrice;


    public static int MagnetPrice => SelectedExtra._magnetPrice;
    public static int DoublePrice => SelectedExtra._doublePrice;
    public static int RocketPrice => SelectedExtra._rocketPrice;
    public static int TicketPrice => SelectedExtra._ticketPrice;
    public static int HeartPrice => SelectedExtra._heartPrice;
    public static int AcceleratorPrice => SelectedExtra._acceleratorPrice;
    public static int PuzzleGrayPrice => SelectedExtra._puzzleGrayPrice;
    public static int PuzzleBluePrice => SelectedExtra._puzzleBluePrice;
    public static int PuzzlePurplePrice => SelectedExtra._puzzlePurplePrice;
    public static int PuzzleGoldPrice => SelectedExtra._puzzleGoldPrice;

    #region Editor

    [ExecuteInEditMode]
    private void OnEnable()
    {
        if (SelectedExtra != null)
            DestroyImmediate(this, true);
        else
            SelectedExtra = this;
    }

    [ExecuteInEditMode]
    private void OnDisable()
    {
        if (SelectedExtra != this) return;

        SelectedExtra = null;
    }

    private static Extra SelectedExtra { get; set; }

    #endregion
}

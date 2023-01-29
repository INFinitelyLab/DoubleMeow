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

    public static int MagnetPrice => SelectedExtra._magnetPrice;
    public static int DoublePrice => SelectedExtra._doublePrice;
    public static int RocketPrice => SelectedExtra._rocketPrice;
    public static int TicketPrice => SelectedExtra._ticketPrice;
    public static int HeartPrice => SelectedExtra._heartPrice;
    public static int AcceleratorPrice => SelectedExtra._acceleratorPrice;

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

using UnityEngine;
using TMPro;


public class MiniGamesSelector : MonoBehaviour
{
    [SerializeField] private RectTransform _metroerButton;
    [SerializeField] private RectTransform _curverButton;
    [SerializeField] private RectTransform _tilerButton;
    [SerializeField] private RectTransform _retroButton;
    [SerializeField] private TextMeshProUGUI _ticketBar;

    public MiniGameType SelectedGameType { get; private set; }


    private void Awake() => SelectMiniGame( MiniGameType.Metroer );

    private void OnEnable() => _ticketBar.text = Ticket.Count.ToString();


    public void Play()
    {
        if (SelectedGameType == MiniGameType.None) return;

        if (Ticket.TryUse() == false) return;

        switch (SelectedGameType)
        {
            case MiniGameType.Retrowave:
                MainMenu.LaunchRetrowaver();
                break;
            case MiniGameType.Metroer:
                MainMenu.LaunchMetroer();
                break;
            case MiniGameType.Curver:
                MainMenu.LaunchCurver();
                break;
            case MiniGameType.Tiler:
                MainMenu.LaunchTiler();
                break;
        }
    }


    public void TryBuyTicket()
    {
        int price = Extra.TicketPrice;

        if (Bank.TryDecreaseCoins(price))
        {
            Ticket.Pickup();

            _ticketBar.text = Ticket.Count.ToString();
        }
    }


    public void Select_Retrowaver() => SelectMiniGame( MiniGameType.Retrowave );

    public void Select_Metroer() => SelectMiniGame( MiniGameType.Metroer );

    public void Select_Curver() => SelectMiniGame( MiniGameType.Curver );
    
    public void Select_Tiler() => SelectMiniGame( MiniGameType.Tiler );


    private void SelectMiniGame(MiniGameType type)
    {
        GetButtonByType(SelectedGameType).localScale = Vector3.one;

        SelectedGameType = type;

        GetButtonByType(SelectedGameType).localScale = Vector3.one * 1.25f;
    }


    private RectTransform GetButtonByType( MiniGameType type )
    {
        switch (type)
        {
            case MiniGameType.Retrowave:
                return _retroButton;
            case MiniGameType.Metroer:
                return _metroerButton;
            case MiniGameType.Curver:
                return _curverButton;
            case MiniGameType.Tiler:
                return _tilerButton;

            default: return null;
        }
    }
}

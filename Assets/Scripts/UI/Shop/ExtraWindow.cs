using UnityEngine;

[DisallowMultipleComponent]
public class ExtraWindow : ShopWindow
{
    [SerializeField] private ExtraPanel _magnetPanel;
    [SerializeField] private ExtraPanel _doublePanel;
    [SerializeField] private ExtraPanel _rocketPanel;
    [SerializeField] private ExtraPanel _heartPanel;
    [SerializeField] private ExtraPanel _acceleratorPanel;

    [SerializeField] private PuzzlePanel _puzzleGrayPanel;
    [SerializeField] private PuzzlePanel _puzzleBluePanel;
    [SerializeField] private PuzzlePanel _puzzlePurplePanel;
    [SerializeField] private PuzzlePanel _puzzleGoldPanel;


    private void OnEnable() => UpdateInfo();

    public void UpdateInfo()
    {
        _magnetPanel.UpdateInfo( Magnet.IsFullUpgraded == false, Magnet.Level );
        _doublePanel.UpdateInfo( Double.IsFullUpgraded == false, Double.Level );
        _rocketPanel.UpdateInfo( Rocket.IsFullUpgraded == false, Rocket.Level );
        _heartPanel.UpdateInfo( true, Heart.Count );
        _acceleratorPanel.UpdateInfo( true, Accelerator.Count );

        _puzzleGrayPanel.UpdateInfo( Puzzle.Gray.Count );
        _puzzleBluePanel.UpdateInfo( Puzzle.Blue.Count );
        _puzzlePurplePanel.UpdateInfo( Puzzle.Purple.Count );
        _puzzleGoldPanel.UpdateInfo( Puzzle.Gold.Count );
    }


    private void TryBuy( ExtraType type )
    {
        int price = 100;

        switch (type)
        {
            case ExtraType.Magnet:
                price = Extra.MagnetPrice;
                break;
            case ExtraType.Double:
                price = Extra.DoublePrice;
                break;
            case ExtraType.Rocket:
                price = Extra.RocketPrice;
                break;
            case ExtraType.Heart:
                price = Extra.HeartPrice;
                break;
            case ExtraType.Accelerator:
                price = Extra.AcceleratorPrice;
                break;
            case ExtraType.PuzzleGray:
                price = Extra.PuzzleGrayPrice;
                break;
            case ExtraType.PuzzleBlue:
                price = Extra.PuzzleBluePrice;
                break;
            case ExtraType.PuzzlePurple:
                price = Extra.PuzzlePurplePrice;
                break;
            case ExtraType.PuzzleGold:
                price = Extra.PuzzleGoldPrice;
                break;

        }

        if (Bank.TryDecreaseCoins(price))
        {
            switch (type)
            {
                case ExtraType.Magnet:
                    Magnet.Upgrade();
                    break;
                case ExtraType.Double:
                    Double.Upgrade();
                    break;
                case ExtraType.Rocket:
                    Rocket.Upgrade();
                    break;
                case ExtraType.Heart:
                    Heart.Pickup();
                    break;
                case ExtraType.Accelerator:
                    Accelerator.Pickup();
                    break;
                case ExtraType.PuzzleGray:
                    Puzzle.Gray.Pickup();
                    break;
                case ExtraType.PuzzleBlue:
                    Puzzle.Blue.Pickup();
                    break;
                case ExtraType.PuzzlePurple:
                    Puzzle.Purple.Pickup();
                    break;
                case ExtraType.PuzzleGold:
                    Puzzle.Gold.Pickup();
                    break;
            }

            UpdateInfo();
        }
    }



    public void TryBuyMagnet()
    {
        if (Magnet.IsFullUpgraded) return;

        TryBuy(ExtraType.Magnet);
    }

    public void TryBuyDouble()
    {
        if (Double.IsFullUpgraded) return;

        TryBuy(ExtraType.Double);
    }

    public void TryBuyRocket()
    {
        if (Rocket.IsFullUpgraded) return;

        TryBuy(ExtraType.Rocket);
    }

    public void TryBuyHeart()
    {
        TryBuy(ExtraType.Heart);
    }

    public void TryBuyAccelerator()
    {
        TryBuy(ExtraType.Accelerator);
    }


    public void TryBuyPuzzleGray()
    {
        TryBuy(ExtraType.PuzzleGray);
    }

    public void TryBuyPuzzleBlue()
    {
        TryBuy(ExtraType.PuzzleBlue);
    }

    public void TryBuyPuzzlePurple()
    {
        TryBuy(ExtraType.PuzzlePurple);
    }

    public void TryBuyPuzzleGold()
    {
        TryBuy(ExtraType.PuzzleGold);
    }




    public override void Enable(ShopWindowType type) { gameObject.SetActive(true); }

    public override void Disable() { gameObject.SetActive(false); }

    public override bool TryGetElementByIndex(int index, out InventoryItem item) { item = null; return false; }
}

public enum ExtraType
{
    Magnet,
    Double,
    Rocket,
    Heart,
    Accelerator,

    PuzzleGray,
    PuzzleBlue,
    PuzzlePurple,
    PuzzleGold
}
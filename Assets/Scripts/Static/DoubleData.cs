using UnityEngine;
using System.IO;
using System.Linq;

public static class DoubleData
{
    // ===== Debug Member | Delete after connect to google services ===== //
    private const string SavePath = @"C:\Users\INF\Desktop\save.json";


    public static void Download()
    {
        string json = File.ReadAllText(SavePath, System.Text.Encoding.Unicode);

        SaveData data = JsonUtility.FromJson<SaveData>( json );

        if (data == null) return;

        Bank.Update( new BankData(data.Coins) );
        Notice.Initialize( data.Notices );
        Inventory.Update( new InventoryData( data.Inventory, data.PuzzledItems ) );
        Battlepass.Update( new BattlepassData( data.BattlepassLevel, data.BattlepassProgress, data.BattlepassIsPremium, data.BattlepassAvailableItems ) );

        Puzzle.Initialize( data.PuzzleGray, data.PuzzleBlue, data.PuzzlePurple, data.PuzzleGold );

        Magnet.Initialize( data.MagnetLevel );
        Double.Initialize( data.DoubleLevel );
        Rocket.Initialize( data.RocketLevel );
        Ticket.Initialize( data.TicketCount );
        Heart.Initialize( data.HeartCount );
        Accelerator.Initialize( data.AcceleratorCount );

        if (Inventory.TryGetItemByID(data.SelectedSkin, out var item))
        {
            Skin.Update(item as Skin);
        }
    }


    public static void Upload()
    {
        SaveData data = new SaveData();

        data.Coins = Bank.Coins;
        data.Notices = Notice.GetItemsIDs();
        data.Inventory = Inventory.GetItemsIDs();
        data.PuzzledItems = Inventory.PuzzledItems.Select( p => p.GetData() ).ToArray();

        data.BattlepassLevel = Battlepass.Level;
        data.BattlepassProgress = Battlepass.BattleCoins;
        data.BattlepassIsPremium = Battlepass.IsPremium;

        data.PuzzleGray = Puzzle.Gray.Count;
        data.PuzzleBlue = Puzzle.Blue.Count;
        data.PuzzlePurple = Puzzle.Purple.Count;
        data.PuzzleGold = Puzzle.Gold.Count;

        data.MagnetLevel = Magnet.Level;
        data.DoubleLevel = Double.Level;
        data.RocketLevel = Rocket.Level;
        data.TicketCount = Ticket.Count;
        data.HeartCount = Heart.Count;
        data.AcceleratorCount = Accelerator.Count;
        
        if (Skin.Current != null) data.SelectedSkin = Skin.Current.ProductID;
        if (Battlepass.AvailableItems != null) data.BattlepassAvailableItems = Battlepass.AvailableItems.Select( i => i.ProductID ).ToArray();

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(SavePath, json, System.Text.Encoding.Unicode);
    }
}


[System.Serializable]
public class SaveData
{
    public int Coins = 0;

    public int BattlepassLevel = 0;
    public int BattlepassProgress = 0;
    public bool BattlepassIsPremium = false;
    public string[] BattlepassAvailableItems = new string[0];

    public string[] Notices = new string[0];
    public string[] Inventory = new string[0];
    public PuzzledItemData[] PuzzledItems = new PuzzledItemData[0];

    public string SelectedSkin = "Cherry";

    public int PuzzleGray = 0;
    public int PuzzleBlue = 0;
    public int PuzzlePurple = 0;
    public int PuzzleGold = 0;

    public int MagnetLevel = 0;
    public int DoubleLevel = 0;
    public int RocketLevel = 0;
    public int TicketCount = 0;
    public int HeartCount = 0;
    public int AcceleratorCount = 0;
}
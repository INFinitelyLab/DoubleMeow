using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Selector : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;


    public void UpdateInfo( InventoryItem item)
    {
        _image.color = GetColorByInventoryItem(item);

        _text.text = GetTextByInventoryItem(item);
    }


    private static Color GetColorByInventoryItem( InventoryItem item )
    {
        if (Inventory.Has(item) == true)
            return Color.green;

        return new Color(0,1,1);
    }

    private static string GetTextByInventoryItem( InventoryItem item )
    {
        if (Inventory.Has(item) == true) return "Select";

        if (item is SkinPuzzled skin) return skin.PuzzleCollected + " " + skin.PuzzleType.ToString();

        if (item == Discounter.TodayItem) return (item.Price * (1 - ((float)Discounter.RandomChance / 100f))).ToString() + " (-" + Discounter.RandomChance + ")";

        return item.Price + (item.IsDonateItem ? "$" : "");
    }


    private void OnEnable()
    {
        Shop.Reselected += UpdateInfo;
    }

    private void OnDisable()
    {
        Shop.Reselected -= UpdateInfo;
    }
}

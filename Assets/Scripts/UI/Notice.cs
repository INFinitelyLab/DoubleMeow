using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Notice : MonoBehaviour
{
    #region Dynamic

    private void Awake() => gameObject.SetActive(false);

    private void OnDisable() => gameObject.SetActive(false);

    public void UpdateState(InventoryItem item)
    {
        Debug.Log( "State : " + _all.Contains(item) );

        gameObject.SetActive( _all.Contains(item) );
    }


    #endregion

    #region Static

    public static IReadOnlyCollection<InventoryItem> All => _all;

    private static List<InventoryItem> _all = new List<InventoryItem>(10);


    public static void Notify(InventoryItem item)
    {
        if (_all.Contains(item)) return;

        _all.Add(item);
    }

    public static void Unotify(InventoryItem item)
    {
        if (_all.Contains(item) == false) return;

        _all.Remove(item);
    }

    public static void Initialize(string[] itemsIDs)
    {
        _all.Clear();

        foreach( string ID in itemsIDs )
        {
            if (Inventory.TryGetItemByID(ID, out var item))
            {
                Notify(item);
            }
        }
    }

    public static string[] GetItemsIDs()
    {
        return _all.Select(i => i.ProductID).ToArray();
    }

    #endregion
}

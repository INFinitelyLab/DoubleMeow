using UnityEngine;
using System;

[CreateAssetMenu( fileName = "Discounter")]
public class Discounter : ScriptableObject
{
    [SerializeField] private InventoryItem[] _possibleItems;
    [SerializeField] private int[] _chances;

    public static int RandomChance => Instance._chances[ (((int)DateTime.Today.DayOfWeek) + DateTime.Today.Day + DateTime.Today.Month) % Instance._chances.Length ];

    public static InventoryItem TodayItem => Instance._possibleItems[ (DateTime.Today.Year * DateTime.Today.Month * DateTime.Today.Day) % Instance._possibleItems.Length ];

    
    #region Editor

    [ExecuteInEditMode]
    private void OnEnable()
    {
        if (Instance != null)
            DestroyImmediate(this, true);
        else
            Instance = this;
    }

    [ExecuteInEditMode]
    private void OnDisable()
    {
        if (Instance != this) return;

        Instance = null;
    }

    public static Discounter Instance { get; private set; }

    #endregion
}

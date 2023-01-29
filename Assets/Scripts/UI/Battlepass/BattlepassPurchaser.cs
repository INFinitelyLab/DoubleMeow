using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using System;
using TMPro;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public sealed class BattlepassPurchaser : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private Image _image;
    private Button _button;

    public Action PurchaseCompleted;
    public Action PurchaseFailed;


    private void OnEnable()
    {
        _image.color = Battlepass.IsPremium ? Color.green : Color.yellow;
        _text.text = Battlepass.IsPremium ? "Purchased!" : "Buy Premium!";

        _button.enabled = Battlepass.IsPremium == false;
    }


    private void Awake()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BattlepassUI : MonoBehaviour
{
    [SerializeField] private Image _progressBar;
    [SerializeField] private BattlepassChunk _chunkPrefab;
    [SerializeField] private Transform _selector;
    [SerializeField] private RectTransform _content;
    [SerializeField] private RectTransform _centerPoint;
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private BattlepassItemPresenter _itemPresenterPrefab;
    [SerializeField] private GameObject _blackscreen;

    public static BattlepassUI Instance { get; private set; }

 
    private void OnEnable()
    {
        Instance = this;

        _progressBar.fillAmount = Mathf.Clamp01( (float)Battlepass.BattleCoins / (float)Battlepass.Destination );
        _progressText.text = Battlepass.BattleCoins + " / " + Battlepass.Destination;

        for (int index = 0; index < Battlepass.Items.Length; index++)
        {
            BattlepassChunk chunk = Instantiate(_chunkPrefab, _content);

            chunk.Initialize( Battlepass.Items[index], index + 1);

            if (index == Battlepass.Level)
            {
                _selector.SetParent( chunk.transform );
                _selector.localPosition = Vector3.zero;

                _content.localPosition = new Vector3(_content.localPosition.x, -chunk.transform.localPosition.y, 0);
            }
        }
    }

    private void OnDisable()
    {
        _selector.parent = null;

        foreach(Transform child in _content)
        {
            Destroy( child.gameObject );
        }
    }

    public void CreatePresenterItem(Vector2 size, Vector3 position, Sprite icon)
    {
        CancelInvoke();

        var presenter = Instantiate( _itemPresenterPrefab, transform );

        presenter.GetComponent<RectTransform>().sizeDelta = size;
        presenter.transform.localScale = Vector3.one;
        presenter.transform.SetSiblingIndex(99);

        presenter.Initialize(position, icon);

        _blackscreen.SetActive(true);

        Invoke(nameof(DisableBlackscreen), 5f);
    }

    private void DisableBlackscreen() => _blackscreen.SetActive(false);


    public void PurchasePremium() => Battlepass.OnPremiumPurchased();
}

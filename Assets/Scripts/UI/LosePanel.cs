using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// =====[ Панель, всплывающая при проигрыше игрока и отказе от возраждения ]===== //
public class LosePanel : SingleBehaviour<LosePanel>
{
    [Header("Верхняя часть панели!")]
        [SerializeField] private SpringEffector _milkImage;
        [SerializeField] private TextMeshProUGUI _milkCount;
    [Header("Coins Reached!")]
        [SerializeField] private Image _progresser;
        [SerializeField] private TextMeshProUGUI _coinsBar;
    [Header("Meow Pass!")]
        [SerializeField] private TextMeshProUGUI _passCoinsBar;
        [SerializeField] private TextMeshProUGUI _passLevelBar;
    [Header("Кнопки!")]
        [SerializeField] private RectTransform _buttonsPanel;

    private static Vector2 _targetPanelPosition;


    public static void Initialize()
    {
        _targetPanelPosition = Instance._buttonsPanel.anchoredPosition;

        Instance._buttonsPanel.anchoredPosition += Vector2.down * 300;

        Instance.StartCoroutine( Process() );
    }


    private static IEnumerator Process()
    {
        Instance._progresser.fillAmount = 0;
        Instance._coinsBar.text = "0";
        Instance._milkCount.text = Game.MilkCollected.ToString();

        yield return new WaitForSeconds(0.5f);

        int MilkRemain = Game.MilkCollected;
        
        Bank.Exchange(Game.MilkCollected);
        Battlepass.Exchange(Game.MilkCollected);

        yield return Instance.StartCoroutine(MoveProgress( Instance._progresser, MilkRemain, 1.5f ));
        yield return Instance.StartCoroutine(MoveButtonsPanel(Instance._buttonsPanel));
    }


    private static IEnumerator MoveProgress(Image progressBar, int milkCount, float speed)
    {
        float progress = 0;
        float lastFillAmount = 0;

        int coins = 0;

        progressBar.fillAmount = 0;

        while (progress < milkCount)
        {
            progressBar.fillAmount = Mathf.Clamp01((progress % Bank.ExchangeRate) / Bank.ExchangeRate);

            if (lastFillAmount > progressBar.fillAmount)
            {
                coins++;
                Instance._coinsBar.text = coins.ToString();
                Instance._passCoinsBar.text = (coins * Bank.ExchangeRate / Battlepass.ExchangeRate).ToString();
                Instance._milkImage.Play();
            }

            Instance._milkCount.text = Mathf.RoundToInt(milkCount - progress).ToString();

            lastFillAmount = progressBar.fillAmount;

            progress = Mathf.MoveTowards( progress, milkCount, speed * Bank.ExchangeRate * Time.deltaTime );

            yield return null;
        }
    }

    private static IEnumerator MoveButtonsPanel(RectTransform panel)
    {
        while( panel.anchoredPosition != _targetPanelPosition )
        {
            panel.anchoredPosition = Vector3.MoveTowards( panel.anchoredPosition, _targetPanelPosition, 300 * Time.deltaTime );

            yield return null;
        }
    }
}

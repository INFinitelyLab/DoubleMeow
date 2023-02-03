using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class MainMenu : MonoBehaviour   
{
    [SerializeField] private Settings _settings;
    [SerializeField] private GameObject _shop;
    [SerializeField] private GameObject _miniGames;
    [SerializeField] private GameObject _battlepass;

    private static MainMenu Instance;
    public static Settings Settings => Instance._settings;
    public static GameObject Shop => Instance._shop;
    public static GameObject MiniGamesWindow => Instance._miniGames;
    public static GameObject BattlepassWindow => Instance._battlepass;

    public static Action WindowAreOpenOrClosed;

    public static bool IsOpened => Instance == null? false : Instance.gameObject.activeInHierarchy;

    public static bool IsIn => Shop.activeInHierarchy == false && MiniGamesWindow.activeInHierarchy == false && BattlepassWindow.activeInHierarchy == false && Settings.gameObject.activeInHierarchy == false;

    private static bool _isNeedToOpen;
    private static bool _isNeedToOpenShop;
    private static bool _isNeedToOpenBattlepass;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (_isNeedToOpen)
        {
            Open();
            _isNeedToOpen = false;
        }
        if (_isNeedToOpenShop)
        {
            OpenShop();
            _isNeedToOpenShop = false;
        }
        if (_isNeedToOpenBattlepass)
        {
            OpenBattlepass();
            _isNeedToOpenBattlepass = false;
        }
    }


    public static void OpenBattlepassAfterReset() => _isNeedToOpenBattlepass = true;

    public static void OpenShopAfterReset() => _isNeedToOpenShop = true;

    public static void OpenAfterReset() => _isNeedToOpen = true;


    public static void Open()
    {
        Instance.gameObject.SetActive(true);

        WindowAreOpenOrClosed?.Invoke();
    }

    public static void Close()
    {
        Instance.gameObject.SetActive(false);

        WindowAreOpenOrClosed?.Invoke();
    }


    public static void OpenSettings()
    {
        Settings.gameObject.SetActive(true);
        Settings.OnOpen();

        WindowAreOpenOrClosed?.Invoke();
    }

    public static void CloseSettings()
    {
        Settings.OnClose();
        Settings.gameObject.SetActive(false);

        WindowAreOpenOrClosed?.Invoke();
    }


    public void OpenShop()
    {
        _shop.SetActive(true);

        WindowAreOpenOrClosed?.Invoke();
    }

    public void CloseShop()
    {
        _shop.SetActive(false);

        WindowAreOpenOrClosed?.Invoke();
    }


    public void OpenMiniGames()
    {
        _miniGames.SetActive(true);

        WindowAreOpenOrClosed?.Invoke();
    }

    public void CloseMiniGames()
    {
        _miniGames.SetActive(false);

        WindowAreOpenOrClosed?.Invoke();
    }


    public void OpenBattlepass()
    {
        _battlepass.SetActive(true);

        WindowAreOpenOrClosed?.Invoke();
    }

    public void CloseBattlepass()
    {
        _battlepass.SetActive(false);

        WindowAreOpenOrClosed?.Invoke();
    }


    public static void ResetScene()
    {
        SceneManager.LoadScene(0);
    }


    public static void Launch()
    {
        if (IsOpened) Close();

        MiniGames.Deactivate();

        Controller.SetNewGameType( GameType.Game );

        ResetScene();
    }

    public static void LaunchMetroer()
    {
        if (Bank.TryDecreaseCoins(100) == false) return;

        MiniGames.Activate( MiniGameType.Metroer );

        Controller.SetNewGameType( GameType.MiniGame );

        ResetScene();
    }

    public static void LaunchCurver()
    {
        if (Bank.TryDecreaseCoins(100) == false) return;

        MiniGames.Activate( MiniGameType.Curver );

        Controller.SetNewGameType( GameType.MiniGame );

        ResetScene();
    }

    public static void LaunchTiler()
    {
        if (Bank.TryDecreaseCoins(100) == false) return;

        MiniGames.Activate( MiniGameType.Tiler );

        Controller.SetNewGameType( GameType.MiniGame );

        ResetScene();
    }

    public static void LaunchRetrowaver()
    {
        if (Bank.TryDecreaseCoins(100) == false) return;

        MiniGames.Activate(MiniGameType.Retrowave);

        Controller.SetNewGameType(GameType.MiniGame);

        ResetScene();
    }
}
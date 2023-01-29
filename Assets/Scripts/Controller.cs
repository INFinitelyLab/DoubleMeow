using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Controller : SingleBehaviour<Controller>
{
    private static GameType _targetGameType = GameType.Game;


    private void Awake()
    {
        Inventory.Initialize();

        DontDestroyOnLoad(gameObject);

        Portal.Reset();
    }


    protected override void OnActive()
    {
        DoubleData.Download();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    protected override void OnDisactive()
    {
        DoubleData.Upload();

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }


    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            switch (_targetGameType)
            {
                case GameType.MiniGame:
                    if (MiniGames.IsReadyToActive)
                    {
                        Game.Launch();
                        MiniGames.Launch();
                    }

                    break;
            }

            //if (Game.IsActive == false) Game.Launch();
        }
        else if (scene.buildIndex == 0)
        {
            if (MiniGames.IsActive) MiniGames.Deactivate();
        }
    }
 
    public void OnSceneUnloaded(Scene scene)
    {
        Game.Mode.DisableAllModes();
        MiniGames.Stop();
        
        if (scene.buildIndex == 1)
        {
            if (Game.IsActive)
            {
                Game.Stop();
            }
        }
    }

    
    public static void SetNewGameType(GameType gameType)
    {
        _targetGameType = gameType;
    }
}

public enum GameType
{
    Game,
    MiniGame
}
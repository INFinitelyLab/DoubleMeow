using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransiter
{
    private static SceneType _sceneToTransite;

    public static void TransiteTo( SceneType scene )
    {
        TransiteAnimator.Fade(ChangeScene);

        _sceneToTransite = scene;
    }


    private static void ChangeScene()
    {
        switch(_sceneToTransite)
        {
            case SceneType.Game:
                SceneManager.LoadScene(1);
                break;

            case SceneType.Menu:
                SceneManager.LoadScene(0);
                break;
        }
    }
}


public enum SceneType
{
    Game,
    Menu
}

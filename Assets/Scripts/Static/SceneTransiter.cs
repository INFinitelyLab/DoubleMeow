using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransiter
{
    private static Scene _sceneToTransite;

    public static void TransiteTo( Scene scene )
    {
        TransiteAnimator.Fade(ChangeScene);

        _sceneToTransite = scene;
    }


    private static void ChangeScene()
    {
        switch(_sceneToTransite)
        {
            case Scene.Game:
                SceneManager.LoadScene(1);
                break;

            case Scene.Menu:
                SceneManager.LoadScene(0);
                break;
        }
    }
}


public enum Scene
{
    Game,
    Menu
}

using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using System;

public sealed class Game : SingleBehaviour<Game>
{
    [SerializeField] private float _levelHeight;

    public static float levelHeight => Instance._levelHeight;
    public static float Difficulty { get; private set; } = 1;

    public static bool IsActive { get; private set; }

    private static Coroutine _acceleration;

    // Инициализация

    protected override void OnActive()
    {
        Achievements.Initialize();
        Skin.Initialize();

        if (IsActive == false) Launch();
    }

    protected override void OnDisactive()
    {
        if (IsActive) Stop();
    }

    // Управление

    private void OnSwiped(Direction direction)
    {
        if (direction.IsHorizontal()) Player.Movement.Surf(direction);

        if (direction == Direction.Up) Player.Movement.Jump();

        if (direction == Direction.Down) Player.Movement.Land();
    }

    // Цикл игры

    public static void Launch()
    {
        if (IsActive == true) throw new Exception("Нельзя запустить игру, игра уже запущена");

        IsActive = true;

        Portal.Reset();

        Difficulty = 1;

        Inputer.Swiped += Instance.OnSwiped;
        Mode.DisableAllModes();

        Stats.Load();

        _acceleration = Instance.StartCoroutine( SpeedUp() );
    }

    public static void Stop()
    {
        if (IsActive == false) throw new Exception("Нельзя остановить игру, игра уже остановлена");

        Inputer.Swiped -= Instance.OnSwiped;

        IsActive = false;

        Stats.Save();

        Instance.StopCoroutine( _acceleration );
    }

    public static void Restart()
    {
        if(IsActive) Game.Stop();

        SceneManager.LoadScene(0);
    }

    private static IEnumerator SpeedUp()
    {
        while(IsActive)
        {
            Difficulty = Mathf.MoveTowards( Difficulty, 2, 0.01f );

            yield return new WaitForSeconds(10);
        }
    }

    // Моды

    public static class Mode
    {


        // Mario Mode

        public static bool InVehicleMode { get; private set; }

        public static void EnableVehicleMode()
        {
            if (InVehicleMode == true) return;

            InVehicleMode = true;

            Player.Movement.EnableVehicleControl();
            Player.Presenter.EnableVehicleMode();
        }

        public static void DisableVehicleMode()
        {
            if (InVehicleMode == false) return;

            InVehicleMode = false;

            Player.Movement.DisableVehicleControl();
            Player.Presenter.DisableVehicleMode();
        }


        // Double Mode

        public static bool InDoubleMode { get; private set; }

        public static void EnableDoubleMode()
        {
            if (InDoubleMode == true) return;

            InDoubleMode = true;

            _doubleModeCoroutine = Instance.StartCoroutine( InvokeDelay(30, DisableDoubleMode) );
        }

        public static void DisableDoubleMode()
        {
            if (InDoubleMode == false) return;

            InDoubleMode = false;

            Instance.StopCoroutine(_doubleModeCoroutine);
        }

        private static Coroutine _doubleModeCoroutine;


        // Magnet Mode

        public static bool InMagnetMode { get; private set; }

        public static void EnableMagnetMode()
        {
            if (InMagnetMode == true) return;

            InMagnetMode = true;

            _magnetModeCoroutine = Instance.StartCoroutine( InvokeDelay(30, DisableMagnetMode) );
        }

        public static void DisableMagnetMode()
        {
            if (InMagnetMode == false) return;

            InMagnetMode = false;

            Instance.StopCoroutine( _magnetModeCoroutine );
        }

        private static Coroutine _magnetModeCoroutine;


        // Other

        public static void DisableAllModes()
        {
            DisableVehicleMode();
            DisableDoubleMode();
            DisableMagnetMode();
        }

        private static System.Collections.IEnumerator InvokeDelay(float duration, Action invokable)
        {
            yield return new WaitForSeconds(duration);

            invokable?.Invoke();
        }
    }
}
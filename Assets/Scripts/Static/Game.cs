using System.Collections;
using UnityEngine;
using System;

public sealed class Game : SingleBehaviour<Game>
{
    [SerializeField] private float _levelHeight;
    [SerializeField] private GameObject _postProcessInstance;

    public static float levelHeight => Instance._levelHeight;
    public static float Difficulty { get; private set; } = 1;

    public static bool IsActive { get; private set; }
    public static int Phase { get; private set; }

    private static Coroutine _acceleration;

    private const float SpeedUpTime = 10; // In Minutes

    // Инициализация

    protected override void OnActive()
    {
        Stats.Load();

        Instance._postProcessInstance.SetActive( Stats.TargetGraphics == GraphicPreset.High );

        Achievements.Initialize();
        Skin.Initialize();

        if (IsActive == true) Stop();
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


    private void OnDraged(float axis)
    {
        Player.Movement.Drag(axis);
    }


    // Цикл игры

    public static void Launch()
    {
        if (IsActive == true) throw new Exception("Нельзя запустить игру, игра уже запущена");

        IsActive = true;

        Portal.Reset();

        Difficulty = 0.8f;
        Phase = 0;

        Inputer.Swiped += Instance.OnSwiped;
        Inputer.Draged += Instance.OnDraged;

        Mode.DisableAllModes();

        Stats.Load();

        _acceleration = Instance.StartCoroutine( SpeedUp() );
    }

    public static void Stop()
    {
        if (IsActive == false) throw new Exception("Нельзя остановить игру, игра уже остановлена");

        Inputer.Swiped -= Instance.OnSwiped;
        Inputer.Draged -= Instance.OnDraged;

        IsActive = false;

        Stats.Save();
        Stats.OnUnperfectJump();

        Instance.StopCoroutine( _acceleration );
    }

    public static void Restart()
    {
        if(IsActive) Stop();

        SceneTransiter.TransiteTo(Scene.Game);
    }

    public static void Menu()
    {
        SceneTransiter.TransiteTo( Scene.Menu );
    }

    private static IEnumerator SpeedUp()
    {
        while(Phase == 0)
        {
            yield return new WaitForSeconds(10);

            Difficulty = Mathf.MoveTowards( Difficulty, 1.6f, 10 / (SpeedUpTime * 60) );

            if (Difficulty >= 1.4f) Phase = 1;
        }

        while (Phase == 1)
        {
            yield return new WaitForSeconds(10);

            Difficulty = Mathf.MoveTowards(Difficulty, 3.2f, 10 / (SpeedUpTime * 60) / 2);
        }
    }

    // Моды

    public static class Mode
    {


        // Mario Card Mode

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


        // Curve Mode

        public static bool InCurveMode { get; private set; }

        public static void EnableCurveMode()
        {
            if (InCurveMode == true) return;

            InCurveMode = true;

            Player.Movement.EnableCurveControl();
        }

        public static void DisableCurveMode()
        {
            if (InCurveMode == false) return;

            InCurveMode = false;

            Player.Movement.DisableCurveControl();
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
            DisableCurveMode();
        }

        private static System.Collections.IEnumerator InvokeDelay(float duration, Action invokable)
        {
            yield return new WaitForSeconds(duration);

            invokable?.Invoke();
        }
    }
}
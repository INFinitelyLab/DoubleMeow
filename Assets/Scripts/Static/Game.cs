using System.Collections;
using UnityEngine;
using System;

public sealed class Game : SingleBehaviour<Game>
{
    [SerializeField] private float _levelHeight;
    [SerializeField] private GameObject _postProcessInstance;

    public static float levelHeight => Instance._levelHeight;
    public static float Difficulty { get; private set; } = 1;
    public static float PassedTime { get; private set; } = 0;

    public static bool IsActive { get; private set; }
    public static int Phase { get; private set; }

    private static Coroutine _acceleration;

    private const float SpeedUpTime = 10; // In Minutes

    // Инициализация

    protected override void OnActive()
    {
        Stats.Load();

        Portal.Reset();

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


    private void OnDraged(float horizontal, float vertical)
    {
        Player.Movement.Drag(horizontal, vertical);
    }


    // Цикл игры

    public static void Launch()
    {
        if (IsActive == true) throw new Exception("Нельзя запустить игру, игра уже запущена");

        IsActive = true;

        Difficulty = 0.8f;
        Phase = 0;
        PassedTime = 0;

        Inputer.Swiped += Instance.OnSwiped;
        Inputer.Draged += Instance.OnDraged;

        Mode.DisableAllModes();

        Stats.Load();
        CameraSyncer.Reset();

        _acceleration = Instance.StartCoroutine( SpeedUp() );

        Player.Movement.enabled = true;
        Player.Camera.enabled = true;
    }

    public static void Stop(bool isNeedToDisactiveCamera = false)
    {
        if (IsActive == false) throw new Exception("Нельзя остановить игру, игра уже остановлена");

        Inputer.Swiped -= Instance.OnSwiped;
        Inputer.Draged -= Instance.OnDraged;

        IsActive = false;

        Stats.Save();
        Stats.OnUnperfectJump();

        Instance.StopCoroutine( _acceleration );

        //if (isNeedToDisactiveCamera)
        //Player.Camera.enabled = false;

        Debuger.Instance.OnGameEnd();
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

    public static void Regenerate()
    {
        if (Game.IsActive)
            Game.Stop();

        IsActive = true;

        Inputer.Swiped += Instance.OnSwiped;
        Inputer.Draged += Instance.OnDraged;

        Stats.Load();
        CameraSyncer.Reset();

        _acceleration = Instance.StartCoroutine(SpeedUp());

        Player.Movement.enabled = true;
        Player.Camera.enabled = true;

        Drone.Instance.Enable();
    }

    public void Update()
    {
        PassedTime += Time.deltaTime;
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

            DisableParachuteMode();

            Player.Movement.EnableVehicleControl();
            Player.Presenter.EnableVehicleMode();

            Builder.Instance.DeleteAllRegeneratePoints();
            Builder.Instance.Metroer.CreateRegeneratePoint();

            DisableHulkMode();
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

            DisableHulkMode();
        }

        public static void DisableCurveMode()
        {
            if (InCurveMode == false) return;

            InCurveMode = false;

            DisableParachuteMode();

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

            DisableParachuteMode();

            Instance.StopCoroutine(_doubleModeCoroutine);
        }

        private static Coroutine _doubleModeCoroutine;


        // Magnet Mode

        public static bool InMagnetMode { get; private set; }

        public static void EnableMagnetMode()
        {
            if (InMagnetMode == true) return;

            InMagnetMode = true;

            DisableParachuteMode();

            _magnetModeCoroutine = Instance.StartCoroutine( InvokeDelay(30, DisableMagnetMode) );
        }

        public static void DisableMagnetMode()
        {
            if (InMagnetMode == false) return;

            InMagnetMode = false;

            Instance.StopCoroutine( _magnetModeCoroutine );
        }

        private static Coroutine _magnetModeCoroutine;


        // Hulk Mode

        public static bool InHulkMode { get; private set; }

        public static void EnableHulkMode()
        {
            if (InHulkMode == true) return;

            InHulkMode = true;

            DisableParachuteMode();
            
            Player.Presenter.EnableHulkMode();

            _hulkModeCoroutine = Instance.StartCoroutine(InvokeDelay(30, DisableHulkMode));
        }

        public static void DisableHulkMode()
        {
            if (InHulkMode == false) return;

            InHulkMode = false;

            Player.Presenter.DisableHulkMode();

            Instance.StopCoroutine( _hulkModeCoroutine );
        }   

        private static Coroutine _hulkModeCoroutine;


        // xAxIxRx Mode

        public static bool InxAxIxRxMode { get; private set; }

        public static void EnablexAxIxRxMode()
        {
            if (InxAxIxRxMode == true) return;

            InxAxIxRxMode = true;

            DisableHulkMode();
            DisableParachuteMode();

            Player.Movement.EnablexAxIxRxControl();
            Player.Presenter.EnablexAxIxRxMode();

            Builder.Instance.CancelErLoopTimer();

            xAxIxRxer.Enable();
        }

        public static void DisablexAxIxRxMode()
        {
            if (InxAxIxRxMode == false) return;

            InxAxIxRxMode = false;

            Player.Movement.DisablexAxIxRxControl();
            Player.Presenter.DisablexAxIxRxMode();
            Player.Camera.DisablexAxIxRxMode();

            Builder.Instance.ContinueErLoopTimer();
        }


        // Parachute Mode

        public static bool InParachuteMode { get; private set; }

        public static void EnableParachuteMode()
        {
            if (InParachuteMode == true) return;

            InParachuteMode = true;

            Player.Camera.EnablexAxIxRxMode();
            Player.Presenter.EnableParachuteMode();
            Player.Movement.EnableParachuteControl();
        }

        public static void DisableParachuteMode()
        {
            if (InParachuteMode == false) return;

            InParachuteMode = false;

            Player.Camera.DisablexAxIxRxMode();
            Player.Presenter.DisableParachuteMode();
            Player.Movement.DisableParachuteControl();
        }


        // Invincibility Mode

        public static bool InInvincibilityMode { get; private set; }

        public static void EnableInvincibilityMode()
        {
            if (InInvincibilityMode == true)
                return;

            InInvincibilityMode = true;

            Builder.Instance.DisableAllObstacles();

            _invincibilityModeCoroutine = Instance.StartCoroutine(InvokeDelay(5, DisableInvincibilityMode));
        }

        public static void DisableInvincibilityMode()
        {
            if (InInvincibilityMode == false)
                return;

            InInvincibilityMode = false;

            Builder.Instance.EnableAllObstacles();

            Instance.StopCoroutine(_invincibilityModeCoroutine);
        }

        private static Coroutine _invincibilityModeCoroutine;


        // Other


        public static void DisableAllModes()
        {
            DisableInvincibilityMode();
            DisableParachuteMode();
            DisablexAxIxRxMode();
            DisableVehicleMode();
            DisableDoubleMode();
            DisableMagnetMode();
            DisableCurveMode();
            DisableHulkMode();
        }

        private static System.Collections.IEnumerator InvokeDelay(float duration, Action invokable)
        {
            yield return new WaitForSeconds(duration);

            invokable?.Invoke();
        }
    }
}
using System.Collections;
using UnityEngine;
using System;

public sealed class Game : SingleBehaviour<Game>
{
    [SerializeField] private float _levelHeight;
    [SerializeField] private GameObject _postProcessInstance;

    public static float levelHeight => Instance._levelHeight;
    public static float Difficulty => Mathf.Max(_difficulty, _acceleratedDifficulty);
    public static float PassedTime { get; private set; } = 0;

    public static bool InMiniGames => MiniGames.IsActive || MiniGames.IsReadyToActive;
    public static bool IsActive { get; private set; }
    public static int Phase { get; private set; }

    public static int MilkCollected { get; private set; }
    public static Action<int> MilkChanged;
    public static Action Started;

    private static Coroutine _acceleration;
    private static bool isNeedToLaunch;

    private const float SpeedUpTime = 5; // In Minutes
    private const float SpeedUpInMiniGamesTime = 2;

    private static float _difficulty;
    private static float _acceleratedDifficulty;

    private bool isRestarted;

    // Инициализация

    protected override void OnActive()
    {
        Instance._postProcessInstance.SetActive( Stats.TargetGraphics == GraphicPreset.High );
    }

    protected override void OnDisactive()
    {
        // Хеллоу, ворлд!
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

    public static void OnMilkCollected()
    {
        if (Game.IsActive == false) return;

        MilkCollected++;
        MilkChanged?.Invoke( MilkCollected );
    }

    public static void OnTurn()
    {
        Phase++;
    }

    public static void Launch()
    {
        if (IsActive == true) throw new Exception("Нельзя запустить игру, игра уже запущена");

        IsActive = true;

        _acceleratedDifficulty = 0;
        _difficulty = 0.8f;
        Phase = 0;
        PassedTime = 0;
        MilkCollected = 0;

        Inputer.Swiped += Instance.OnSwiped;
        Inputer.Draged += Instance.OnDraged;

        CameraSyncer.Reset();

        Player.Movement.enabled = true;
        Player.Camera.enabled = true;

        Started?.Invoke();

        if (MainMenu.IsOpened) MainMenu.Close();

        if (InMiniGames == false)
        {
            Mode.DisableAllModes();
            Builder.Instance.Initialize();
            
            _acceleration = Instance.StartCoroutine( SpeedUp() );
        }
        else
        {
            _acceleration = Instance.StartCoroutine( SpeedUpInMiniGames() );
        }
    }

    public static void Stop(bool isNeedToDisactiveCamera = false)
    {
        if (IsActive == false) throw new Exception("Нельзя остановить игру, игра уже остановлена");

        Inputer.Swiped = null;
        Inputer.Draged = null;

        IsActive = false;

        Stats.OnUnperfectJump();

        if (IsExist && _acceleration != null) Instance.StopCoroutine( _acceleration );

        if (GameUI.IsExist) GameUI.OpenDronePanel();

        if (Player.IsExist) Player.Camera.DisableTilerMode();
    }

    public static void Menu()
    {
        if (Game.IsActive)
            Game.Stop();

        MainMenu.Launch();
        MainMenu.ResetScene();
    }

    public static void Regenerate()
    {
        if (Game.IsActive)
            Game.Stop();

        IsActive = true;

        Inputer.Swiped += Instance.OnSwiped;
        Inputer.Draged += Instance.OnDraged;

        CameraSyncer.Reset();

        _acceleration = Instance.StartCoroutine(SpeedUp());

        Player.Movement.enabled = true;
        Player.Camera.enabled = true;

        Drone.Instance.Enable();
    }

    public static void Accelerate()
    {
        _acceleratedDifficulty = 1.5f;
    }

    public void Update()
    {
        if (IsActive) PassedTime += Time.deltaTime;

        if (Player.Movement.enabled) Player.Movement.GameUpdate();
        
        xAxIxRxer.GameUpdate();
    }

    private static IEnumerator SpeedUp()
    {
        while (Game.IsActive)
        {
            yield return new WaitForSeconds(10);

            _difficulty = Mathf.MoveTowards(Difficulty, 1.1f, 10 / (SpeedUpTime * 60) / (1.1f / 0.8f));
        }
    }


    private static IEnumerator SpeedUpInMiniGames()
    {
        while (Game.IsActive)
        {
            yield return new WaitForSeconds(10);

            _difficulty = Mathf.MoveTowards(Difficulty, 1.6f, 10 / (SpeedUpInMiniGamesTime * 60) / (1.1f / 0.8f));
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

            if (Player.IsExist == false) return;

            DisableParachuteMode();

            Player.Movement.EnableVehicleControl();
            Player.Presenter.EnableVehicleMode();

            DisableHulkMode();
        }

        public static void DisableVehicleMode()
        {
            if (InVehicleMode == false) return;

            InVehicleMode = false;

            if (Player.IsExist == false) return;

            Player.Movement.DisableVehicleControl();
            Player.Presenter.DisableVehicleMode();
        }


        // Curve Mode

        public static bool InCurveMode { get; private set; }

        public static void EnableCurveMode()
        {
            if (InCurveMode == true) return;

            InCurveMode = true;

            if (Player.IsExist == false) return;

            DisableParachuteMode();
            DisableHulkMode();

            Player.Movement.EnableCurveControl();
            Player.Camera.SetHeight(0);

        }

        public static void DisableCurveMode()
        {
            if (InCurveMode == false) return;

            InCurveMode = false;

            if (Player.IsExist == false) return;

            Player.Movement.DisableCurveControl();
            Player.Camera.SetHeight(0);
        }


        // Double Mode

        public static bool InDoubleMode { get; private set; }

        public static void EnableDoubleMode()
        {
            if (InDoubleMode == true) return;

            InDoubleMode = true;

            _doubleModeCoroutine = Instance.StartCoroutine( InvokeDelay( Double.UseTime , DisableDoubleMode) );
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

            _magnetModeCoroutine = Instance.StartCoroutine( InvokeDelay( Magnet.UseTime , DisableMagnetMode) );
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
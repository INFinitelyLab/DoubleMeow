using UnityEngine;
using System;

public sealed class Inputer : SingleBehaviour<Inputer>
{
    public static Action<Direction> Swiped;
    public static Action<float> Draged;


    private void OnEnable()
    {
        Swiper.Swiped += OnSwiped;
        Swiper.Draged += OnDraged;
    }


    private void OnDisable()
    {
        Swiper.Swiped -= OnSwiped;
        Swiper.Draged -= OnDraged;
    }


    private void OnSwiped(Direction direction) => Swiped?.Invoke(direction);

    private void OnDraged(float axis) => Draged?.Invoke(axis);


    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0)) Swiper.OnPressed(Input.mousePosition);
            else Swiper.OnMove(Input.mousePosition);
        }

        if (Input.GetKeyDown(KeyCode.W)) Swiped?.Invoke(Direction.Up);
        if (Input.GetKeyDown(KeyCode.S)) Swiped?.Invoke(Direction.Down);
        if (Input.GetKeyDown(KeyCode.A)) Swiped?.Invoke(Direction.Left);
        if (Input.GetKeyDown(KeyCode.D)) Swiped?.Invoke(Direction.Right);

        if (Input.GetKey(KeyCode.Q))
        {
            int value = UnityEngine.Random.Range(0, 4);

            switch(value)
            {
                case 0: Swiped?.Invoke(Direction.Up); break;
                case 1: Swiped?.Invoke(Direction.Down); break;
                case 2: Swiped?.Invoke(Direction.Left); break;
                case 3: Swiped?.Invoke(Direction.Right); break;
            }
        }

        //Debug
        if (Input.GetKeyDown(KeyCode.R)) Game.Restart();


        if (Input.GetKeyDown(KeyCode.T)) Application.targetFrameRate = Application.targetFrameRate == 20 ? 120 : 20;

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (Game.Mode.InCurveMode)
                Game.Mode.DisableCurveMode();
            else
                Game.Mode.EnableCurveMode();
        }
    }


    private static class Swiper
    {
        private static Resolution _screenResolution;
        private static Vector2 _lastPressedPosition2;
        private static Vector2 _pressedPosition;
        private static bool _isPressed;


        public static Action<Direction> Swiped;
        public static Action<float> Draged;


        public static void OnPressed(Vector2 screenPosition)
        {
            _screenResolution = new Resolution();
            _screenResolution.width = Screen.width;
            _screenResolution.height = Screen.height;

            _pressedPosition = screenPosition;

            _lastPressedPosition2 = new Vector2(Player.Movement.transform.position.x / 2.425f * _screenResolution.width / 2, 0);

            _isPressed = true;
        }


        public static void OnMove(Vector2 screenPosition)
        {
            Vector2 delta = screenPosition - _pressedPosition;

            Draged?.Invoke( Mathf.Clamp((screenPosition.x - (_pressedPosition.x - _lastPressedPosition2.x)) / _screenResolution.width * 3.25f, -1f, 1f) );

            //_lastPressedPosition.x = Mathf.Clamp(screenPosition.x - (_pressedPosition.x - _lastPressedPosition2.x), _screenResolution.width / -2, _screenResolution.width / 2);
            //_lastPressedPosition.y = Mathf.Clamp(screenPosition.y - (_pressedPosition.y - _lastPressedPosition2.y), _screenResolution.height / -2, _screenResolution.height / 2);

            if ( delta.magnitude < _screenResolution.width / 200 || _isPressed == false ) return;

            Direction direction = Mathf.Abs(delta.x) > Mathf.Abs(delta.y) ? (delta.x > 0 ? Direction.Right : Direction.Left) : (delta.y < 0 ? Direction.Down : Direction.Up);

            Swiped?.Invoke( direction );

            _isPressed = false;
        }
    }
}
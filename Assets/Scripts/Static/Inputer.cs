using UnityEngine;
using System;
using UnityEngine.UI;

public sealed class Inputer : SingleBehaviour<Inputer>
{
    public static Action<Direction> Swiped;
    public static Action<float> Draged;


    private void OnEnable()
    {
        Swiper.Swiped += OnSwiped;
    }


    private void OnDisable()
    {
        Swiper.Swiped -= OnSwiped;
    }


    private void OnSwiped(Direction direction) => Swiped?.Invoke(direction);


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


        if (Input.GetKey(KeyCode.A)) Draged?.Invoke(-10 * Time.deltaTime);
        if (Input.GetKey(KeyCode.D)) Draged?.Invoke( 10 * Time.deltaTime);

        //Debug
        if (Input.GetKeyDown(KeyCode.R)) Game.Restart();
    }


    private static class Swiper
    {
        private static Resolution _screenResolution;
        private static Vector2 _pressedPosition;
        private static bool _isPressed;

        public static Action<Direction> Swiped;


        public static void OnPressed(Vector2 screenPosition)
        {
            _screenResolution = Screen.currentResolution;

            _pressedPosition = screenPosition;

            _isPressed = true;
        }


        public static void OnMove(Vector2 screenPosition)
        {
            Vector2 delta = screenPosition - _pressedPosition;

            if( delta.magnitude < _screenResolution.width / 300 || _isPressed == false ) return;

            Direction direction = Mathf.Abs(delta.x) > Mathf.Abs(delta.y) ? (delta.x > 0 ? Direction.Right : Direction.Left) : (delta.y < 0 ? Direction.Down : Direction.Up);

            Swiped?.Invoke( direction );

            _isPressed = false;
        }
    }
}
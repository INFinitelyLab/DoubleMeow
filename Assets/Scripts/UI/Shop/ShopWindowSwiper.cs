using UnityEngine;

// =====[ Swiper between Shop Windows ]===== //
public class ShopWindowSwiper : MonoBehaviour
{
    private Vector2 _startPosition;
    private Vector2 _mousePosition;
    private Vector2 _resolution;

    private bool _isPressed;

    private bool _canSwipe => ModelPresenter.IsPressed == false;


    private Vector2 _delta => (_mousePosition - _startPosition) / _resolution;


    private void Awake()
    {
        _resolution = new Vector2( Screen.width, Screen.height );
    }


    private void Update()
    {
        if (_canSwipe == false) return;

        if (Input.GetMouseButtonDown(0))
        {
            _startPosition = Input.mousePosition;
            _isPressed = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isPressed = false;
        }

        if (_isPressed == false)
            return;

        _mousePosition = Input.mousePosition;

        if (_delta.x > 0.4f)
            Swipe(Direction.Left);
        else if (_delta.x < -0.4f)
            Swipe(Direction.Right);
    }


    private void Swipe( Direction direction )
    {
        int windowsCount = System.Enum.GetValues(typeof(ShopWindowType)).Length;
        int currentWindowIndex = (int)Shop.CurrentWindowType;

        currentWindowIndex += direction == Direction.Left ? -1 : 1;

        if (currentWindowIndex < 0 || currentWindowIndex > windowsCount)
            return;

        Shop.SelectWindow( (ShopWindowType)currentWindowIndex );

        _isPressed = false;
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class ModelPresenter : MonoBehaviour, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private Transform _model;
    [SerializeField] private float _sensitivity;

    private Vector2 _lastPosition;
    private Vector2 _currentPosition;
    private Vector2 _screenResolution;

    private static bool _isPressed;

    private Quaternion _velocity;

    public Vector2 Delta => new Vector2( _lastPosition.x - _currentPosition.x, _lastPosition.y - _currentPosition.y ) / _screenResolution;
    public static bool IsPressed => _isPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        _lastPosition = eventData.position;

        _isPressed = true;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (_isPressed == false)
            return;

        _currentPosition = eventData.position;

        _velocity = Quaternion.Euler(0, 0, Delta.x / Time.deltaTime * _sensitivity);

        _model.localRotation *= _velocity;

        _lastPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPressed = false;
    }

    public void OnReselected(InventoryItem item)
    {
        if (item is Skin)
        {
            _renderer.material = (item as Skin).Material;
        }
    }


    private void Awake()
    {
        _screenResolution = new Vector2( Screen.currentResolution.width, Screen.currentResolution.height );
    }

    private void OnEnable()
    {
        if (_model != null)
            _model.gameObject.SetActive(true);

        _model.rotation = Quaternion.Euler(600f, 0f, -145f);
        _velocity = Quaternion.identity;

        Shop.Reselected += OnReselected;
    }

    private void OnDisable()
    {
        if (_model != null)
            _model.gameObject.SetActive(false);

        Shop.Reselected -= OnReselected;
    }

    private void Update()
    {
        if (_isPressed == true)
            return;

        _velocity = Quaternion.Lerp( _velocity, Quaternion.identity, 3 * Time.deltaTime );

        _model.localRotation *= _velocity;
    }

}

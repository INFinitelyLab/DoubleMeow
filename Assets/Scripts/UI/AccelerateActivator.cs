using UnityEngine;
using UnityEngine.UI;


public class AccelerateActivator : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _transform;
    private Vector2 _startPosition;
    private Vector2 _targetPosition;
    private bool _isActivate;


    private void Start()
    {
        _image.color = Accelerator.Count > 0 ? Color.white : Color.gray;
        _startPosition = _transform.anchoredPosition;
        _targetPosition = _startPosition + Vector2.right * 300f;
        _transform.anchoredPosition = _targetPosition;
    }

    private void OnEnable()
    {
        Game.Started += Show;
    }

    private void OnDisable()
    {
        Game.Started -= Show;
    }

    private void Update()
    {
        _transform.anchoredPosition = Vector2.Lerp( _transform.anchoredPosition, _targetPosition, 10 * Time.deltaTime );
    }

    public void TryActivate()
    {
        if (_isActivate == true) return;

        if (Accelerator.TryUse())
        {
            _isActivate = true;

            Game.Accelerate();
            Hide();
        }
    }


    private void Show()
    {
        CancelInvoke();

        _targetPosition = _startPosition;

        Invoke(nameof(Hide), 5);
    }

    private void Hide()
    {
        CancelInvoke();

        _targetPosition = _startPosition + Vector2.right * 300;

        // HelloWorld
    }
}

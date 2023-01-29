using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BattlepassItemPresenter : MonoBehaviour
{
    [SerializeField] private float _speed = 10;

    private Image _image;
    private RectTransform _transform;


    private void Awake()
    {
        _image = GetComponent<Image>();
        _transform = GetComponent<RectTransform>();
    }


    public void Initialize(Vector3 position, Sprite icon)
    {
        CancelInvoke();

        _transform.position = position;

        _image.sprite = icon;
        _image.color = Color.white;

        Invoke(nameof(Disable), 5);
    }


    private void Update()
    {
        _transform.localPosition = Vector2.Lerp( _transform.localPosition, Vector2.zero, _speed * Time.deltaTime );
        _transform.localScale = Vector3.MoveTowards( _transform.localScale, Vector3.one * 3, Time.deltaTime );
    }


    private void Disable()
    {
        Destroy(gameObject);
    }
}

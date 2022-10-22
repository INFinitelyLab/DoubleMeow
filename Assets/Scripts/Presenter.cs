using UnityEngine;
using System.Collections;


public class Presenter : MonoBehaviour
{
    [SerializeField] private GameObject _ownVehicle;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _bendIntensive;
    [SerializeField] private float _bendSmoothTime;
    [SerializeField] private SkinnedMeshRenderer _mesh;

    private Transform _transform;
    private Quaternion _targetRotation;

    private float _moveSpeed = 1;

    private Vector3 _scale = Vector3.one * 0.55f;
    private float _rescaleTime;


    private void Awake()
    {
        Shader.SetGlobalFloat("_CurvatureIntensive", 0);

        _transform = transform;
    }


    public void OnJump() =>_animator.SetTrigger("Jump");

    public void OnBumped() => _animator.SetTrigger("Bump");

    public void OnGrounded(bool state) => _animator.SetBool("Grounded", state);


    public void OnRedirection(Direction direction, float intensive)
    {
        if (direction.IsHorizontal())
        {
            if (Game.Mode.InVehicleMode)
                _targetRotation = Quaternion.Euler(0, 0, direction.ToInt() * _bendIntensive * -intensive / 3);
            else
                 _targetRotation = Quaternion.Euler(0, direction.ToInt() * _bendIntensive * intensive, 0);

            _moveSpeed = 10;
        }
    }


    public void EnableVehicleMode()
    {
        _ownVehicle.SetActive(true);

        _animator.speed = 0;

        _bendSmoothTime /= 2f;

        Invoke(nameof(EnableCurvatization), 1f);
    }
   

    public void DisableVehicleMode()
    {
        _ownVehicle.SetActive(false);

        _animator.speed = 1;

        _bendSmoothTime *= 2f;
    }


    public void EnableCurvatization()
    {
        StartCoroutine(Curvatization(true, 0.5f));
    }


    public void DisableCurvatization()
    {
        StartCoroutine(Curvatization(false, 0.5f));
    }


    public void SetScale(float size, float duration)
    {
        _scale = Vector3.one * size;
        _rescaleTime = duration;
    }


    public void EnableCat()
    {
        _mesh.enabled = true;
    }


    public void DisableCat()
    {
        _mesh.enabled = false;
    }


    public void Update()
    {
        _transform.localRotation = Quaternion.Lerp( _transform.localRotation, _targetRotation, _moveSpeed * _bendSmoothTime * Time.deltaTime );

        _targetRotation = Quaternion.Lerp( _targetRotation, Quaternion.identity, _moveSpeed * _bendSmoothTime * Time.deltaTime );

        _moveSpeed = Mathf.MoveTowards( _moveSpeed, 1 , Time.deltaTime );

        //_transform.localScale = Vector3.MoveTowards( _transform.localScale, _scale, (_rescaleTime * Game.Difficulty ) * Time.deltaTime );
    }


    public IEnumerator Curvatization(bool isEnable, float duration)
    {
        float endValue = isEnable ? 1 : 0;
        float value = 1 - endValue;

        while (value != endValue)
        {
            value = Mathf.MoveTowards(value, endValue, duration * Time.fixedDeltaTime);

            Shader.SetGlobalFloat("_CurvatureIntensive", value);

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDisable()
    {
        Shader.SetGlobalFloat("_CurvatureIntensive", 0);
    }
}
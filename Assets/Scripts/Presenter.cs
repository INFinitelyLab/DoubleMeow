using UnityEngine;
using System.Collections;


public class Presenter : MonoBehaviour
{
    [SerializeField] private GameObject _ownVehicle;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _bendIntensive;
    [SerializeField] private float _bendSmoothTime;
    [SerializeField] private SkinnedMeshRenderer _mesh;
    [SerializeField] private Transform _head;

    private Transform _transform;
    private Quaternion _targetRotation = Quaternion.identity;

    private float _moveSpeed = 1;

    private Vector3 _scale = Vector3.one * 0.55f;
    private float _rescaleTime;

    public Transform Head => _head;


    private void Awake()
    {
        Shader.SetGlobalFloat("_CurvatureIntensive", 0);

        _transform = transform;
    }


    private void Start()
    {
        if( Stats.selectedSkin != "" ) _mesh.sharedMaterial = Skin.Find(Stats.selectedSkin).Material;
    }


    public void OnJump() =>_animator.SetTrigger("Jump");

    public void OnBumped() => _animator.SetTrigger("Bump");

    public void OnGrounded(bool state) => _animator.SetBool("Grounded", state);


    public void OnRedirection(Direction direction, float intensive)
    {
        if (direction.IsHorizontal())
        {
            if (Game.Mode.InVehicleMode)
                _targetRotation = Quaternion.Euler(0, 0, direction.ToInt() * _bendIntensive * -intensive / 2.5f);
            else
                 _targetRotation = Quaternion.Euler(0, direction.ToInt() * _bendIntensive * intensive, 0);

            _moveSpeed = 10;
        }
    }


    public void OnSlope(float gradus)
    {
        _targetRotation = Quaternion.Euler( gradus, _targetRotation.eulerAngles.y, _targetRotation.eulerAngles.z );
    }


    public void EnableCurveMode()
    {
        _animator.SetBool("Belly", true);
    }


    public void DisableCurveMode()
    {
        _animator.SetBool("Belly", false);
    }


    public void OnRedraged(float intensive)
    {
        _transform.localRotation = Quaternion.Euler( 0, 0, intensive );
    }


    public void EnableVehicleMode()
    {
        _ownVehicle.SetActive(true);

        _animator.SetBool("InVehicle", true);

        _bendSmoothTime /= 2f;

        Invoke(nameof(EnableCurvatization), 1f);
    }


    public void DisableVehicleMode()
    {
        _ownVehicle.SetActive(false);

        _animator.SetBool("InVehicle", false);

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
        if (Game.IsActive == false || Game.Mode.InCurveMode) return;
            
        _transform.localRotation = Quaternion.Lerp( _transform.localRotation, _targetRotation, _moveSpeed * _bendSmoothTime * Time.deltaTime );

        _targetRotation.x = Mathf.Lerp(_targetRotation.x, 0, _moveSpeed * _bendSmoothTime * Time.deltaTime * 1.5f);
        _targetRotation.y = Mathf.Lerp(_targetRotation.y, 0, _moveSpeed * _bendSmoothTime * Time.deltaTime);
        _targetRotation.z = Mathf.Lerp(_targetRotation.z, 0, _moveSpeed * _bendSmoothTime * Time.deltaTime);


        _moveSpeed = Mathf.MoveTowards( _moveSpeed, 1 , Time.deltaTime );

        _transform.localScale = Vector3.MoveTowards( _transform.localScale, _scale, (_rescaleTime * Game.Difficulty ) * Time.deltaTime );
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
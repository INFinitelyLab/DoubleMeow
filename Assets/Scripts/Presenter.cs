using UnityEngine;
using System.Collections;


public class Presenter : MonoBehaviour
{
    [SerializeField] private GameObject _ownParachute;
    [SerializeField] private GameObject _ownVehicle;
    [SerializeField] private GameObject _ownUFO;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _bendIntensive;
    [SerializeField] private float _bendSmoothTime;
    [SerializeField] private SkinnedMeshRenderer _mesh;
    [SerializeField] private Transform _head;
    [SerializeField] private AnimationCurve _upscaleCurve;
    
    [field:SerializeField] public Interpolation Interpolation { get; private set; }

    private Transform _transform;
    private Quaternion _targetRotation = Quaternion.identity;
    private Quaternion _rotation = Quaternion.identity;

    private float _moveSpeed = 1;


    private Vector3 _scale = Vector3.one * 0.7f;
    private Vector3 _startScale;
    private Vector3 _targetScale;
    private float _rescaleTime;

    public Transform Head => _head;

    public bool InHulkMode { get; private set; }


    private void Awake()
    {
        Shader.SetGlobalFloat("_CurvatureIntensive", 0);

        _transform = transform;
        _startScale = _transform.localScale;
        _targetScale = _transform.localScale;
    }


    private void Start()
    {
        if( Stats.selectedSkin != "" ) _mesh.sharedMaterial = Skin.Find(Stats.selectedSkin).Material;
    }


    public void OnRegenerate()
    {
        _animator.ResetTrigger("Bump");

        StartCoroutine(Blinking(5));
    }

    public void OnDrone()
    {
        _animator.SetBool("InDrone", true);
    }

    public void OnUndrone()
    {
        _animator.SetBool("InDrone", false);
    }


    public void OnJump() =>_animator.SetTrigger("Jump");

    public void OnBumped() => _animator.SetTrigger("Bump");

    public void OnGrounded(bool state) => _animator.SetBool("Grounded", state);


    public void OnRedirection(Direction direction, float intensive)
    {
        if (direction.IsHorizontal())
        {
            if (Game.Mode.InVehicleMode || Game.Mode.InxAxIxRxMode || Game.Mode.InParachuteMode)
                _targetRotation = Quaternion.Euler(0, 0, Mathf.Clamp(direction.ToInt() * _bendIntensive * -intensive / 2.5f, -25f, 25f));
            else
                 _targetRotation = Quaternion.Euler(0, direction.ToInt() * _bendIntensive * intensive * (Game.Mode.InHulkMode? 0.7f : 1f), 0);

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


    public void EnableHulkMode()
    {
        StartCoroutine(Upscale(1));
    }

    public void DisableHulkMode()
    {
        // We are a programmers, my friend! 
    }


    public void OnRedraged(float intensive)
    {
        _transform.localRotation = Quaternion.Euler( 0, 0, intensive );
    }


    public void EnablexAxIxRxMode()
    {
        _bendSmoothTime /= 7f;

        Invoke(nameof(_EnableUFO), 0.2f);
    }

    public void DisablexAxIxRxMode()
    {
        _bendSmoothTime *= 7f;

        Invoke(nameof(_DisableUFO), 0.2f);
    }


    public void EnableParachuteMode()
    {
        _bendSmoothTime /= 5f;

        Invoke(nameof(_EnableParachute), 0.2f);
    }

    public void DisableParachuteMode()
    {
        _bendSmoothTime *= 5f;

        Invoke(nameof(_DisableParachute), 0.2f);
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


    public void CreateGhostVehicle()
    {
        GameObject vehicle = Instantiate(_ownVehicle);

        vehicle.SetActive(true);

        vehicle.transform.rotation = _ownVehicle.transform.rotation;
        vehicle.transform.position = _ownVehicle.transform.position;
        vehicle.transform.localScale = _ownVehicle.transform.lossyScale;
    }


    public void EnableCurvatization()
    {
        StartCoroutine(Curvatization(true, 0.5f));
    }

    public void DisableCurvatization()
    {
        StartCoroutine(Curvatization(false, 0.5f));
    }


    public void SetScale(bool upscale, float duration)
    {
        _scale = upscale? _targetScale : Vector3.one * 0.04f;
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
            
        _rotation = Quaternion.Lerp( _transform.localRotation, _targetRotation, _moveSpeed * _bendSmoothTime * Time.deltaTime );

        _rotation.y = Mathf.Clamp( _rotation.y, -0.6f, 0.6f );

        _transform.localRotation = _rotation;

        _targetRotation.x = Mathf.Lerp(_targetRotation.x, 0, _moveSpeed * _bendSmoothTime * Time.deltaTime * 1.5f);
        _targetRotation.y = Mathf.Lerp(_targetRotation.y, 0, _moveSpeed * _bendSmoothTime * Time.deltaTime);
        _targetRotation.z = Mathf.Lerp(_targetRotation.z, 0, _moveSpeed * _bendSmoothTime * Time.deltaTime);

        _moveSpeed = Mathf.MoveTowards( _moveSpeed, 1 , Time.deltaTime );

        _transform.localScale = Vector3.MoveTowards(_transform.localScale, _scale, (_rescaleTime * Game.Difficulty) * Time.deltaTime);
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

    public IEnumerator Upscale( float targetScale )
    {
        InHulkMode = true;

        float time = 0;

        Player.Camera.EnableHulkMode();

        while( time < _upscaleCurve.keys[ _upscaleCurve.keys.Length -1 ].time )
        {
            _transform.localScale = Vector3.one * (_upscaleCurve.Evaluate(time) * _startScale.x + _startScale.x);
            _scale = _transform.localScale;
            _targetScale = _scale;

            time += Time.deltaTime;

            yield return null;
        }

        yield return new WaitUntil(() => Game.Mode.InHulkMode == false);

        time = _upscaleCurve.keys[_upscaleCurve.keys.Length - 1].time;

        while (time > 0)
        {
            _transform.localScale = Vector3.one * (_upscaleCurve.Evaluate(time) * _startScale.x + _startScale.x);
            _scale = _transform.localScale;
            _targetScale = _scale;

            time -= Time.deltaTime;

            yield return null;
        }

        Player.Camera.DisableHulkMode();

        InHulkMode = false;
    }

    public IEnumerator Blinking(float duration)
    {
        float time = 0;

        while(time < duration)
        {
            time += Time.deltaTime;

            _transform.localScale = time % 0.3f < 0.15f ? _scale : Vector3.zero;

            yield return null;
        }

        _transform.localScale = _scale;
    }


    private void OnDisable()
    {
        Shader.SetGlobalFloat("_CurvatureIntensive", 0);
    }


    private void _EnableUFO()
    {
        _ownUFO.SetActive( true );

        _animator.SetBool("InVehicle", true);
    }

    private void _DisableUFO()
    {
        _ownUFO.SetActive(false);

        _animator.SetBool("InVehicle", false);
    }


    private void _EnableParachute()
    {
        _ownParachute.SetActive(true);

        _ownParachute.GetComponent<Animator>().SetTrigger("Open");

        _animator.SetBool("InParachute", true);
    }

    private void _DisableParachute()
    {
        _ownParachute.SetActive(false);

        _ownParachute.GetComponent<Animator>().SetTrigger("Close");

        _animator.SetBool("InParachute", false);
    }
}
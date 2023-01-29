using UnityEngine;

public class ModelCasePresenter : MonoBehaviour
{
    [SerializeField] private Transform _model;
    [SerializeField] private float _rotateSpeed = 180f;

    private void OnEnable()
    {
        if (_model != null)
            _model.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (_model != null)
            _model.gameObject.SetActive(false);
    }

    private void Update()
    {
        _model.rotation *= Quaternion.Euler(0, _rotateSpeed * Time.deltaTime, 0 );
    }
}

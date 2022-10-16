using UnityEngine;

public class DecorationBuilding : MonoBehaviour
{
    [SerializeField] private float _width;
    [SerializeField] private Transform _billboardMounting;
    [SerializeField] private Billboard _billboardPrefabLeft;
    [SerializeField] private Billboard _billboardPrefabRight;
    [SerializeField] private Material[] _materials;
    [SerializeField] private Renderer _renderer;

    public float Width => _width;
    public Transform BillboardMounting => _billboardMounting;

    private Vector3 _targetPosition;
    private Transform _transform;


    public void SetMaterial(Material material)
    {
        if (material != null)
            _renderer.material = material;
    }


    private void Start()
    {
        //SetMaterial( _materials. );

        if (Random.Range(0, 3) == 1)
        {
            /*Billboard board = Instantiate(transform.localScale.x > 0 ?_billboardPrefabLeft : _billboardPrefabRight);

            board.transform.parent = _billboardMounting;
            board.transform.localPosition = Vector3.zero;*/
        }

        _transform = transform;
    }
}
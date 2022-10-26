using UnityEngine;
using System.Collections;

public class Metroer : MonoBehaviour
{
    [SerializeField] private MetroTrigger _metroTrigger;
    [SerializeField] private RegenTrigger _regenTrigger;
    [SerializeField] private Building _metroStartPrefab;
    [SerializeField] private Building _metroPrefab;
    [SerializeField] private MetroVehicle _vehicle;
    [SerializeField] private float _targetMetroDistance;
    [SerializeField] private float _vehicleXOffset;
    [SerializeField] private float _distanceBetweenVehicles;
    [SerializeField] private float _generationDistance;

    private Vector3 _lineSpeeds;

    private float _distance;
    private float _startPositionZ;
    private float _lastVehiclePositionZ;

    private Transform _player;

    private int _nextVehicleGenerationSwipeDistance;
    private RoadLine _vehicleLine;

    public System.Action<float> EndGenerate;


    public void Spawn(float fromZ, System.Action<float> OnEndMethod)
    {
        _player = Player.Movement.transform;

        CreateNewBuilding(_metroStartPrefab, Vector3.forward * fromZ);

        EndGenerate = OnEndMethod;

        _distance = 0;

        _lineSpeeds.x = Random.Range(0.5f, 1f);
        _lineSpeeds.y = Random.Range(0.5f, 1f);
        _lineSpeeds.z = Random.Range(0.5f, 1f);

        _lastVehiclePositionZ = fromZ + 10;
        _regenTrigger.Triggered += Regenerate;
        _startPositionZ = fromZ;
    }


    private Building CreateNewBuilding(Building origin, Vector3 position)
    {
        Building metro = Instantiate(origin, position, Quaternion.identity, transform);

        return metro;
    }


    private MetroVehicle CreateNewBuilding(MetroVehicle origin, Vector3 position, RoadLine line)
    {
        MetroVehicle metro = Instantiate(origin, transform);

        metro.Body.gameObject.SetActive(false);

        StartCoroutine(WaitForActive(metro.Body.gameObject));

        metro.transform.localPosition = position;
        metro.SetSpeedMultiplier( line == RoadLine.Mercury? _lineSpeeds.x : line == RoadLine.Venus? _lineSpeeds.y : _lineSpeeds.z );

        return metro;
    }


    public void Regenerate()
    {
        float targetDistance = _player.position.z - _startPositionZ + _generationDistance;

        Building metro = null;

        if (_distance + 5 < _targetMetroDistance)
        {
            while( _lastVehiclePositionZ < targetDistance + _startPositionZ )
            {

                _nextVehicleGenerationSwipeDistance--;

                if (_nextVehicleGenerationSwipeDistance <= 0)
                {
                    _nextVehicleGenerationSwipeDistance = 1;

                    int swipeID = _vehicleLine == RoadLine.Mercury? 0 : 1;

                    swipeID += _vehicleLine == RoadLine.Earth ? 0 : 2;

                    if (swipeID == 3)
                        _vehicleLine.TrySurfRandom();
                    else
                        _vehicleLine.TrySurf( swipeID == 1? Direction.Left : Direction.Right );

                    int rand = Random.Range(0, 4);

                    if (_vehicleLine != RoadLine.Mercury && rand != 0) CreateNewBuilding(_vehicle, new Vector3(-1 * _vehicleXOffset, 0, Random.Range(0f,0.75f) + _lastVehiclePositionZ + _distanceBetweenVehicles), RoadLine.Mercury);
                    if (_vehicleLine != RoadLine.Venus && rand != 1) CreateNewBuilding(_vehicle, new Vector3(0, 0, Random.Range(0f, 0.75f) + _lastVehiclePositionZ + _distanceBetweenVehicles), RoadLine.Venus);
                    if (_vehicleLine != RoadLine.Earth && rand != 2) CreateNewBuilding(_vehicle, new Vector3(1 * _vehicleXOffset, 0, Random.Range(0f, 0.75f) + _lastVehiclePositionZ + _distanceBetweenVehicles), RoadLine.Venus);
                }
                
                _lastVehiclePositionZ += _distanceBetweenVehicles;
            }
        }    

        while( _distance < targetDistance )
        {
            metro = CreateNewBuilding(_metroPrefab, Vector3.forward * (_distance + _startPositionZ));

            _distance += _metroPrefab.EndPoint.transform.localPosition.z;
        }


        _regenTrigger.MoveTo( Player.Movement.transform.localPosition.z + 2 * Game.Difficulty);

        if (_distance >= _targetMetroDistance )
        {
            MetroTrigger mT = Instantiate(_metroTrigger, metro.transform);

            mT.transform.localPosition = metro.EndPoint.localPosition;

            _regenTrigger.Triggered -= Regenerate;

            EndGenerate?.Invoke(_distance + _startPositionZ);
        }
    }


    private IEnumerator WaitForActive(GameObject toActive)
    {
        yield return new WaitForFixedUpdate();

        bool isActive = false;

        while(isActive == false)
        {
            if (Mathf.Abs(_player.position.z - toActive.transform.position.z) < _generationDistance)
            {
                toActive.SetActive(true);
                isActive = true;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
using UnityEngine;
using System.Collections;

// Это очень важный комментарий!
public class Drone : SingleBehaviour<Drone>
{
    [SerializeField] private Transform[] _rotors;

    private Coroutine _routine;

    public bool IsEnabled { get; private set; }


    private void Update()
    {
        foreach(Transform rotor in _rotors)
        {
            rotor.rotation *= Quaternion.Euler(0, 5 * 360f * Time.deltaTime, 0);
        }
    }


    public void Enable()
    {
        transform.rotation = Player.Movement.transform.rotation;

        Vector3 playerPosition = Player.Movement.transform.position;
        Vector3 closestPoint = Vector3.zero;
        float closestDistance = 999;

        foreach (RegeneratePoint point in FindObjectsOfType<RegeneratePoint>())
        {
            if (Vector3.Distance(point.transform.position, playerPosition) < closestDistance)
            {
                closestPoint = point.transform.position;
                closestDistance = Vector3.Distance(point.transform.position, playerPosition);
            }
        }

        _routine = StartCoroutine( Translate(playerPosition, closestPoint) );
    }

    public void Disable()
    {
        StopCoroutine(_routine);
    }


    private IEnumerator Translate(Vector3 playerPosition, Vector3 point)
    {
        IsEnabled = true;

        Player.Movement.Controller.enabled = false;
        Player.Movement.enabled = false;
        Player.Camera.enabled = false;

        Vector3 normalizedPlayerPosition = playerPosition;
        normalizedPlayerPosition.y = Mathf.Max(playerPosition.y, Player.Movement.Height);

        Vector3 startPosition = normalizedPlayerPosition + ( Player.Movement.transform.rotation * Vector3.back * 3 );
        Vector3 endPosition = point + ( Player.Movement.transform.rotation * Vector3.back * 3 );
        Vector3 offset = new Vector3(0, 1f, 0);
        Vector3 lowerOffset = new Vector3(0,0.5f,0);


        yield return StartCoroutine( MoveTo( startPosition + offset, normalizedPlayerPosition + offset, 1.5f ) );    // Подлет


        yield return StartCoroutine( MoveTo( normalizedPlayerPosition + offset, playerPosition + lowerOffset, 1 ));  // Снижение

        Player.Movement.transform.parent = transform;
        Player.Presenter.OnDrone();

        yield return StartCoroutine( MoveTo(playerPosition + lowerOffset, normalizedPlayerPosition + offset, 1 ));  // Возвышение

        Player.Camera.transform.parent = transform;

        yield return StartCoroutine( MoveTo(normalizedPlayerPosition + offset, point + offset, 1 ) );    // Транспортировка

        Player.Camera.transform.parent = null;

        Player.Presenter.OnUndrone();
        yield return StartCoroutine( MoveTo( point + offset, point + lowerOffset, 1 )); // Снижение
        Player.Movement.transform.parent = null;

        IsEnabled = false;

        Player.Camera.enabled = true;
        Player.Movement.enabled = true;
        Player.Movement.Controller.enabled = true;

        Game.Mode.EnableInvincibilityMode();
        Player.Presenter.OnRegenerate();

        yield return StartCoroutine( MoveTo( point + lowerOffset, point + offset, 1 )); // Возвышение
        
        yield return StartCoroutine( MoveTo( point + offset, endPosition + offset, 1 ) );   // Улет
    }


    private IEnumerator MoveTo(Vector3 from, Vector3 to, float duration)
    {
        float progress = 0;

        while( progress < 1 )
        {
            progress += Time.deltaTime / duration;

            transform.position = Vector3.Lerp( from, to, progress );

            yield return null;
        }
    }
}
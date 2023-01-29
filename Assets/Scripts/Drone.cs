using UnityEngine;
using System.Collections;

// Это очень важный комментарий!
public class Drone : SingleBehaviour<Drone>
{
    [SerializeField] private Transform[] _rotors;

    private Coroutine _routine;
    private RegeneratePoint _point;

    public bool IsEnabled { get; private set; }
    public bool IsTranslateFromMode { get; private set; }


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

        Player.Movement.transform.position = new Vector3( Player.Movement.transform.position.x, Mathf.Max(Player.Movement.transform.position.y, -2 + Player.Movement.Height), Player.Movement.transform.position.z );

        Vector3 playerPosition = Player.Movement.transform.position;
        Vector3 point = Player.Movement.TurnPositionWithoutRotation;

        Player.Movement.enabled = false;
        Player.Camera.enabled = false;

        point.y = 0;

        _routine = StartCoroutine( Translate(playerPosition, point ) );
    }

    public void Disable()
    {
        StopCoroutine(_routine);
    }


    private IEnumerator Translate(Vector3 playerPosition, Vector3 point)
    {
        IsEnabled = true;

        IsTranslateFromMode = Game.Mode.InVehicleMode || Game.Mode.InCurveMode;

        bool isMetroMode = Game.Mode.InVehicleMode;
        bool isCurveMode = Game.Mode.InCurveMode;
        bool isTilerMode = Player.Camera.InTilerMode;

        Vector3 normalizedPlayerPosition = playerPosition;
        normalizedPlayerPosition.y = Mathf.Max(playerPosition.y, Player.Movement.Height);

        Vector3 offset = new Vector3(0, 2f, 0);
        Vector3 lowerOffset = new Vector3(0,0.65f,0);

        Fog.Instance.gameObject.SetActive(true);

        StartCoroutine( MoveTo( normalizedPlayerPosition + offset, playerPosition + lowerOffset, 1.75f ));  // Снижение

        yield return new WaitForSeconds(0.75f);

        if (isMetroMode)
        {
            Player.Presenter.CreateGhostVehicle();
            Shader.SetGlobalFloat("_CurvatureIntensive", 0);
        }

        Game.Mode.DisableAllModes();
        Player.Presenter.OnDrone();


        yield return new WaitForSeconds(1f);

        Player.Movement.transform.parent = transform;
        

        Player.Presenter.Fog();

        yield return StartCoroutine( MoveTo(playerPosition + lowerOffset, normalizedPlayerPosition + offset, 1.5f));  // Возвышение


        Player.Camera.transform.parent = transform;

        transform.position = point + offset;

        Builder.Instance.DestroyAllTrash(true);

        if (Builder.Instance.Metroer.IsEnabled) Builder.Instance.Metroer.Disable();
        if (Builder.Instance.Curver.IsEnabled) Builder.Instance.Curver.Disable();
        if (Builder.Instance.Tiler.IsEnabled) Builder.Instance.Tiler.Disable();
        if (Builder.Instance.Retroer.IsEnabled) Builder.Instance.Retroer.Disable();

        if ( isMetroMode )
        {
            Player.Presenter.DisableCurvatization();
            Player.Camera.DisableMetroMode(false);
        }
        else if( isCurveMode )
        {
            Builder.Instance.Curver.Disable();
            Player.Presenter.DisableCurveMode();
        }

        Builder.Instance.CreateBuildingFrom(point + Player.Movement.transform.rotation * Vector3.back * 5, Player.Movement.transform.rotation);


        Player.Presenter.Unfog();

        Player.Camera.OnRegenerate(point);
        Player.Camera.transform.parent = null;

        StartCoroutine( MoveTo( point + offset, point + lowerOffset, 1.75f)); // Снижение

        yield return new WaitForSeconds(0.75f);

        Player.Presenter.OnUndrone();

        yield return new WaitForSeconds(1f);

        Player.Movement.transform.parent = null;

        Game.Mode.EnableInvincibilityMode();
        Player.Movement.OnRegenerate();

        Player.Camera.enabled = true;
        Player.Movement.enabled = true;

        Player.Presenter.OnRegenerate();

        IsTranslateFromMode = false;
        IsEnabled = false;

        yield return StartCoroutine( MoveTo( point + lowerOffset, point + offset, 1.5f)); // Возвышение
    }


    private IEnumerator MoveTo(Vector3 from, Vector3 to, float duration, bool byDistance = false)
    {
        float progress = 0;
        float distance = Vector3.Distance(from, to);

        while( progress < 1 )
        {
            progress += Time.deltaTime / duration / (byDistance? (distance / 20) : 1);

            transform.position = Vector3.Lerp( from, to, Mathf.Sin(progress / 2 * Mathf.PI) );

            yield return null;
        }

        transform.position = to;
    }
}
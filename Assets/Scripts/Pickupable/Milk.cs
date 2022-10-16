using UnityEngine;

public class Milk : Pickup
{
    private Transform _transform;

    public override bool IsCanPlace => true;

    private void Start()
    {
        _transform = transform;

        _transform.localRotation = Quaternion.Euler( 0f, _transform.position.z * 36 , 0f );
    }

    protected override void OnPickup()
    {
        Stats.IncreaseCoin( Game.Mode.InDoubleMode? 2 : 1 );        

        Achievements.Unlock("1234");
    }

    private void FixedUpdate()
    {
        _transform.localRotation *= Quaternion.Euler(0f, Time.fixedDeltaTime * 360, 0f);

        if (Game.Mode.InMagnetMode == false)
            return;

        float distance = Vector3.Distance(_transform.position, Player.Movement.transform.position);

        if ( distance < 1f ) _transform.position = Vector3.MoveTowards( _transform.position, Player.Movement.transform.position, (Time.fixedDeltaTime * 30) / (distance * 5) );
    }
}
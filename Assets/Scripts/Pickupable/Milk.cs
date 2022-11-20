using UnityEngine;
using System.Linq;
using System.Collections;

public class Milk : Pickup
{
    private Transform _transform;
    private Transform _body;

    public override bool IsCanPlace => true;

    private void Start()
    {
        _transform = transform;
        _body = _transform.GetChild(0);

        _body.localRotation = Quaternion.Euler( 0, _transform.position.z * 36, 0 );
     }

    protected override void OnPickup()
    {
        Stats.IncreaseCoin( Game.Mode.InDoubleMode? 2 : 1 );

        ParticleManager.Play( "MilkFlash", Player.Presenter.Head.position );

        Achievements.Unlock("1234");
    }

    private void Update()
    {
        _body.localRotation *= Quaternion.Euler(0, Time.deltaTime * 360f, 0f);

        if (Game.Mode.InMagnetMode == false)
            return;

        float distance = Vector3.Distance(_transform.position, Player.Movement.transform.position);

        if ( distance < 2f) _transform.position = Vector3.MoveTowards( _transform.position, Player.Movement.transform.position, (Time.deltaTime * 30) / (distance * 5) );
    }
}
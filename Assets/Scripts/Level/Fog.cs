using UnityEngine;

public class Fog : SingleBehaviour<Fog>
{
    private Transform _target;
    private Transform _transform;
    private float _height;

    protected override void OnActive()
    {
        _target = Player.Movement.transform;
        _transform = transform;
        _height = _transform.position.y;

        StartCoroutine(Move());
    }


    protected override void OnDisactive()
    {
        StopAllCoroutines();
    }


    private System.Collections.IEnumerator Move()
    {
        while(isActiveAndEnabled)
        {
            Vector3 position = _target.position;

            position.y = _height;

            _transform.position = position;

            yield return new WaitForSeconds(0.1f);
        }
    }
}
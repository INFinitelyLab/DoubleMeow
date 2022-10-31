using UnityEngine;

public class ParticleManager : SingleBehaviour<ParticleManager>
{
    private static ParticleSystem[] _particles;


    private void Awake()
    {
        _particles = GetComponentsInChildren<ParticleSystem>();
    }


    public static void Play(string particleName, Vector3 position)
    {
        foreach( ParticleSystem particle in _particles )
        {
            if( particle.name == particleName )
            {
                particle.transform.position = position;

                particle.Play();

                return;
            }
        }
    }
}

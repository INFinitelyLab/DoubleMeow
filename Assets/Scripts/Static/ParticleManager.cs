using UnityEngine;

public class ParticleManager : SingleBehaviour<ParticleManager>
{
    private static ParticleSystem[] _particlesSystems;


    private void Awake()
    {
        _particlesSystems = GetComponentsInChildren<ParticleSystem>();
    }


    public static void Play(string particleName, Vector3 position)
    {
        foreach( ParticleSystem particle in _particlesSystems)
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

using UnityEngine;
using System;

public class Player : SingleBehaviour<Player>, IGroundeable
{
    [SerializeField] private Presenter _presenter;
    [SerializeField] private Movement _movement;
    [SerializeField] private Detector _detector;

    public static Detector Detector
    {
        get
        {
            if (Instance._detector == null)
                throw new Exception("Модуль обнаружения препятствий не назначен!");

            return Instance._detector;
        }
    }

    public static Presenter Presenter
    {
        get
        {
            if (Instance._presenter == null)
                throw new Exception("Модуль представления не назначен!");

            return Instance._presenter;
        }
    }

    public static Movement Movement
    {
        get
        {
            if (Instance._movement == null)
                throw new Exception("Модуль перемещения не назначен!");

            return Instance._movement;
        }
    }


    public static Action Jumped;

    public static Action<Direction, float> Redirected;


    protected override void OnActive()
    {
        Movement.Redirected += Presenter.OnRedirection;
        Movement.Jumped += Presenter.OnJump;

        Detector.Bumped += OnBumped;
        Movement.Jumped += Presenter.OnJump;
        Movement.Grounded += Presenter.OnGrounded;
    }


    protected override void OnDisactive()
    {
        Movement.Redirected -= Presenter.OnRedirection;
        Movement.Jumped -= Presenter.OnJump;

        Detector.Bumped -= OnBumped;
        Movement.Jumped -= Presenter.OnJump;
        Movement.Grounded -= Presenter.OnGrounded;
    }


    private void OnBumped(Direction direction)
    {
        if( direction == Direction.Up )
        {
            if (Game.IsActive)
            {
                Stats.OnDeath();

                Game.Stop();
                Presenter.OnBumped();
            }
        }
    }
}
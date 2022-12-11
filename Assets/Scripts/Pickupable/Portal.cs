using UnityEngine;

public class Portal : Placeable
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent<Player>(out var player))
        {
            if (IsAlreadyExist || IsCanPlace)
                StartPlayerTransition( Mathf.Clamp((Quaternion.Inverse(Player.Movement.transform.rotation) * transform.position).z - (Quaternion.Inverse(Player.Movement.transform.rotation) * Player.Movement.transform.position).z, 0.1f, 3) );
            else
                EndPlayerTransition();
        }

        Player.Presenter.EnableCat();
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            if (IsAlreadyTransite) Player.Presenter.DisableCat();
        }
    }


    // Static


    public static bool IsCanPlace => State == PortalState.None;
    public static bool IsAlreadyExist => State == PortalState.Exist;
    public static bool IsAlreadyTransite => State == PortalState.Transite;
    public static bool IsWaitingForSecondPortal => State == PortalState.WaitingForSecondPortal;

    private static PortalState State = PortalState.None;
    private static Vector3 SecondPortalPosition = Vector3.zero;


    public static void TakeSecondPortalPosition(Vector3 position)
    {
        if( IsWaitingForSecondPortal )
        {
            SecondPortalPosition = position;

            State = PortalState.Exist;
        }

    }


    public static void Initialize(Vector3 position)
    {
        if (State == PortalState.WaitingForSecondPortal)
        {
            TakeSecondPortalPosition(position);
        }
        else
        {
            State = PortalState.WaitingForSecondPortal;
        }
    }


    public static void StartPlayerTransition(float distanceToPortal)
    {
        if (IsAlreadyTransite == true) throw new System.Exception("Нельзя телепортировать игрока, игрок УЖЕ телепортируется!");

        State = PortalState.Transite;

        Player.Movement.EnablePortalControl(SecondPortalPosition, distanceToPortal);
    }


    public static void EndPlayerTransition()
    {
        if( IsAlreadyTransite == false ) throw new System.Exception("Нельзя оставить телепортацию игрока, игрок НЕ телепортируется!");

        State = PortalState.None;

        Player.Movement.DisablePortalControl();
    }


    public static void Reset()
    {
        State = PortalState.None;
        SecondPortalPosition = Vector3.zero;
    }
}


public enum PortalState
{
    WaitingForSecondPortal,
    Exist,
    Transite,
    None
}
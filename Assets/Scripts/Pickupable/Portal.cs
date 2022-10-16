using UnityEngine;

public class Portal : Placeable
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent<Player>(out var player))
        {
            if (IsAlreadyExist)
                StartPlayerTransition( Mathf.Clamp(-Player.Movement.transform.position.z + transform.position.z, 0.1f, 3) );
            else
                EndPlayerTransition();
        }
    }


    // Static


    public static bool IsAlreadyExist => State == PortalState.Exist;
    public static bool IsAlreadyTransite => State == PortalState.Transite;
    public static bool IsWaitingForSecondPortal => State == PortalState.WaitingForSecondPortal;

    private static PortalState State;
    private static Vector3 SecondPortalPosition;


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
        else if (State == PortalState.None)
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
using UnityEngine;

public interface IMove
{
    void SetVelocity( float VelocityVector );
    void StopMovement();
    void SetJumpInput( bool pressed);
    void SetIsGrounded(bool grounded);
}

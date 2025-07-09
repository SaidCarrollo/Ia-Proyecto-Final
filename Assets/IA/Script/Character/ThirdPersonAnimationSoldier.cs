using UnityEngine;
using UnityEngine.AI;

public class ThirdPersonAnimationSoldier : ThirdPersonAnimationBase
{
    private static readonly int ForwardHash = Animator.StringToHash("Forward");
     
    private void Awake()
    {
        LoadComponent();
    }
    public override void LoadComponent()
    {
        base.LoadComponent();
    }

    public override void Mover(Vector3 direccion)
    {
        Vector3 velocity = agent.velocity;
        float speed = velocity.magnitude;

        if (speed < 0.05f)
        {
            animator.SetFloat(ForwardHash, 0f);
            return;
        }

        // Dirección local del movimiento
        Vector3 localVelocity = transform.InverseTransformDirection(velocity.normalized);

        float forward = 0f;


        forward = Mathf.Clamp01(localVelocity.z);

        animator.SetFloat(ForwardHash, forward, 0.1f, Time.deltaTime);

    }
     

     
    
}
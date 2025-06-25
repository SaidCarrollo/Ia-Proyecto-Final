using UnityEngine;
using UnityEngine.AI;

public  class ThirdPersonAnimationBase : MonoBehaviour
{
    [Header("Componentes Base")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected NavMeshAgent agent;
    /// <summary>
    /// Mueve al personaje en una dirección específica.
    /// </summary>
    /// <param name="direccion">Dirección de movimiento.</param>
    public virtual void LoadComponent()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    /// <summary>
    /// Mueve al personaje en una dirección específica.
    /// </summary>
    /// <param name="direccion">Dirección de movimiento.</param>
    public virtual void Mover(Vector3 direccion)
    { }

    /// <summary>
    /// Ejecuta la animación de muerte.
    /// </summary>
    public virtual void Dead()
    { }
}
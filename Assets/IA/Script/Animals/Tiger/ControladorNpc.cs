using UnityEngine;
using UnityEngine.AI; // Necesario para usar NavMeshAgent

// Asegura que el GameObject tenga los componentes necesarios para que el script funcione.
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class ControladorNPC : MonoBehaviour
{
    // Referencias a los componentes
    private NavMeshAgent agent;
    private Animator animator;

    // Start se llama antes del primer frame
    void Start()
    {
        // Obtener las referencias a los componentes en este GameObject
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update se llama una vez por frame
    void Update()
    {
        // 1. Calcular la velocidad actual del agente
        // Usamos la magnitud de la velocidad para obtener un valor numérico simple (un float).
        float velocidad = agent.velocity.magnitude;

        // 2. Enviar esa velocidad al Animator
        // "Speed" es el nombre del parámetro que crearemos en el Animator.
        animator.SetFloat("Speed", velocidad);
    }

    /// <summary>
    /// Esta función pública puede ser llamada desde otro script para iniciar el ataque.
    /// </summary>
    public void IniciarAtaque()
    {
        // Activa el "Trigger" en el Animator llamado "Attack".
        // Los Triggers son ideales para eventos que ocurren una sola vez.
        animator.SetTrigger("Attack");
    }
}
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class DeerAnimationController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Parámetros Animator")]
    public string paramState = "State"; // Parámetro para correr
    public string paramVert = "Vert";   // Parámetro para caminar

    [Header("Configuración")]
    public float maxSpeed = 5f;           // Velocidad máxima configurada externamente
    [Range(0.1f, 3f)]
    public float animationSpeed = 1f;    // Velocidad de reproducción de animaciones
    float vertValue;
    float stateValue;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float speed = agent.speed; // velocidad objetivo del agente, no la magnitud de la velocidad actual
        float halfSpeed = maxSpeed * 0.5f;

        // Calcular valor para Vert (caminar): 1 cuando speed == halfSpeed, 0 en 0 y maxSpeed
        // Para hacerlo suave, vamos a hacer que Vert sea 1 en halfSpeed, y baje a 0 en 0 y maxSpeed (curva triangular)
        
        if (speed <= halfSpeed)
        {
            vertValue = Mathf.Lerp(vertValue, 1, Time.deltaTime * speed); // baja de 1 a 0
            stateValue = Mathf.Lerp(stateValue,0, Time.deltaTime * speed);
        }
        else
        {
            vertValue = Mathf.Lerp(vertValue, 1, Time.deltaTime * speed); // baja de 1 a 0
            stateValue = vertValue;
        }
        vertValue = Mathf.Clamp01(vertValue);

        // Calcular valor para State (correr): 0 cuando speed <= halfSpeed, 1 cuando speed == maxSpeed
         
        stateValue = Mathf.Clamp01(stateValue);

        animator.SetFloat(paramVert, vertValue);
        animator.SetFloat(paramState, stateValue);

        animator.speed = animationSpeed;
    }
}

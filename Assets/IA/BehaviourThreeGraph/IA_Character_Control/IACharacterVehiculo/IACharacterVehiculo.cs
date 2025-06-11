using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class IACharacterVehiculo : IACharacterControl 
{
    protected CalculateDiffuse _CalculateDiffuse;
    protected float speedRotation = 0; // Considera inicializarla, ej. en LoadComponent

    [Header("Configuración de Huida")]
    [SerializeField] protected float fleeMultiplier = 2f;

    public float RangeWander;
    Vector3 positionWander;
    float FrameRate = 0;
    float Rate = 4;

    // Nuevas propiedades para gestionar el estado de huida
    public bool IsCurrentlyFleeing { get; private set; } = false;
    public Vector3 CurrentFleeDestination { get; private set; }

    public override void LoadComponent()
    {
        base.LoadComponent();
        positionWander = RandoWander(transform.position, RangeWander);
        _CalculateDiffuse = GetComponent<CalculateDiffuse>();
        // 'agent' debe ser inicializado aquí o en base.LoadComponent()
        // Ejemplo: if (agent == null) agent = GetComponent<NavMeshAgent>();
        // 'speedRotation' podría necesitar un valor inicial si se usa antes de LookRotationCollider
        if (AIEye == null) Debug.LogError("AIEye no está asignado en " + gameObject.name);

    }
    public virtual void LookEnemy()
    {
        if (AIEye.ViewEnemy == null) return;
        Vector3 dir = (AIEye.ViewEnemy.transform.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        rot.x = 0;
        rot.z = 0;
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 50);
    }
    public virtual void LookPosition(Vector3 position)
    {
        Vector3 dir = (position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        rot.x = 0;
        rot.z = 0;

        float currentSpeedRotation = (speedRotation > 0) ? speedRotation : 10f; // Valor por defecto si no se ha seteado
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * currentSpeedRotation);
    }
    public virtual void LookRotationCollider()
    {
        if (_CalculateDiffuse != null && _CalculateDiffuse.Collider)
        {
            speedRotation = _CalculateDiffuse.speedRotation;
            Vector3 posNormal = _CalculateDiffuse.hit.point + _CalculateDiffuse.hit.normal * 2;
            LookPosition(posNormal);
        }
    }
    public virtual void MoveToPosition(Vector3 pos)
    {
        if (agent != null && agent.isOnNavMesh) // Buena práctica verificar
        {
            agent.SetDestination(pos);
        }
    }
    public virtual void MoveToEnemy()
    {
        if (AIEye.ViewEnemy == null) return;
        MoveToPosition(AIEye.ViewEnemy.transform.position);
    }
    public virtual void MoveToAllied()
    {
        if (AIEye.ViewAllie == null) return;
        MoveToPosition(AIEye.ViewAllie.transform.position);
    }
    public virtual void MoveToEvadEnemy()
    {
        if (AIEye.ViewEnemy == null) return;
        Vector3 dir = (transform.position - AIEye.ViewEnemy.transform.position).normalized;
        Vector3 newPosition = transform.position + dir * 5f;
        MoveToPosition(newPosition);
    }

    Vector3 RandoWander(Vector3 position, float range)
    {
        Vector3 randomDirection = Random.insideUnitSphere * range;
        randomDirection.y = 0; // Mantener en plano horizontal

        Vector3 targetPosition = position + randomDirection;

        NavMeshHit hit;
        for (int i = 0; i < 30; i++)
        {
            if (NavMesh.SamplePosition(targetPosition, out hit, range, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        // Si falla, regresa la posición original
        return position;
    }

    // Método existente, se mantiene igual
    public virtual void FleeRandomDirection()
    {
        if (AIEye.ViewEnemy == null && !IsCurrentlyFleeing) // Si ya está huyendo, podría no necesitar un enemigo visible para continuar
        {
            // Si no hay enemigo y no está huyendo, podría huir a un punto aleatorio sin referencia enemiga
            // o simplemente no hacer nada si la huida siempre es gatillada por un enemigo.
            // Por ahora, mantenemos la dependencia del enemigo para generar la dirección inicial.
            // Si quieres que huya incluso sin enemigo visible (ej. por estar herido),
            // esta lógica necesitaría ajustarse o que FleeRandomDirection tome un target opcional.
            if (AIEye.ViewEnemy == null) return; // Mantenemos la lógica original por ahora
        }


        Vector3 fleePositionBase;
        if (AIEye.ViewEnemy != null)
        {
            // Generar posición aleatoria con mayor rango que el wander normal
            Vector3 fleePosition = RandoWander(transform.position, RangeWander * fleeMultiplier);

            // Calcular dirección opuesta al enemigo combinada con dirección aleatoria
            Vector3 enemyDirection = (transform.position - AIEye.ViewEnemy.transform.position).normalized;
            Vector3 randomDirection = (fleePosition - transform.position).normalized;

            // Mezclar ambas direcciones para mayor efectividad
            Vector3 finalDirection = (enemyDirection + randomDirection).normalized;
            fleePositionBase = transform.position + finalDirection * (RangeWander * fleeMultiplier);
        }
        else // Si no hay enemigo visible pero se decidió huir (ej. por baja salud)
        {
            // Huir en una dirección aleatoria general
            fleePositionBase = RandoWander(transform.position, RangeWander * fleeMultiplier);
        }


        MoveToPosition(fleePositionBase);
    }

    public virtual void MoveToWander()
    {
        if (AIEye.ViewEnemy != null) return;

        float distance = (transform.position - positionWander).magnitude;

        if (distance < 2)
        {
            positionWander = RandoWander(transform.position, RangeWander);
        }

        if (FrameRate > Rate)
        {
            FrameRate = 0;
            positionWander = RandoWander(transform.position, RangeWander);
        }
        FrameRate += Time.deltaTime;

        MoveToPosition(positionWander);
    }

    public virtual void InitiateFleeState()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        IsCurrentlyFleeing = true;
        FleeRandomDirection(); // Esto ya llama a MoveToPosition
        CurrentFleeDestination = agent.destination; // Guardamos el destino establecido por FleeRandomDirection
        LookPosition(CurrentFleeDestination);
        // Opcionalmente, podrías aumentar la velocidad del agente aquí
        // agent.speed = originalSpeed * fleeSpeedMultiplierFactor;
    }

    public virtual void ConcludeFleeState()
    {
        if (agent == null) return;

        IsCurrentlyFleeing = false;
        if (agent.isOnNavMesh && agent.hasPath) // Solo resetea si tiene un camino activo
        {
            agent.ResetPath();
        }

    }

    public virtual bool HasReachedFleeDestination()
    {
        if (agent == null || !agent.isOnNavMesh || !IsCurrentlyFleeing) // Si no está huyendo o no hay agente, no ha llegado
        {
            return false;
        }
        // Si no tiene ruta y está en estado de huida, podría considerarse que llegó o que la ruta falló.
        // Si no hay ruta pendiente y la distancia restante es menor o igual a la stoppingDistance.
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // A veces, si no hay velocidad, remainingDistance puede ser > stoppingDistance pero igual no se mueve.
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true; // Consideramos que llegó si no tiene ruta o no se mueve.
                }
            }
        }
        return false;
    }
}
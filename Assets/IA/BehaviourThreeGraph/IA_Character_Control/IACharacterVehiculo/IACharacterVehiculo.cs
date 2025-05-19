using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IACharacterVehiculo : IACharacterControl
{
    protected CalculateDiffuse _CalculateDiffuse;
    protected float speedRotation = 0;

    [Header("Configuraci�n de Huida")]
    [SerializeField] protected float fleeMultiplier = 2f;

    public float RangeWander;
    Vector3 positionWander;
    float FrameRate = 0;
    float Rate = 4;
    public override void LoadComponent()
    {
        base.LoadComponent();
        positionWander = RandoWander(transform.position, RangeWander);
        _CalculateDiffuse = GetComponent<CalculateDiffuse>();
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
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * speedRotation);
    }
    public virtual void LookRotationCollider()
    {

        if (_CalculateDiffuse.Collider)
        {
            speedRotation = _CalculateDiffuse.speedRotation;

            Vector3 posNormal = _CalculateDiffuse.hit.point + _CalculateDiffuse.hit.normal * 2;

            LookPosition(posNormal);
        }
    }


    public virtual void MoveToPosition(Vector3 pos)
    {
        agent.SetDestination(pos);
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
        Vector3 randP = Random.insideUnitSphere * range;
        randP.y = transform.position.y;
        return position + randP;
    }
    public virtual void FleeRandomDirection()
    {
        if (AIEye.ViewEnemy == null) return;

        // Generar posici�n aleatoria con mayor rango que el wander normal
        Vector3 fleePosition = RandoWander(transform.position, RangeWander * fleeMultiplier);

        // Calcular direcci�n opuesta al enemigo combinada con direcci�n aleatoria
        Vector3 enemyDirection = (transform.position - AIEye.ViewEnemy.transform.position).normalized;
        Vector3 randomDirection = (fleePosition - transform.position).normalized;

        // Mezclar ambas direcciones para mayor efectividad
        Vector3 finalDirection = (enemyDirection + randomDirection).normalized;
        Vector3 targetPosition = transform.position + finalDirection * (RangeWander * fleeMultiplier);

        MoveToPosition(targetPosition);
    }
    public virtual void MoveToWander()
    {
        if (AIEye.ViewEnemy != null) return;

        float distance = (transform.position - positionWander).magnitude;

        if(distance<2)
        {
            positionWander = RandoWander(transform.position, RangeWander);
        }

        if(FrameRate>Rate)
        {
            FrameRate = 0;
            positionWander = RandoWander(transform.position, RangeWander);
        }
        FrameRate += Time.deltaTime;


        MoveToPosition(positionWander);
    }
}

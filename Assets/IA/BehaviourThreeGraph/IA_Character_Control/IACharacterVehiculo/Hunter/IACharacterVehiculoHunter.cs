using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class IACharacterVehiculoHunter : IACharacterVehiculo
{
    // --- MÉTODOS Y VARIABLES RESTAURADOS DEL SCRIPT ORIGINAL ---
    Vector3 normales = Vector3.zero;
    public bool ISDrawGizmos = false;
    ThirdPersonAnimationSoldier TPS;
    
    // --- LÓGICA DEL CEREBRO Y REFERENCIA A LAS ACCIONES (DE LA REFACTORIZACIÓN) ---
    private IACharacterActionsHunter actions;

    void Awake()
    {
        ISDrawGizmos = true;
        // Llamamos a LoadComponent desde Start para asegurar el orden de ejecución.
        this.LoadComponent();
        
    }

    public override void LoadComponent()
    {
        base.LoadComponent();
        TPS = GetComponent<ThirdPersonAnimationSoldier>();
    }
    public override void MoveToWander()
    {
         
        base.MoveToWander();
        Vector3 dir = (positionWander - transform.position).normalized;
        TPS.Mover(dir);

    }
    public override void MoveToEnemy()
    {
        if (AIEye.ViewEnemy == null) return;
        Vector3 dir = (AIEye.ViewEnemy.transform.position - transform.position).normalized;
        TPS.Mover(dir);
    }




    public void MoveToStrategy()
    {
        if (AIEye.ViewEnemy == null) return;
        Vector3 dir = Vector3.zero;
        normales = ColliderWall();
        if (normales != Vector3.zero)
            dir = normales;
        else
        {
            dir = (transform.position - AIEye.ViewEnemy.transform.position).normalized;
        }
        Vector3 newPosition = transform.position + dir * 2;
        MoveToPosition(newPosition);
    }

    Vector3 ColliderWall()
    {
        normales = Vector3.zero;
        Ray[] arrayRay = new Ray[3];
        arrayRay[0] = new Ray(health.AimOffset.position, health.AimOffset.right);
        arrayRay[1] = new Ray(health.AimOffset.position, -health.AimOffset.forward);
        arrayRay[2] = new Ray(health.AimOffset.position, -health.AimOffset.right);
        for (int i = 0; i < 2; i++)
        {
            RaycastHit hit;
            // Usamos la capa de oclusión definida en el AIEye para las paredes
            if (Physics.Raycast(arrayRay[i], out hit, 3, AIEye.mainDataView.occlusionlayers))
            {
                normales += hit.normal;
            }
        }
        return normales;
    }

    private void OnDrawGizmos()
    {
        // Solo dibujar si el componente del ojo (AIEye) está disponible
        if (!ISDrawGizmos || AIEye == null) return;

        Ray[] arrayRay = new Ray[3];
        arrayRay[0] = new Ray(health.AimOffset.position, health.AimOffset.right);
        arrayRay[1] = new Ray(health.AimOffset.position, -health.AimOffset.forward);
        arrayRay[2] = new Ray(health.AimOffset.position, -health.AimOffset.right);
        for (int i = 0; i < arrayRay.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(arrayRay[i], out hit, 3, AIEye.mainDataView.occlusionlayers))
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.blue;
            }
            Gizmos.DrawLine(arrayRay[i].origin, arrayRay[i].origin + arrayRay[i].direction * 3f);
            Gizmos.DrawSphere(arrayRay[i].origin + arrayRay[i].direction * 3f, 0.2f);
        }

        Gizmos.color = Color.yellow;
        if (normales != Vector3.zero)
        {
            Gizmos.DrawLine(health.AimOffset.position, health.AimOffset.position + normales * 2f);
            Gizmos.DrawSphere(health.AimOffset.position + normales * 2f, 0.5f);
        }
    }
}
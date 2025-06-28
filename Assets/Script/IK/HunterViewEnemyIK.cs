using UnityEngine;

public class HunterViewEnemyIK : MonoBehaviour
{
    IKMarine IK;
    IAEyeHunterShootAttack eye;
    // La variable 'health' no se usa, pero la dejamos por si la necesitas luego
    // healthHunter health; 
    public Transform aim;
    Vector3 storeposition;
    public float lenght;

    void Start()
    {
        IK = GetComponent<IKMarine>();
        eye = GetComponent<IAEyeHunterShootAttack>();
        // health = GetComponent<healthHunter>(); // Descomenta si lo necesitas

        if (aim != null)
        {
            storeposition = aim.localPosition;
        }
        else
        {
            Debug.LogError("¡El Transform 'aim' por defecto no está asignado en el Inspector!", this.gameObject);
        }
    }

    void Update()
    {
        if (eye.ViewEnemy != null)
        {
            // Añadimos una comprobación extra para asegurarnos de que el AimOffset existe
            if (eye.ViewEnemy.AimOffset != null)
            {
                IK.target = eye.ViewEnemy.AimOffset;
                // MENSAJE DE ÉXITO: Si ves esto, el apuntado al enemigo funciona.
              //  Debug.Log("<color=green>Apuntando al enemigo: " + eye.ViewEnemy.name + "</color>", this.gameObject);
            }
            else
            {
                // MENSAJE DE ERROR: El enemigo fue detectado, pero le falta el AimOffset.
               // Debug.LogWarning("<color=orange>Enemigo " + eye.ViewEnemy.name + " detectado, pero su 'AimOffset' no está asignado.</color>", this.gameObject);
                IK.target = aim; // Volvemos al apuntado por defecto para evitar errores.
            }
        }
        else
        {
            IK.target = aim;
            // MENSAJE DE ESTADO: Si solo ves esto, el problema está en la detección.
           // Debug.Log("<color=red>No se detecta ningún enemigo. Usando apuntado por defecto.</color>", this.gameObject);
        }
    }
}

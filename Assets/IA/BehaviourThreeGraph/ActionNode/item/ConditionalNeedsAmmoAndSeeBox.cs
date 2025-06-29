using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("MyAI/Conditional")]
[TaskDescription("Revisa si la IA necesita munición y puede ver un ítem de tipo 'Balas'.")]
public class ConditionalNeedsAmmoAndSeeBox : Conditional
{
    private IACharacterVehiculo _IACharacterVehiculo;
    private IAEyeBase AIEye;
    private WeaponsManager _weaponsManager;

    public override void OnStart()
    {
        _IACharacterVehiculo = GetComponent<IACharacterVehiculo>();
        // El WeaponsManager debe estar en el mismo GameObject que la IA
        _weaponsManager = GetComponent<WeaponsManager>();

        if (_IACharacterVehiculo != null)
        {
            AIEye = _IACharacterVehiculo.AIEye as IAEyeBase;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (AIEye == null || _weaponsManager == null || _weaponsManager.currentWeaponBase == null || AIEye.ViewItem == null || _IACharacterVehiculo.health.IsDead)
        {
            return TaskStatus.Failure;
        }

        WeaponBase currentWeapon = _weaponsManager.currentWeaponBase;

        // Condición 1: ¿Necesita la IA munición? (si la reserva no está llena)
        bool needsAmmo = currentWeapon._cartridge < currentWeapon._Maxcartridge;

        if (needsAmmo)
        {
            // Condición 2: ¿El ítem visible es una caja de balas?
            if (AIEye.ViewItem.itemType == ItemType.Balas)
            {
                return TaskStatus.Success;
            }
        }

        return TaskStatus.Failure;
    }
}
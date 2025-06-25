using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { PISTOL, SHOTGUN, ASSAULT_RIFLE, MINIGUN, SNIPER_RIFLE, LAUNCH, FLAME, NONE };

[System.Serializable]
public class ParticleFlash
{
    public ParticleSystem particle;
    public int CountEmitter = 1;
    public ParticleFlash() { }
}

[System.Serializable]
public class MuzzleFlashWeamon
{
    public ParticleFlash bullet = new ParticleFlash();
    public List<ParticleFlash> particles = new List<ParticleFlash>();
    public GameObject root; //
    public Light flash;
    public float minSpeed = 0.01f;
    public float maxSpeed = 0.5f;
    public float minIntensity = 2;
    public float maxIntensity = 5;

    public MuzzleFlashWeamon() { }

    public void Play()
    {
        if (bullet.particle != null)
        {
            bullet.particle.Emit(bullet.CountEmitter);
        }
        foreach (var item in particles)
        {
            item.particle.Emit(item.CountEmitter);
        }
        if (flash != null)
            flash.intensity = 1;
    }

    public void Stop()
    {
        if (flash != null)
            flash.intensity = 0;
        if (bullet != null)
        {
            bullet.particle.Emit(0);
        }
        foreach (var item in particles)
        {
            item.particle.Emit(0);
            item.particle.Stop();
        }
    }

    public void LookAtPosition(Vector3 pos)
    {
        if (root != null)
        {
            root.transform.LookAt(pos);
        }
    }

    public void ResetLookAt()
    {
        if (root != null)
        {
            root.transform.rotation = Quaternion.identity;
        }
    }
}

public class WeaponBase : MonoBehaviour
{
    [Header("Muzzle Flash Weamon")]
    public MuzzleFlashWeamon _MuzzleFlashWeamon = new MuzzleFlashWeamon(); //

    [Header("Gun Attributes")]
    public string weaponName;
    public WeaponType weaponType;
    public int damage = 10;

    [Header("Weapon Amount")]
    public int _cartridge = 0;
    public int _Maxcartridge = 0;
    public int _countbullet = 0;
    public int _MaxbulletTocartridge = 0;

    protected bool canShoot = true;
    protected float FrameRate = 0;

    [Header("Rate")]
    public float Rate = 1;

    public Health ownerHealth;

    public virtual void LoadComponent()
    {
        _countbullet = _MaxbulletTocartridge; //
        ownerHealth = GetComponentInParent<Health>();
    }

    public virtual void Shoot(LayerMask enemyLayer)
    {
        if (!canShoot) return;

        if (Time.time < FrameRate) return;
        FrameRate = Time.time + Rate;

        _MuzzleFlashWeamon.Play(); //

        Transform muzzleTransform = _MuzzleFlashWeamon.root != null ? _MuzzleFlashWeamon.root.transform : this.transform;
        Vector3 rayOrigin = muzzleTransform.position;
        Vector3 rayDirection = muzzleTransform.forward;
        Debug.DrawRay(rayOrigin, rayDirection * 100f, Color.cyan, 2.0f);
        RaycastHit hit;
        // Se lanza el Raycast desde la posición y dirección del cañón del arma.
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 100f, enemyLayer))
        {
            Debug.Log("<color=green>¡Impacto de Raycast! Objeto: " + hit.collider.name + "</color>", hit.collider.gameObject);
            _MuzzleFlashWeamon.LookAtPosition(hit.point);

            Health targetHealth = hit.collider.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.Damage(damage, ownerHealth); 
            }
            else
            {
                // --- AÑADE ESTA LÍNEA ---
                Debug.LogWarning("<color=yellow>El objeto impactado '" + hit.collider.name + "' no tiene un componente Health.</color>", hit.collider.gameObject);
            }
        }
        else
        {
            Debug.Log("<color=red>Disparo al aire. El Raycast no impactó con nada.</color>");
            // Si no se impacta nada, los efectos visuales apuntan hacia adelante en la dirección del disparo.
            _MuzzleFlashWeamon.LookAtPosition(rayOrigin + rayDirection * 100f);
        }

        _countbullet--;
        if (_countbullet <= 0)
        {
            Debug.Log("Out of ammo!");
        }
    }
}
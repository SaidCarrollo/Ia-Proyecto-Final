using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum WeaponType { PISTOL, SHOTGUN, ASSAULT_RIFLE, MINIGUN, SNIPER_RIFLE,  LAUNCH, FLAME,   NONE };
[System.Serializable]
public class ParticleFlash
{
    public ParticleSystem particle;
    public int CountEmitter = 1;
    public ParticleFlash()
    { }

}
[System.Serializable]
public class MuzzleFlashWeamon
{
    public ParticleFlash bullet = new ParticleFlash();
    public List<ParticleFlash> particles = new List<ParticleFlash>();
    public GameObject root;
    public Light flash;
    public float minSpeed = 0.01f;
    public float maxSpeed = 0.5f;
    public float minIntensity = 2;
    public float maxIntensity = 5;

    public MuzzleFlashWeamon()
    {


    }

    public void Play()
    {
        if (bullet.particle != null/*&& !bullet.particle.isPlaying*/)
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
        root.transform.LookAt(pos);
    }
    public void ResetLookAt()
    {
        root.transform.rotation = Quaternion.identity;
    }

}
public class WeaponBase : MonoBehaviour
{
    [Header("Muzzle Flash Weamon")]
    public MuzzleFlashWeamon _MuzzleFlashWeamon = new MuzzleFlashWeamon();

    [Header("Gun Attributes")]
    public string weaponName;
    public WeaponType weaponType;

    [Header("Weapon Amount")]
    public int _cartridge = 0;
    public int _Maxcartridge = 0;
    public int _countbullet = 0;
    public int _MaxbulletTocartridge = 0;



    protected bool canShoot = true;
    protected float FrameRate = 0;
    [Header("Rate")]
    public float Rate = 1;
    public virtual void LoadComponent()
    {
        _countbullet = _MaxbulletTocartridge;
    }

    public virtual void Shoot()
    {
        _MuzzleFlashWeamon.Play();
    }
}

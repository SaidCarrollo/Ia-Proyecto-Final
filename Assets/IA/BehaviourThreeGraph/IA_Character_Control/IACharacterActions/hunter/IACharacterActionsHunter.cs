using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IACharacterActionsHunter : IACharacterActions
{

    float FrameRate = 0;
    public float Rate=1;

    public override void LoadComponent()
    {
        base.LoadComponent();

    }
    public void Attack()
    {
        if(FrameRate>Rate)
        {
            FrameRate = 0;
            Debug.Log("Attack "+Time.time);
        }
        FrameRate += Time.deltaTime;


    }
    public void Shoot()
    {
        if (FrameRate > Rate)
        {
            FrameRate = 0;
            Debug.Log("Shoot " + Time.time);
        }
        FrameRate += Time.deltaTime;
        //Debug.Log("Shoot " + Time.time);
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IACharacterActions : IACharacterControl
{
    public override void LoadComponent()
    {
        base.LoadComponent();

    }
    public void ConsumeVisibleItem()
    {
        if (AIEye != null && AIEye.ViewItem != null)
        {
            // La lógica de consumir el ítem ahora vive aquí.
            AIEye.ViewItem.Consume(health);
        }
    }
}

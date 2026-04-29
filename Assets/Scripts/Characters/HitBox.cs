using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour, IInteractable
{
    [SerializeField] public Killable parentScript;

    public void TakeDamage(int dmg, string source)
    {
        if (parentScript == null) return;

        if (gameObject.tag == "WeakPoint")
        {
            dmg *= 2;
        }
        else if (gameObject.tag == "Shield")
        {
            dmg /= 2;
        }

        parentScript.HandleDamage(dmg, source, gameObject.name);

        if (parentScript == null)
        {
            Destroy(this);
        }
    }
}

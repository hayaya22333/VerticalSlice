using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    PlayerController player;

    void Start()
    {
        player = Locator.Instance.Player;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            player.EnterNPC(other);
        }

        if (other.CompareTag("Item"))
        {
            player.EnterItem(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            player.ExitNPC(other);
        }
        if (other.CompareTag("Item"))
        {
            player.ExitItem(other);
        }
    }
}

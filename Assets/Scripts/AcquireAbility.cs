using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquireAbility : MonoBehaviour
{
    private void Awake()
    {
        var player = FindObjectOfType<PlayerController>();
        if (player.specialAbilities.Contains(this.name))
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            var player = FindObjectOfType<PlayerController>();
            player.specialAbilities.Add(this.name);
            if (this.name == "JUMP")
            {
                player.specialAbility = PlayerController.SpecialAbility.JUMP;
            }
            Destroy(this.gameObject);
        }
    }
}

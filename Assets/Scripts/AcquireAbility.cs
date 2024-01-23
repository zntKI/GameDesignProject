using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquireAbility : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            FindObjectOfType<PlayerController>().specialAbilities.Add(this.name);
            Destroy(this.gameObject);
        }
    }
}

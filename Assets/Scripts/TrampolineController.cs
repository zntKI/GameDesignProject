using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineController : MonoBehaviour
{
    [SerializeField]
    private float bounceAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            FindObjectOfType<PlayerController>().Bounce(bounceAmount);
        }
    }
}

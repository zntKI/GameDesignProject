using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    private PlayerController player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        if (this.gameObject.name == "Start" && player.isAtNextLevel)
        {
            player.transform.position = new Vector3(this.transform.position.x + 2f, this.transform.position.y, player.transform.position.z);
        }
        else if (this.gameObject.name == "Finish" && !player.isAtNextLevel)
        {
            player.transform.position = new Vector3(this.transform.position.x - 2f, this.transform.position.y, player.transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            if (this.gameObject.name == "Finish")
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                player.isAtNextLevel = true;
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                player.isAtNextLevel = false;
            }
        }

    }
}

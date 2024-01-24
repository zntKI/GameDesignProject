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

                if (SceneManager.GetActiveScene().buildIndex + 1 == 2 && !player.moveFeels.Contains("MARIO"))
                {
                    player.moveFeels.Add("MARIO");
                }
                else if (SceneManager.GetActiveScene().buildIndex + 1 == 3 && !player.moveFeels.Contains("HOLLOW_KNIGHT"))
                {
                    player.moveFeels.Add("HOLLOW_KNIGHT");
                }

                FindObjectOfType<CameraFollowPlayer>().ChangeOffset(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                player.isAtNextLevel = false;
            }
        }

    }
}

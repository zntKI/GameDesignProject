using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollisionController : MonoBehaviour
{
    private PlayerController player;

    //Collectables
    [SerializeField]
    private GameObject coin;
    [SerializeField]
    private GameObject geo;
    [SerializeField]
    private GameObject strawberry;

    void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Spikes")
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            if (index == 4 || index == 5)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

                player.tempStrawberries.Clear();
            }

            var start = GameObject.Find("Start");
            this.transform.position = new Vector3(start.transform.position.x + 2f, start.transform.position.y, this.transform.position.z);

            FindObjectOfType<AbilitySwitchPanelController>().HidePanel();
        }
        else if (collision.gameObject.tag == "Revealable")
        {
            foreach (var contact in collision.contacts)
            {
                if (collision.gameObject.name == "MysteryBox")
                {
                    Sprite spr = collision.gameObject.GetComponent<SpriteRenderer>().sprite;
                    if (spr.name.Contains("3") && (contact.normal.y < 0f && (contact.normal.x < .1f && contact.normal.x > -.1f)))
                    {
                        GameObject obj;

                        if (player.specialAbility == PlayerController.SpecialAbility.JUMP)
                        {
                            obj = Instantiate(coin, collision.gameObject.transform.parent);
                        }
                        else
                        {
                            obj = Instantiate(geo, collision.gameObject.transform.parent);
                        }
                        obj.transform.position = collision.gameObject.transform.position + Vector3.up;

                        collision.gameObject.name = "MysteryBoxUnactive";
                    }
                    else if (spr.name.Contains("0") && (contact.normal.y < 0f && (contact.normal.x < .1f && contact.normal.x > -.1f))
                        && player.specialAbility == PlayerController.SpecialAbility.JUMP)
                    {
                        GameObject obj = Instantiate(coin);
                        obj.transform.position = collision.gameObject.transform.position + Vector3.up;

                        collision.gameObject.name = "MysteryBoxUnactive";
                    }
                    else if (spr.name.Contains("1") && (contact.normal.y < 0f && (contact.normal.x < .1f && contact.normal.x > -.1f))
                        && player.specialAbility == PlayerController.SpecialAbility.DOUBLE_JUMP)
                    {
                        GameObject obj = Instantiate(geo);
                        obj.transform.position = collision.gameObject.transform.position + Vector3.up;

                        collision.gameObject.name = "MysteryBoxUnactive";
                    }
                    else if (spr.name.Contains("2") && player.isDashing)
                    {
                        GameObject obj = Instantiate(strawberry);
                        obj.transform.position = collision.gameObject.transform.position - new Vector3(contact.normal.x, contact.normal.y, 0) * 0.2f;

                        player.isDashing = false;
                        player.Bounce(10f, new Vector2(1, 0));

                        collision.gameObject.name = "MysteryBoxUnactive";

                        Destroy(collision.gameObject);
                    }
                }
                else if (collision.gameObject.name == "Goomba")
                {
                    if (contact.normal.y > 0f)
                    {
                        player.Bounce(10f, Vector2.up);
                        GameObject obj = Instantiate(coin);
                        obj.transform.position = collision.gameObject.transform.position + Vector3.down * 0.5f;
                        Destroy(collision.gameObject);
                    }
                    else
                    {
                        var start = GameObject.Find("Start");
                        this.transform.position = new Vector3(start.transform.position.x + 2f, start.transform.position.y, this.transform.position.z);
                    }
                }
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Collectable")
        {
            player.IncreaseScore(collision.gameObject.name);
            Destroy(collision.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AcquireAbility : MonoBehaviour
{
    private void Awake()
    {
        //var player = FindObjectOfType<PlayerController>();
        //if (player.specialAbilities.Contains(this.name))
        //{
        //    Destroy(this.gameObject);
        //}
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

                var obj = Instantiate(new GameObject().AddComponent<TextMeshPro>());
                obj.text =
                    "You acquired the 'Jump' special ability!";
                obj.fontSize = 5;
                obj.alignment = TextAlignmentOptions.Center;

                obj.transform.position = this.gameObject.transform.position + Vector3.up * 2;
            }
            else if (this.name == "DOUBLE_JUMP")
            {
                var obj = Instantiate(new GameObject().AddComponent<TextMeshPro>());
                obj.text =
                    "You acquired the 'Wall jump' special ability!\n" +
                    "You can toggle through the special abilities\n" +
                    "by pressing the 'left' or 'right' arrow keys.\n" +
                    "Play around to see what happens when you do that!";
                obj.fontSize = 5;
                obj.alignment = TextAlignmentOptions.Center;

                obj.transform.position = this.gameObject.transform.position + Vector3.up * 2;
            }
            else if (this.name == "DASH")
            {
                var obj = Instantiate(new GameObject().AddComponent<TextMeshPro>());
                obj.text =
                    $"You acquired the 'Dash' special ability!";
                obj.fontSize = 5;
                obj.alignment = TextAlignmentOptions.Center;

                obj.transform.position = this.gameObject.transform.position + Vector3.up * 2;
            }
            else if (this.name == "WALL_JUMP")
            {
                var obj = Instantiate(new GameObject().AddComponent<TextMeshPro>());
                obj.text =
                    $"You acquired the 'Wall jump' special ability!";
                obj.fontSize = 5;
                obj.alignment = TextAlignmentOptions.Center;

                obj.transform.position = this.gameObject.transform.position + Vector3.up * 2;
            }
            Destroy(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalScoreDisplayer : MonoBehaviour
{
    private PlayerController player;

    // Start is called before the first frame update
    void Awake()
    {
        var playerObj = GameObject.Find("Player");
        player = playerObj.GetComponent<PlayerController>();

        var rectTransform = this.gameObject.GetComponent<RectTransform>();
        for (int i = 0; i < rectTransform.childCount; i++)
        {
            Transform child = rectTransform.GetChild(i);
            if (child.name == "Score")
            {
                continue;
            }
            else
            {
                var text = child.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
                if (child.name == "Coins")
                {
                    text.text = $"x   {player.coinsNum}";
                }
                else if (child.name == "Geos")
                {
                    text.text = $"x   {player.geoNum}";
                }
                else if (child.name == "Strawberries")
                {
                    text.text = $"x   {player.strawberries.Count}";
                }
            }
        }

        Destroy(playerObj);
    }
}

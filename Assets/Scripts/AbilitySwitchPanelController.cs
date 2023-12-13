using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySwitchPanelController : MonoBehaviour
{
    [SerializeField]
    private RectTransform abilitySwitchPanel;
    [SerializeField]
    private TimeController timeController;

    private bool isShown = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UpdatePanel();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            HidePanel();
        }
    }

    private void UpdatePanel()
    {
        if (!isShown)
        {
            abilitySwitchPanel.anchoredPosition = new Vector2(abilitySwitchPanel.anchoredPosition.x, abilitySwitchPanel.anchoredPosition.y - abilitySwitchPanel.rect.height);
            isShown = true;

            timeController.StartSlowMotion();
        }
        else
        {
            //Traverse trough the abilities
        }
    }

    private void HidePanel()
    {
        if (isShown)
        {
            abilitySwitchPanel.anchoredPosition = new Vector2(abilitySwitchPanel.anchoredPosition.x, abilitySwitchPanel.anchoredPosition.y + abilitySwitchPanel.rect.height);
            isShown = false;

            //Update the ability state

            timeController.StopSlowMotion();
        }
    }
}

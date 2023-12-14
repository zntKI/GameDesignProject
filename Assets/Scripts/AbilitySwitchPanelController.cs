using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySwitchPanelController : MonoBehaviour
{
    [SerializeField]
    private Image abilitySwitchPanel;

    private RectTransform panelRectTransform;
    private TimeController timeController;

    private bool isShown = false;

    private struct CurrentAbilities 
    {
        internal string currentMoveFeel;
        internal string currentJumpFeel;
        internal string currentWallJumpFeel;
        internal string currentWallSlideFeel;
        internal string currentSpecialAbility;

        internal CurrentAbilities(string currentMoveFeel, string currentJumpFeel, string currentWallJumpFeel, string currentWallSlideFeel, string currentSpecialAbility)
        {
            this.currentMoveFeel = currentMoveFeel;
            this.currentJumpFeel = currentJumpFeel;
            this.currentWallJumpFeel = currentWallJumpFeel;
            this.currentWallSlideFeel = currentWallSlideFeel;
            this.currentSpecialAbility = currentSpecialAbility;
        }
    }
    CurrentAbilities currentAbilities;

    // Start is called before the first frame update
    void Start()
    {
        panelRectTransform = abilitySwitchPanel.GetComponent<RectTransform>();
        timeController = GetComponent<TimeController>();
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
            panelRectTransform.anchoredPosition = new Vector2(panelRectTransform.anchoredPosition.x, panelRectTransform.anchoredPosition.y - panelRectTransform.rect.height);
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
            panelRectTransform.anchoredPosition = new Vector2(panelRectTransform.anchoredPosition.x, panelRectTransform.anchoredPosition.y + panelRectTransform.rect.height);
            isShown = false;

            //Update the ability state

            timeController.StopSlowMotion();
        }
    }
}

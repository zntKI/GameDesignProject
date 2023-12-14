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

    private int currentAbilitySelectedIndex = 0;
    private int abilityCount;

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

        SetAbilities();
    }

    private void SetAbilities()
    {
        Transform outerContainer = abilitySwitchPanel.transform;

        abilityCount = outerContainer.childCount;

        for (int i = 0; i < abilityCount; i++)
        {
            Transform innerContainer = outerContainer.GetChild(i);
            for (int j = 0; j < innerContainer.childCount; j++)
            {
                GameObject imageOption = innerContainer.GetChild(j).gameObject;

                if (imageOption.name == "Title")
                    continue;

                if (imageOption.name == "MARIO")
                    imageOption.SetActive(true);
                else
                    imageOption.SetActive(false);
            }
        }

        currentAbilities = new CurrentAbilities("MARIO", "MARIO", "MARIO", "MARIO", "DOUBLE_JUMP");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UpdatePanel();
        }
        else if (isShown && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            //Go through the options in the currently selected ability
            Transform ability = abilitySwitchPanel.transform.GetChild(currentAbilitySelectedIndex);
            UpdateAbilityAppear(ability, Input.GetKeyDown(KeyCode.RightArrow));
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
            //Change the index of the current ability selected
            currentAbilitySelectedIndex++;
            if (currentAbilitySelectedIndex == abilityCount)
            {
                currentAbilitySelectedIndex = 0;
            }
        }
    }

    private void UpdateAbilityAppear(Transform ability, bool isRight)
    {
        for (int i = 0; i < ability.childCount; i++)
        {
            GameObject presentAbility = ability.GetChild(i).gameObject;

            //Setting the current option false and the next/previous to true
            if (presentAbility.activeSelf)
            {
                presentAbility.gameObject.SetActive(false);

                int updateI = CalculateUpdateI(ability.childCount, i, isRight);
                GameObject abilityToUpdate = ability.GetChild(updateI).gameObject;

                if (abilityToUpdate.name != "Title")
                {
                    abilityToUpdate.SetActive(true);
                }
                else
                {
                    //Get the next/previous element, ignoring the Title one
                    updateI = CalculateUpdateI(ability.childCount, updateI, isRight);
                    ability.GetChild(updateI).gameObject.SetActive(true);
                }

                break;
            }
        }
    }

    private int CalculateUpdateI(int childCount, int currentI, bool isRight)
    {
        if (isRight)
            return currentI + 1 == childCount ? 0 : currentI + 1;
        else
            return currentI - 1 == -1 ? childCount - 1 : currentI - 1;
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySwitchPanelController : MonoBehaviour
{
    [SerializeField]
    private Image abilitySwitchPanel;
    [SerializeField]
    private PlayerController player;

    private RectTransform panelRectTransform;
    private TimeController timeController;

    private bool isShown = false;

    private int currentAbilitySelectedIndex = 0;
    private int abilityCount;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        panelRectTransform = abilitySwitchPanel.GetComponent<RectTransform>();
        timeController = GetComponent<TimeController>();

        SetAbilities();
    }

    private void SetAbilities(string basicAbility = "", string specialAbility = "")
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

                if (innerContainer.name == "BasicAbilityFeelContainer")
                {
                    if (basicAbility != "" && imageOption.name == basicAbility || (imageOption.name == "BASIC"))
                        imageOption.SetActive(true);
                    else
                        imageOption.SetActive(false);
                }
                else if (innerContainer.name == "SpecialAbilityContainer")
                {
                    if (specialAbility != "" && imageOption.name == specialAbility)
                        imageOption.SetActive(true);
                    else
                        imageOption.SetActive(false);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            UpdatePanel();

            //Go through the options in the currently selected ability
            Transform ability = abilitySwitchPanel.transform.GetChild(1);
            UpdateAbilityAppear(ability, Input.GetKeyDown(KeyCode.RightArrow));
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            UpdatePanel();

            //Go through the options in the currently selected ability
            Transform ability = abilitySwitchPanel.transform.GetChild(0);
            UpdateAbilityAppear(ability, Input.GetKeyDown(KeyCode.UpArrow));
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            HidePanel();
        }
    }

    private void UpdatePanel()
    {
        if (panelRectTransform == null)
        {
            GameObject image = GameObject.Find("AbilitySwitchPanel");
            abilitySwitchPanel = image.GetComponent<Image>();
            panelRectTransform = abilitySwitchPanel.GetComponent<RectTransform>();

            SetAbilities(player.basicAbilityFeel.ToString(), player.specialAbility.ToString());
        }
        if (!isShown)
        {
            panelRectTransform.anchoredPosition = new Vector2(panelRectTransform.anchoredPosition.x + panelRectTransform.rect.width, panelRectTransform.anchoredPosition.y);
            isShown = true;

            timeController.StartSlowMotion();
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
                //presentAbility.gameObject.SetActive(false);

                int updateI = CalculateUpdateI(ability.childCount, i, isRight);
                GameObject abilityToUpdate = ability.GetChild(updateI).gameObject;

                if (abilityToUpdate.name != "Title" && ((ability.name == "BasicAbilityFeelContainer" && player.moveFeels.Contains(abilityToUpdate.name))
                    || (ability.name == "SpecialAbilityContainer" && player.specialAbilities.Contains(abilityToUpdate.name))))
                {
                    abilityToUpdate.SetActive(true);
                    presentAbility.gameObject.SetActive(false);
                }
                else
                {
                    //Get the next/previous element, ignoring the Title one
                    updateI = CalculateUpdateI(ability.childCount, updateI, isRight);
                    abilityToUpdate = ability.GetChild(updateI).gameObject;
                    if ((ability.name == "BasicAbilityFeelContainer" && player.moveFeels.Contains(abilityToUpdate.name))
                        || (ability.name == "SpecialAbilityContainer" && player.specialAbilities.Contains(abilityToUpdate.name)))
                    {
                        ability.GetChild(updateI).gameObject.SetActive(true);
                        presentAbility.gameObject.SetActive(false);
                    }
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
            panelRectTransform.anchoredPosition = new Vector2(panelRectTransform.anchoredPosition.x - panelRectTransform.rect.width, panelRectTransform.anchoredPosition.y);
            isShown = false;

            //Update the ability state
            UpdateCurrentAbilities();

            timeController.shouldStop = true;
            player.ResetGravityScale();
        }
    }

    private void UpdateCurrentAbilities()
    {
        Transform outerContainer = abilitySwitchPanel.transform;
        for (int i = 0; i < abilityCount; i++)
        {
            Transform innerContainer = outerContainer.GetChild(i);
            GameObject innerContainerObject = innerContainer.gameObject;
            for (int j = 0; j < innerContainer.childCount; j++)
            {
                GameObject imageOption = innerContainer.GetChild(j).gameObject;

                if (!imageOption.activeSelf || imageOption.name == "Title")
                    continue;

                if (innerContainerObject.name.Contains("BasicAbilityFeel"))
                {
                    player.UpdateBasicAbilityFeel(imageOption.name);
                }
                else if (innerContainerObject.name.Contains("SpecialAbility"))
                {
                    player.UpdateSpecialAbility(imageOption.name);
                }
            }
        }
    }
}

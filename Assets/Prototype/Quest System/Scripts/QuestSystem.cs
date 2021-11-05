using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    [SerializeField] GameObject interactUI;
    GameObject questJournal;

    GameObject questTracker;
    GameObject questInformation;
    GameObject claimButton;
    GameObject acceptButton;

    public GameObject closestQuestGiver;

    QuestManager questManager;
    QuestWindow questWindow;
    QuestGiver questGiver;

    CameraControllerMain cameraController;
    TestMovement testMovement;

    List<GameObject> questGivers;

    bool isClose = false;
    bool questsActive = false;

    private void Awake()
    {
        IntializeReferences();
    }

    private void Start()
    {
        Cursor.visible = false;
        questJournal.SetActive(false);
        claimButton.SetActive(false);
        questInformation.SetActive(false);
        questTracker.SetActive(false);
    }

    private void Update()
    {
        GetClosestQuestGiver();
        if(questGivers.Count >0)
        {
            CheckIfCloseToQuestGiver();
            IfCloseActivateUI();

            if (interactUI.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (!questGiver.completedQuest)
                    {
                        claimButton.SetActive(false);
                    }
                    if (questInformation.activeSelf)
                    {
                        CloseQuestInfo();
                    }
                    else if (questGiver.completedQuest)
                    {
                        claimButton.SetActive(true);
                        DisableCharacterRotation();
                        questManager.InitializeWindow(questGiver.quest);
                        Cursor.visible = true;

                    }
                    else if (!questGiver.acceptedQuest)
                    {
                        acceptButton.SetActive(true);
                        DisableCharacterRotation();
                        questManager.InitializeWindow(questGiver.quest);
                        Cursor.visible = true;
                    }
                    else
                    {
                        EnableCharacterRotation();
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                OpenQuestJournal();
            }
        }
    }

    private void OpenQuestJournal()
    {
        CheckIfQuestActive();
        if (questsActive)
        {
            ActivateOrDisactivateQuestJournal();
        }
    }

    void ActivateOrDisactivateQuestJournal()
    {
        if (questJournal.activeSelf == true)
        {
            questWindow.CloseWindow();
            Cursor.visible = false;
            EnableCharacterRotation();
            questInformation.SetActive(false);
            questJournal.SetActive(false);
        }
        else
        {
            Cursor.visible = true;
            DisableCharacterRotation();
            questJournal.SetActive(true);
        }
    }

    private void CheckIfQuestActive()
    {
        for (int i = 0; i < questGivers.Count; i++)
        {
            if (questGivers[i].GetComponent<QuestGiver>().questActive)
            {
                questsActive = true;
                return;
            }
            else questsActive = false;
        }
    }

    private void IfCloseActivateUI()
    {
        if (isClose && !questGiver.acceptedQuest || isClose && questGiver.completedQuest)
        {
            interactUI.SetActive(true);
        }
        else
        {
            interactUI.SetActive(false);
        }
    }

    private void CheckIfCloseToQuestGiver()
    {
        float distance = Vector3.Distance(transform.position, closestQuestGiver.transform.position);
        if (distance < 5) isClose = true;
        else isClose = false;
    }

    private void GetClosestQuestGiver()
    {
        float closestQG = Mathf.Infinity;
        for (int i = 0; i < questGivers.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, questGivers[i].transform.position);
            if (distance < closestQG)
            {
                closestQG = distance;
                closestQuestGiver = questGivers[i];
                questGiver = closestQuestGiver.GetComponent<QuestGiver>();
            }
        }
    }

    public void CloseQuestInfo()
    {
        Cursor.visible = false;
        if (questJournal.activeSelf)
        {
            questJournal.SetActive(false);
        }
        if (!questGiver.acceptedQuest && isClose)
        {
            questWindow.CloseWindow();
            EnableCharacterRotation();
        }
        else
        {
            questWindow.CloseWindow();
            EnableCharacterRotation();
        }
    }
    
    private void DisableCharacterRotation()
    {
        cameraController.enabled = false;
        testMovement.enabled = false;
    }

    public void EnableCharacterRotation()
    {
        cameraController.enabled = true;
        testMovement.enabled = true;
    }

    private void InitializeQuestGiversList()
    {
        questGivers = new List<GameObject>();
        var _questGivers = GameObject.FindGameObjectsWithTag("QuestGiver");
        foreach (var questGiver in _questGivers)
        {
            questGivers.Add(questGiver);
        }
    }

    private void IntializeReferences()
    {
        InitializeQuestManagers();
        InitializeQuestUI();
        InitializeQuestGiversList();
        InitializeCharacterControls();
    }

    private void InitializeQuestManagers()
    {
        questManager = FindObjectOfType<QuestManager>();
        questWindow = FindObjectOfType<QuestWindow>();
    }

    private void InitializeCharacterControls()
    {
        cameraController = FindObjectOfType<CameraControllerMain>();
        testMovement = FindObjectOfType<TestMovement>();
    }

    private void InitializeQuestUI()
    {
        questInformation = GameObject.FindGameObjectWithTag("QuestInformation");
        questTracker = GameObject.FindGameObjectWithTag("QuestTracker");
        questJournal = GameObject.FindGameObjectWithTag("QuestContainer");
        acceptButton = GameObject.FindGameObjectWithTag("AcceptButton");
        claimButton = GameObject.FindGameObjectWithTag("ClaimButton");
    }

    public void AcceptQuest()
    {
        Cursor.visible = false;

        questManager.InstantiateQuestButton(questGiver.quest);
        
        questGiver.acceptedQuest = true;

        questGiver.questActive = true;

        EnableCharacterRotation();

        questWindow.CloseWindow();

        acceptButton.SetActive(false);

    }

    public void Claim()
    {
        interactUI.SetActive(false);
        Debug.LogWarning("Congrats !" + questGiver.quest.Reward.XP);
        Debug.LogWarning("Congrats !" + questGiver.quest.Reward.Currency);
        Destroy(questManager.questsContent.GetChild(questManager.CurrentQuests.IndexOf(questGiver.quest)).gameObject);
        questManager.CurrentQuests.Remove(questGiver.quest);
        questWindow.CloseWindow();
        questGiver.claimedQuest = true;
        questGivers.Remove(closestQuestGiver);
    }
}
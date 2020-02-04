using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.AI;
using TMPro;
using System;

[RequireComponent(typeof(PlayerAnimController))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PathCreation.PathFollower))]
[RequireComponent(typeof(NavMeshAgent))]

public abstract class ABPlayerScript : MonoBehaviour, IClickable
{
    public delegate void OnGameEvent(int i);
    public static event OnGameEvent onGameEvent;

    private Outline outlineComponent;
    private PlayerAnimController animController;
    private PathFollower pathFollower;
    private NavMeshAgent navMeshAgent;

    [SerializeField] GameObject pointerComponent = null;
    [SerializeField] SO_PlayerInfo playerInfo = null;
    private bool isDecreaseScoreRunning = false;
    private bool isOnCrossing = false;
    private bool isOnRoad;
    private bool rulebooksSpawned = false;

    public SO_PlayerInfo PlayerInfo { get => playerInfo; }

    private void Awake()
    {
        outlineComponent = GetComponent<Outline>();
        animController = GetComponent<PlayerAnimController>();
        pathFollower = GetComponent<PathFollower>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (!CheckRefs())
            return;

        playerInfo.attachedObject = gameObject;

        SwitchComponents(false);
        //SwitchComponents(true);
    }

    private void OnEnable()
    {
        pathFollower.enabled = true;
    }

    // Process events when player begins collision with other objects
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Vehicle")
        {
            //Debug.Log("Player got hit by a vehicle");
        }
        else if (other.gameObject.tag.Substring(0, 4) == "Book")
        {
            Debug.Log("HIT BOOK: " + other.gameObject.tag);
            RulesScript ruleBook = other.GetComponent<RulesScript>();
            ruleBook.CollideWithPlayer();

        }
        else if (other.gameObject.tag == "PedestrianBalcombe")
        {
            MainManager.Instance.SetVehicleSpeed(0.0f);
            isOnCrossing = true;
        }
        else if (other.gameObject.tag == "ComoCrossing")
        {
            MainManager.Instance.SetVehicleSpeed(0.0f);
            isOnCrossing = true;
        }
        else if (other.gameObject.tag == "Road")
        {
            if (MainManager.Instance.GetState() == EGameState.QUEST_START)
            {
                isOnRoad = true;

                if (!isOnCrossing)
                    StartCoroutine(DecreaseScore());
            }
        }
        else if (other.gameObject.tag == "Station")
        {
            if (MainManager.Instance.CurrSelectedPlayer.PlayerInfo.characterMission == EMissionType.GET_TO_STATION)
            {
                Outline outline = other.GetComponent<Outline>();
                outline.OutlineWidth = 0.0f;
                SphereCollider collider = other.GetComponent<SphereCollider>();
                collider.gameObject.SetActive(false);
                MainManager.Instance.UpdateScore(EScoreEvent.AT_STATION);
                MainManager.Instance.SetState(EGameState.QUEST_COMPLETE);
            }
        }
        else if (other.gameObject.tag.Substring(0, 14) == "Level3Question")
        {
            if (MainManager.Instance.CurrSelectedPlayer.PlayerInfo.characterMission == EMissionType.ANSWER_QUESTIONS)
            {
                int questionNo = Int32.Parse(other.gameObject.tag.Substring(14, 1));
                questionNo--;
                UIManager.Instance.ShowQuestionUI(questionNo);
            }
        }
    }

    // Process events when player continues started collisions with other objects
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Road")
        {
            if (MainManager.Instance.GetState() == EGameState.QUEST_START)
            {
                Debug.Log("Player still on the road - Collider");
                Debug.Log("Is On Crosssing: " + isOnCrossing);
                isOnRoad = true;

                if (!isOnCrossing)
                    StartCoroutine(DecreaseScore());
            }
        }
    }

    // Coroutine to reduce score when player is on the road
    IEnumerator DecreaseScore()
    {
        if (!isDecreaseScoreRunning && isOnRoad)
        {
            isDecreaseScoreRunning = true;

            while (isOnRoad)
            {
                yield return new WaitForSeconds(1.5f);

                MainManager.Instance.UpdateScore(EScoreEvent.ON_ROAD);
                isDecreaseScoreRunning = false;
            }
        }
    }

    // Process events when player ends collision with other objects
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Road")
        {
            if (MainManager.Instance.GetState() == EGameState.QUEST_START)
            {
                Debug.Log("Player stepped off the road - Trigger exit");
                // Game End
                isOnRoad = false;
            }
        }
        else if (other.gameObject.tag == "PedestrianBalcombe")
        {
            Debug.Log("Xing EXIT");
            isOnCrossing = false;
            MainManager.Instance.SetVehicleSpeed(1.3f);
        }
        else if (other.gameObject.tag == "ComoCrossing")
        {
            Debug.Log("Como EXIT");
            isOnCrossing = false;
            MainManager.Instance.SetVehicleSpeed(1.3f);
        }
    }

    // Check components on player game object are initialised
    private bool CheckRefs()
    {
        bool check = true;
        if (outlineComponent == null)
        {
            Debug.LogError("Outline : Ref Missing - on " + transform.name);
            check = false;
        }
        else if (animController == null)
        {
            Debug.LogError(" PlayerAnimController : Ref Missing - on " + transform.name);
            check = false;
        }
        else if (pointerComponent == null)
        {
            Debug.LogError("Pointer : Ref Missing - on " + transform.name);
            check = false;
        }
        else if (pathFollower == null)
        {
            Debug.LogError("PathFollower : Ref Missing - on " + transform.name);
            check = false;
        }
        else if (PlayerInfo == null)
        {
            Debug.LogError("Character UI Info : Ref Missing - on " + transform.name);
            check = false;
        }
        return check;
    }

    // Toggle to enable/disbale path, pointer and outline components
    public void SwitchComponents(bool condition)
    {
        outlineComponent.enabled = !condition;
        animController.enabled = condition;
        pointerComponent.SetActive(condition);

        pathFollower.enabled = !condition;
    }

    // Event handler for mouse click on player - NOT CURRENTLY IN USE
    public void OnClick()
    {
        if (MainManager.Instance.CurrSelectedPlayer != this)
            return;

        SwitchComponents(true);

        //MainManager.Instance.SetState(EGameState.QUEST_START);
        EnactSpecificMissionActions();
    }

    private void EnactSpecificMissionActions()
    { 
        switch (MainManager.Instance.CurrSelectedPlayer.PlayerInfo.characterMission)
        {
            case EMissionType.FIND_CORRECT_RULES:
                if (!rulebooksSpawned)
                {
                    SpawnManager.Instance.StartSpawn(ESpawnSelection.RULES);
                    rulebooksSpawned = true;
                }
                break;
            case EMissionType.GET_TO_STATION:
                GameObject station = GameObject.FindGameObjectWithTag("Station");
                Outline outline = station.GetComponent<Outline>();
                outline.OutlineWidth = 7.0f;
                break;
            case EMissionType.ANSWER_QUESTIONS:
                UIManager.Instance.ShowDonuts(true);
                break;
            default:
                break;
        }
    }

    public void SendMessageToManager()
    {
        throw new System.NotImplementedException();
    }

    public void MoveToDestination(Vector3 pos)
    {
        navMeshAgent.SetDestination(pos);
    }
}

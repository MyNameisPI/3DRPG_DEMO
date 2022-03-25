using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
public class SceneController : Singleton<SceneController>,IEndGameObserver
{
    public GameObject playerPrefab;
    GameObject player;
    NavMeshAgent playAgent;
    public SceneFader sceneFaderPrafab;

    bool fadeFinished;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinished = true;
    }
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SamScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName,transitionPoint.destinationTag));
                break;
            default:
                break;
        }
    }
    

    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destination)
    {
        SaveManager.Instance.SavePlayerData();
        InventoryManager.Instance.SaveDate();

        if (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetDestination(destination).transform.position, GetDestination(destination).transform.rotation);
            SaveManager.Instance.LoadPlayerData();
            InventoryManager.Instance.LoadDate();
            yield break;
        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject;
            playAgent = player.GetComponent<NavMeshAgent>();
            playAgent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destination).transform.position, GetDestination(destination).transform.rotation);
            playAgent.enabled = true;
            yield return null;
        }
        
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destination)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();

        for (int i = 0; i < entrances.Length; i++)
        {
            if(entrances[i].destinationTag == destination)
            {
                return entrances[i];
            }
        }

        return null;
    }

    public void TransitionFirstLevel()
    {

        StartCoroutine(LoadLevel("Level1"));
    }

    public void TransitonToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    IEnumerator LoadLevel(string scene)
    {
        SceneFader sceneFader = Instantiate(sceneFaderPrafab); 
        if(scene!=string.Empty)
        {
            yield return StartCoroutine(sceneFader.FadeOut(1.5f));
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);

            SaveManager.Instance.LoadPlayerData();
            InventoryManager.Instance.SaveDate();
            yield return StartCoroutine(sceneFader.FadeIn(1.5f));
            yield break;
        }
    }

    IEnumerator LoadMain()
    {
        SceneFader sceneFader = Instantiate(sceneFaderPrafab);
        yield return StartCoroutine(sceneFader.FadeOut(1.5f));
        yield return SceneManager.LoadSceneAsync("MainMenu");
        yield return StartCoroutine(sceneFader.FadeIn(1.5f));
    }

    public void EndNotify()
    {
        Debug.Log("EndNotify In SceneController");
        InventoryManager.Instance.endGameText.gameObject.SetActive(true);
        InventoryManager.Instance.endGameText.text = "YOU DIED";
        InventoryManager.Instance.endGameText.color = Color.red;
        if (fadeFinished)
        {
            fadeFinished = false;

            StartCoroutine(WaitForSecondAndLoadMain(2f));
        }
    }

    public void PlayerWin()
    {
        InventoryManager.Instance.endGameText.gameObject.SetActive(true);
        InventoryManager.Instance.endGameText.text = "YOU WIN";
        InventoryManager.Instance.endGameText.color = Color.blue;

        if (fadeFinished)
        {
            fadeFinished = false;

            StartCoroutine(WaitForSecondAndLoadMain(2f));
        }
    }

    IEnumerator WaitForSecondAndLoadMain(float time)
    {
        yield return new WaitForSeconds(time);
        StartCoroutine(LoadMain());
    }
}

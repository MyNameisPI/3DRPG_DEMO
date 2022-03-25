using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{


    public CharacterStats playerStats;

    private CinemachineFreeLook followCamera;

    public List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    public void RigisterPlayer(CharacterStats player)
    {
        //由对象调用将自己加入到GameManager中
        playerStats = player;
        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if (followCamera!=null)
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }

    public void AddObserver(IEndGameObserver observer)
    {
        //将观察者添加到List列表中
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)
    {
        //将观察者移除到List列表中
        endGameObservers.Remove(observer);
    }

    public void NotifyObserver()
    {
        Debug.Log("GameManage Obserber");
        Debug.Log(endGameObservers == null);
        //遍历列表 调用所有的观察者的方法
        foreach(var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }

    public void WInNotify()
    {
        foreach (var observer in endGameObservers)
        {
            observer.PlayerWin();
        }
    }


    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<TransitionDestination>())
        {
            if (item.destinationTag == TransitionDestination.DestinationTag.ENTER) return item.transform;
        }

        return null;
    }
}

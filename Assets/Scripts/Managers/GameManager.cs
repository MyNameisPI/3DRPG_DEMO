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
        //�ɶ�����ý��Լ����뵽GameManager��
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
        //���۲�����ӵ�List�б���
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)
    {
        //���۲����Ƴ���List�б���
        endGameObservers.Remove(observer);
    }

    public void NotifyObserver()
    {
        Debug.Log("GameManage Obserber");
        Debug.Log(endGameObservers == null);
        //�����б� �������еĹ۲��ߵķ���
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

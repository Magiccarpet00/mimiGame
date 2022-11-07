using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Character[] players;

    public Animator animatorFlower;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].playerID = i;
        }
    }
    
    public void WhoWin(int looser)
    {
        players[(looser + 1) % 2].Win();
    }

    public void AnimFlower()
    {
        animatorFlower.SetTrigger("pnjSpawn");
    }

    

}

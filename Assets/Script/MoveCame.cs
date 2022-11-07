using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCame : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    
    public void Update()
    {
        float posX = (player1.position.x + player2.position.x) / 2;
        transform.position = (new Vector3(posX, transform.position.y, -10f));
    }

}

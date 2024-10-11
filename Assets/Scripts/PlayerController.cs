using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void StartTurn()
    {

    }

    void EndTurn()
    {
        FindObjectOfType<TurnManager>().EndPlayerTurn();
    }
}

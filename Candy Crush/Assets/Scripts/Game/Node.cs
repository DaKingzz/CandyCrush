using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool isAvailable;

    public GameObject candy;

    public Node(bool available, GameObject candyObj)
    {
        isAvailable = available;
        candy = candyObj;
    }
}

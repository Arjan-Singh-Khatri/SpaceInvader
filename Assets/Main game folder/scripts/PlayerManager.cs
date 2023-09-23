using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    // Start is called before the first frame update
    void Awake()
    {
        Instantiate(playerObject);
    }

}

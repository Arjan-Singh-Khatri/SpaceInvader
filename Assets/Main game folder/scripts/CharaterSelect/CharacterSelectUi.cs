using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUi : MonoBehaviour
{
    [SerializeField] Button ready;

    private void Start()
    {
        ready.onClick.AddListener(() =>
        {
            ReadyLogic.instance.CallPlayerReadyRpc();
        });
    }
}

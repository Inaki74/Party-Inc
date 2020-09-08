using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Helper script that removes components that cant and shant be shared across network users.
/// </summary>
public class RemoveIfNotMine : MonoBehaviourPun
{
    public List<MonoBehaviour> monosToDisable;

    public string mine;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        foreach(var mono in monosToDisable)
        {
            if(mono != null && photonView.IsMine)
            {
                mono.enabled = true;
            }
        }
    }
}

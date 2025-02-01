using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class Target : NetworkBehaviour
{
    internal NetworkVariable<float> health = new NetworkVariable<float>(0);


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsLocalPlayer)
        {
            health.OnValueChanged += HealthChanged;
        }
        if (IsServer)
        {
            health.Value = 1;
        }
    }

    public void HealthChanged(float previous, float current)
    {
        Debug.Log("Health went from " + previous + " to " + current);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

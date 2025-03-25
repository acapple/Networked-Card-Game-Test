using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public abstract class Target : NetworkBehaviour
{
    internal NetworkVariable<float> health = new NetworkVariable<float>(0);
    [SerializeField]
    internal Deck deck;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkHud.nh.print("A target is spawned!");
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
        NetworkHud.nh.print("Health went from " + previous + " to " + current);
    }

    [Rpc(SendTo.Server)]
    internal virtual void RequestCardPlayedServerRPC(int cardNum, int section) { }
}

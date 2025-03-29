using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

/// <summary>
/// Base class for players & enemies
/// </summary>
public abstract class Target : NetworkBehaviour
{
    internal NetworkVariable<float> health = new NetworkVariable<float>(0);
    [SerializeField]
    internal Deck deck;
    [SerializeField]
    internal UnityEngine.UI.RawImage image;


    /// <summary>
    /// Initiate the target's health
    /// </summary>
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


    /// <summary>
    /// Health changed. used for checking health is correct between all clients. can delete i think?
    /// </summary>
    /// <param name="previous"></param>
    /// <param name="current"></param>
    public void HealthChanged(float previous, float current)
    {
        NetworkHud.nh.print("Health went from " + previous + " to " + current);
    }


    /// <summary>
    /// Every target has a deck and will run through it. 
    /// 
    /// TODO: Code this function for enemies
    /// </summary>
    /// <param name="cardNum"> idk... used for players </param>
    /// <param name="section"> section to play the card in </param>
    [Rpc(SendTo.Server)]
    internal virtual void RequestCardPlayedServerRPC(int cardNum, int section) { }
}

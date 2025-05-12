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
    [SerializeField]
    private int defaultHealth = 1;
    internal NetworkVariable<float> health = new NetworkVariable<float>(0);
    [SerializeField]
    internal Deck deck;
    [SerializeField]
    internal UnityEngine.UI.RawImage image;
    [SerializeField]
    internal UnityEngine.UI.Slider healthSlider;


    /// <summary>
    /// Initiate the target's health
    /// </summary>
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkHud.nh.print("A target is spawned!");
        health.OnValueChanged += HealthChanged;
        healthSlider.minValue = 0;
        healthSlider.maxValue = defaultHealth;
        if (IsServer)
        {
            health.Value = defaultHealth;
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
        //defaultHealth = (int)health.Value;
        healthSlider.value = health.Value;
    }


    internal void moveTargetServer(int section)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        Terrain.terrain.moveImage(image, section);
        moveTargetClientRPC(section);
    }


    [Rpc(SendTo.NotServer)]
    internal void moveTargetClientRPC(int section)
    {
        Terrain.terrain.moveImage(image, section);
    }


    /// <summary>
    /// Every target has a deck and will run through it. 
    /// </summary>
    /// <param name="cardNum"> idk... used for players </param>
    /// <param name="section"> section to play the card in </param>
    [Rpc(SendTo.Server)]
    internal virtual void RequestCardPlayedServerRPC(int cardNum, int section) { }
}

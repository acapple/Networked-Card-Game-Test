using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    internal static GameManager gm;
    [SerializeField]
    internal bool debugMessages = false;
    internal bool repetitiveMessages = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (gm == null)gm = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

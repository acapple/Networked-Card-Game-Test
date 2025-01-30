using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Target
{
    [SerializeField]
    List<Card> hand;
    [SerializeField]
    Deck deck;
    [SerializeField]
    GameObject cardHandSample;
    [SerializeField]
    Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        deck.ShuffleDeck();
        for (int i=0; i<5; i++)
        {
            hand.Add(deck.drawCard());
        }
        SortHand();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SortHand()
    {
        int CardWidth = (int) Mathf.Floor(canvas.pixelRect.width / hand.Count);
        for (int i=0; i<hand.Count; i++)
        {
            GameObject temp = Instantiate(cardHandSample, new Vector3(i * CardWidth, (int)canvas.pixelRect.height / 2, 0), Quaternion.identity, canvas.transform);
            temp.GetComponent<Image>().color = new Color(Random.Range(0.0f, 1), Random.Range(0.0f, 1), Random.Range(0.0f, 1));
            temp.GetComponent<RectTransform>().sizeDelta = new Vector2(CardWidth, 50);
        }
    }
}

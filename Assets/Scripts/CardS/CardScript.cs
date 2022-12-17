using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Mirror;

public class CardScript : NetworkBehaviour
{

    public Sprite cardFront;
    public Sprite cardBack;

    public Sprite sprCharisma;
    public Sprite sprCunning;
    public Sprite sprIntelligence;
    public Sprite sprStrength;

    private string title;
    private string effect;
    private string stat;
    public bool hovered;
    public bool selected = false;
    public bool prevSelected = false;
    public int sortingDefault;

    public string Title
    {
        get { return title; }
        set { title = value; }
    }

    public string Effect
    {
        get { return effect; }
        set { effect = value; }
    }

    public string Stat
    {
        get { return stat; }
        set 
        { 
            stat = value;
            switch (stat)
            {
                case "charisma":
                    cardBack = sprCharisma;
                    break;
                case "cunning":
                    cardBack = sprCunning;
                    break;
                case "intelligence":
                    cardBack = sprIntelligence;
                    break;
                case "strength":
                    cardBack = sprStrength;
                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = cardBack;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = cardBack;
    }

    public void Flip()
    {
        Sprite currentSprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        if (!hasAuthority)
        {
            //Debug.Log("No Authority");
        }
        else
        {
            if (currentSprite = cardBack)
               gameObject.GetComponent<SpriteRenderer>().sprite = cardFront;
            
            //Debug.Log("I have Authority");
        }
    }

    public void Enlarge() {
        if (hovered && cardBack != null) {
            this.transform.localScale = new Vector3(1.25f, 1.25f, 0);
            this.GetComponent<SpriteRenderer>().sortingOrder = 9;
        }
        if (!hovered && cardBack != null) {
            this.transform.localScale = new Vector3(1f, 1f, 0);
            this.GetComponent<SpriteRenderer>().sortingOrder = sortingDefault;
        }
    }
    
    public void OnMouseEnter() {
        hovered = true;
    }
    public void OnMouseExit() {
        hovered = false;
    }

    [Command(requiresAuthority = false)]
    public void OnMouseDown()
    {
        if (cardBack != null)
        {
            prevSelected = selected;
            selected = !selected;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Mirror;
using System;
using System.Linq;

//CLIENT SCRIPT
public class PlayerScript : NetworkBehaviour
{

    //Fields
    public GameState FSM;

    public EventManager eMan;

    public bool added = false;

    public GameObject turnToken;

    [SyncVar]
    public string playerName = "";

    [SyncVar]
    public bool untargetable = false;

    [SyncVar]
    public int powerGained = 0;

    [SyncVar]
    public int powerLost = 0;

    [SyncVar]
    public bool turnTaken = false;

    public int currentBet = 0;

    [SyncVar]
    public bool cantLosePower = false;

    [SyncVar]
    public string guess1 = "";
    [SyncVar]
    public string guess2 = "";
    [SyncVar]
    public string guess3 = "";

    [SyncVar]
    public string passive2 = "";

    [SyncVar]
    public bool cantLoseStats = false;

    public int numSelected;

    public int numToReveal = 0;

    #region stats
    [SyncVar]
    private int charisma;
    [SyncVar]
    private int strength;
    [SyncVar]
    private int intelligence;
    [SyncVar]
    private int cunning;
    [SyncVar]
    public string allyStat = "strength";

    public int Charisma
    {
        get { return charisma; }
        set {
            if (value == 0)
                charisma = value;
            charisma += value;
            if (charisma < 0)
                charisma = 0;
        }
    }
    public int Strength
    {
        get { return strength; }
        set
        {
            if (value == 0)
                strength = value;
            strength += value;
            if (strength < 0)
                strength = 0;
        }
    }
    public int Intelligence
    {
        get { return intelligence; }
        set
        {
            if (value == 0)
                intelligence = value;
            intelligence += value;
            if (intelligence < 0)
                intelligence = 0;
        }
    }
    public int Cunning
    {
        get { return cunning; }
        set
        {
            if (value == 0)
                cunning = value;
            cunning += value;
            if (cunning < 0)
                cunning = 0;
        }
    }

    public Text charismaText;
    public Text strengthText;
    public Text intelligenceText;
    public Text cunningText;

    private GameObject[] addButtons;
    private GameObject[] subButtons;

    public GameObject passiveOption1;
    public GameObject passiveOption2;
    public GameObject passiveOption3;

    [SyncVar]
    private int maxPoints = 8;
    [SyncVar]
    private int availablePoints = 8;

    public int AvailablePoints
    {
        get { return availablePoints; }
        set { availablePoints += value; }
    }
    public int MaxPoints
    {
        get { return maxPoints; }
        set { maxPoints += value; }
    }

    #endregion

    private GameObject readyButton;

    [SyncVar]
    public PlayerList playerList;

    [SyncVar]
    public int playerNumber;

    [SyncVar]
    public bool ready;

    [SyncVar]
    public string highest = "";

    [SyncVar]
    public string lowest = "";

    public string Highest
    {
        get { return highest; }
        set
        {
            highest = value;
        }
    }

    [SyncVar]
    public bool hasHighest = false;

    [SyncVar]
    public bool hasDeck = false;

    [SyncVar]
    public bool hasPassive = false;

    [SyncVar]
    public bool sawDeck = false;

    [SyncVar]
    public bool cardSlotsSpawned = false;

    [SyncVar]
    public bool cardsSpawned = false;

    [SyncVar]
    private int power = 500;

    public int Power
    {
        get { return power; }
        set { power += value; }
    }

    public DeckScript deck;

    public PassiveManager passiveManager;

    [SyncVar]
    public Passive passive;

    [SyncVar]
    public string passiveName;

    [SyncVar]
    public bool LockedIn = false;

    [SyncVar]
    public bool threeSelected;

    public PlayerScript[] playerCheck;

    public GameObject[] cardSlots;
    public GameObject[] enemySlots1;
    public GameObject[] enemySlots2;
    public GameObject[] enemySlots3;
    public PlayerScript enemy1;
    public PlayerScript enemy2;
    public PlayerScript enemy3;

    int g = 0;
    public readonly SyncList<string[]> cards = new SyncList<string[]>();
    public readonly SyncList<string[]> hand = new SyncList<string[]>();
    public readonly SyncList<Passive> choicesList = new SyncList<Passive>();

    GameObject bntTop;
    GameObject bntLeft;
    GameObject bntRight;
    GameObject lockInButton;

    Text pLabel1;
    Text pLabel2;
    Text pLabel3;

    //Properties
    //Methods
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
    }

    public void Start()
    {
        playerList = GameObject.Find("PlayerList").GetComponent<PlayerList>();
        FSM = GameObject.Find("FSM").GetComponent<GameState>();

        passive = gameObject.GetComponent<Passive>();

        passiveOption1 = GameObject.Find("bntChoice1");
        passiveOption2 = GameObject.Find("bntChoice2");
        passiveOption3 = GameObject.Find("bntChoice3");

        lockInButton = GameObject.Find("LockInButton1");

        eMan = GameObject.Find("EventManager").GetComponent<EventManager>();

        charismaText = GameObject.Find("CharismaCounter").GetComponent<Text>();
        strengthText = GameObject.Find("StrengthCounter").GetComponent<Text>();
        intelligenceText = GameObject.Find("IntelligenceCounter").GetComponent<Text>();
        cunningText = GameObject.Find("CunningCounter").GetComponent<Text>();
        addButtons = GameObject.FindGameObjectsWithTag("add");
        subButtons = GameObject.FindGameObjectsWithTag("sub");

        turnToken = GameObject.Find("Token");

        cardSlots = GameObject.FindGameObjectsWithTag("CardSlot");
        enemySlots1 = GameObject.FindGameObjectsWithTag("1");
        enemySlots2 = GameObject.FindGameObjectsWithTag("2");
        enemySlots3 = GameObject.FindGameObjectsWithTag("3");

        readyButton = GameObject.Find("Ready");

        passiveManager = GameObject.Find("PassiveManager").GetComponent<PassiveManager>();

        bntTop = GameObject.Find("PlayerTop");
        bntLeft = GameObject.Find("PlayerLeft");
        bntRight = GameObject.Find("PlayerRight");

        pLabel1 = GameObject.Find("PlayerLabel1").GetComponent<Text>();
        pLabel2 = GameObject.Find("PlayerLabel2").GetComponent<Text>();
        pLabel3 = GameObject.Find("PlayerLabel3").GetComponent<Text>();

        if (isLocalPlayer)
        {
            bntTop.SetActive(false);
            bntLeft.SetActive(false);
            bntRight.SetActive(false);
            passiveOption1.SetActive(false);
            passiveOption2.SetActive(false);
            passiveOption3.SetActive(false);
            lockInButton.SetActive(false);
            turnToken.SetActive(false);
        }

    }

    public void Update()
    {
        if (isServer)
        {
            playerCheck = GameObject.FindObjectsOfType<PlayerScript>();
        }
        //START ONCE ALL PLAYERS JOIN
        if (playerCheck.Length == 4 && !added && isServer)
        {
            CmdSendPlayers(this);
            added = true;
        }
        if (!isLocalPlayer)
            return;
        if (FSM == null)
            return;

        //GAMESTATE SPECIFIC EVENTS
        switch (FSM.CurrentState)
        {
            case GameStates.Setup:
                if (playerList.players.Count == 4)
                {
                    CmdSetPlayer();
                }
                break;
            case GameStates.Passive:
                if (isLocalPlayer)
                    readyButton.SetActive(false);

                //calculate players highest stat
                CalcHighest();
                CalcLowest();

                //create deck and get cards data
                if (!hasDeck && hasHighest)
                {
                    hasDeck = true;
                    deck = new DeckScript();
                    deck.CreateDeck(highest);
                    if (deck.cardData.Count > 7)
                    {
                        CmdFillDeck(deck.cardData);
                    }
                }

                //spawn passive choices
                if (g == 0 && hasHighest) {
                    passiveOption1.SetActive(true);
                    passiveOption2.SetActive(true);
                    passiveOption3.SetActive(true);
                    passiveManager.CmdSelectPassive(highest, this);
                    g = 1;
                }
                if (passive.passiveName != "" && isLocalPlayer)
                {
                    CmdSelectedPassive();
                }
                break;

            case GameStates.DrawCards:
                //CmdSwapFalse();
                CmdDrawCard();
                if (hand.Count == 6 && !cardsSpawned)
                {
                    //ran = true;
                    TransferData(cardSlots, this);
                    if (cardSlots[5].GetComponent<CardScript>().Title != "")
                        CmdSpawnedCards();
                }

                break;

            case GameStates.LoadEnemyCards:
                TransferEnemyData();
                switch (passive2)
                {
                    case "StrongAllies":
                        CmdSetAllyStat("strength");
                        break;
                    case "SmartAllies":
                        CmdSetAllyStat("intelligence");
                        break;
                    case "ShadyAllies":
                        CmdSetAllyStat("cunning");
                        break;
                    default:
                        break;
                }
                switch (passive.passiveName)
                {
                    case "StrongAllies":
                        CmdSetAllyStat("strength");
                        break;
                    case "SmartAllies":
                        CmdSetAllyStat("intelligence");
                        break;
                    case "ShadyAllies":
                        CmdSetAllyStat("cunning");
                        break;
                    default:
                        break;
                }
                numSelected = 0;
                foreach (GameObject g in cardSlots)
                {
                    if (g.GetComponent<CardScript>().selected && !g.GetComponent<CardScript>().prevSelected)
                    {
                        g.GetComponent<SpriteRenderer>().color = Color.green;
                        numSelected++;
                    } else
                        g.GetComponent<SpriteRenderer>().color = Color.white;
                }
                if (numSelected == 3 && !LockedIn)
                {
                    CmdThreeSelected(true);
                    lockInButton.SetActive(true);
                }

                else
                {
                    CmdThreeSelected(false);
                    lockInButton.SetActive(false);
                }
                break;

            case GameStates.Turn:
                if (LockedIn)
                    CmdUnlock();
                if (enemy1.hand.Count == 6)
                    UpdateData(enemySlots1, enemy1);
                if (enemy2.hand.Count == 6)
                    UpdateData(enemySlots2, enemy2);
                if (enemy3.hand.Count == 6)
                    UpdateData(enemySlots3, enemy3);
                if (hand.Count == 6)
                    UpdateData(cardSlots, this);
                if (FSM.currentPlayer.netId == this.netIdentity.netId)
                    turnToken.SetActive(true);
                else
                    turnToken.SetActive(false);
                break;
        }

        #region statChanges
        //DETECT CHANGES IN STATS
        charismaText.text = "Charisma: " + charisma;
        strengthText.text = "Strength: " + strength;
        intelligenceText.text = "Intelligence: " + intelligence;
        cunningText.text = "Cunning: " + cunning;

        if (FSM.CurrentState == GameStates.Event && eMan.currentEvent == "Three")
        {
            foreach (GameObject b in addButtons)
            {
                b.SetActive(false);
            }
            foreach (GameObject b in subButtons)
            {
                b.SetActive(true);
            }
        }
        else if (availablePoints > 0 && availablePoints < maxPoints)
        {
            foreach (GameObject b in addButtons)
            {
                b.SetActive(true);
            }
            foreach (GameObject b in subButtons)
            {
                b.SetActive(true);
            }
        }
        else if (availablePoints == maxPoints)
        {
            foreach (GameObject b in addButtons)
            {
                b.SetActive(true);
            }
            foreach (GameObject b in subButtons)
            {
                b.SetActive(false);
            }
        }
        else if (availablePoints == 0)
        {
            foreach (GameObject b in subButtons)
            {
                b.SetActive(false);
            }
            foreach (GameObject b in addButtons)
            {
                b.SetActive(false);
            }
        }
        else if (availablePoints < 0)
        {
            foreach (GameObject b in subButtons)
            {
                b.SetActive(true);
            }
            foreach (GameObject b in addButtons)
            {
                b.SetActive(false);
            }
        }
        #endregion
    }

    [Command(requiresAuthority = false)]
    public void CmdUnlock()
    {
        LockedIn = false;
    }

    [Command(requiresAuthority = false)]
    public void CmdResetCards()
    {
        cards.Clear();
        hand.Clear();
    }
    [Command(requiresAuthority = false)]
    public void CmdConfirm()
    {
        Debug.Log("");
    }

    [TargetRpc]
    public void SwapDeck(string h)
    {
        deck.CreateDeck(h);
        CmdResetCards();

      while (deck.cardData.Count < 40) {}

      CmdFillDeck(deck.cardData);

      for (int i = 0; i < 6; i++)
            CmdDrawCard();

      foreach (GameObject g in cardSlots)
          g.GetComponent<CardScript>().Title = "";

      foreach (GameObject g in cardSlots)
          TransferData(cardSlots, this);
    }

    [Command (requiresAuthority = false)]
    public void CmdThreeSelected(bool b)
    {
        threeSelected = b;
    }

    public void TransferEnemyData()
    {
        if (enemy1 == null || enemy2 == null || enemy3 == null)
        {
            foreach (PlayerScript p in playerList.players)
            {
                if (p.netId != this.netId)
                {
                    if (enemy1 == null)
                        enemy1 = p;
                    else if (enemy2 == null)
                        enemy2 = p;
                    else if (enemy3 == null)
                        enemy3 = p;
                }
            }
        }

        if (enemySlots1[5].GetComponent<CardScript>().Title == "")
            TransferData(enemySlots1, enemy1);
        else if (enemySlots2[5].GetComponent<CardScript>().Title == "")
            TransferData(enemySlots2, enemy2);
        else if (enemySlots3[5].GetComponent<CardScript>().Title == "")
            TransferData(enemySlots3, enemy3);
        if (enemy1 != null && enemy2 != null && enemy3 != null)
        {
            pLabel1.text = enemy1.playerName;
            pLabel2.text = enemy2.playerName;
            pLabel3.text = enemy3.playerName;
        }
        sendPlayerData();
    }

    [Command(requiresAuthority = false)]
    public void CmdSpawnedCards()
    {
        cardsSpawned = true;
    }

    [Command(requiresAuthority = false)]
    public void CmdSelectedPassive()
    {
        hasPassive = true;
    }

    [Command(requiresAuthority = false)]
    public void CmdDrawCard()
    {
        int rand = UnityEngine.Random.Range(0, cards.Count - 1);
        if (hand.Count < 6)
        {
            hand.Add(cards[rand]);
            cards.Remove(cards[rand]);
        }
    }

    public void UpdateData(GameObject[] slots, PlayerScript p)
    {
        int index = 0;
        foreach (GameObject g in slots)
        {
            if (g.GetComponent<CardScript>().Title != p.hand[index][1])
            {
                g.GetComponent<CardScript>().revealed = false;
            }
            g.GetComponent<CardScript>().Title = p.hand[index][1];
            g.GetComponent<CardScript>().Type = p.hand[index][0];
            g.GetComponent<CardScript>().ID = p.hand[index][4];
            index++;
        }
    }

    public void TransferData(GameObject[] slots, PlayerScript p)
    {
        int index = 0;
        foreach (GameObject g in slots)
        {
            if (g.GetComponent<CardScript>().Title == "")
            {
                g.GetComponent<CardScript>().Title = p.hand[index][1];
                g.GetComponent<CardScript>().Type = p.hand[index][0];
                g.GetComponent<CardScript>().ID = p.hand[index][4];
                break;
            }
            index++;
        }
    }

    public void CalcHighest()
    {
        string currentHighest = highest;
        string newHighest;
        int currentHigh = 0;
        switch (currentHighest)
        {
            case "strength":
                currentHigh = strength;
                break;
            case "intelligence":
                currentHigh = intelligence;
                break;
            case "charisma":
                currentHigh = charisma;
                break;
            case "cunning":
                currentHigh = cunning;
                break;
        }

        int[] statList = {strength, intelligence, charisma, cunning};
        int max = statList.Max();
        if (max > currentHigh)
        {
            if (statList[0] == max)
                newHighest = "strength";
            else if (statList[1] == max)
                newHighest = "intelligence";
            else if(statList[2] == max)
                newHighest = "charisma";
            else
                newHighest = "cunning";
            CmdSendHighest(newHighest);
        }
    }

    public void CalcLowest() {
        string currentLowest = lowest;
        string newlow;
        int currentlow = 1000;

        switch (currentLowest)
        {
            case "strength":
                currentlow = strength;
                break;
            case "intelligence":
                currentlow = intelligence;
                break;
            case "charisma":
                currentlow = charisma;
                break;
            case "cunning":
                currentlow = cunning;
                break;
        }

        int[] statList = {strength, intelligence, charisma, cunning};
        int min = statList.Min();

        if (min < currentlow) {
            if (statList[0] == min)
                newlow = "strength";
            else if (statList[1] == min)
                newlow = "intelligence";
            else if(statList[2] == min)
                newlow = "charisma";
            else
                newlow = "cunning";
            CmdSendLowest(newlow);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSendHighest(string h)
    {
        highest = h;
        hasHighest = true;
        if (FSM.CurrentState == GameStates.Turn || FSM.CurrentState == GameStates.Event)
        {
            if (passive.passiveName == "ShadyBusiness" || passive2 == "ShadyBuisness")
                ModifyPower(50);
            SwapDeck(h);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSendLowest(string l)
    {
        lowest = l;
    }

    [Command(requiresAuthority = false)]
    public void CmdFillDeck(List<string[]> cardData)
    {
        foreach (string[] s in cardData)
            cards.Add(s);
    }

    [Command(requiresAuthority = false)]
    public void CmdSendPlayers(PlayerScript player)
    {
        playerList.CmdAddPlayers(player);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetPlayer()
    {
        RpcSetPlayer();
    }

    [ClientRpc]
    public void RpcSetPlayer()
    {
        playerNumber = (int)netIdentity.netId;
        playerName = "Player " + (playerList.players.IndexOf(this) + 1);
        gameObject.name = $"{playerNumber}";
    } 

    public List<PlayerScript> sendPlayerData() {
        List<PlayerScript> playerslots = new List<PlayerScript>();
        playerslots.Add(enemy1); //bntright
        playerslots.Add(enemy2); //bntleft
        playerslots.Add(enemy3); //bnttop

        return playerslots;
    }

    public void UnhideButtons() {
        if ((passive.passiveName == "Precise" || passive2 == "Precise") 
            || ((!enemy1.untargetable) && ((enemy1.passive.passiveName != "Scrapper" && enemy1.passive2 != "Scrapper") 
            || ((enemy1.passive.passiveName == "Scrapper" || enemy1.passive2 == "Scrapper")  && enemy1.Power >= power))
            && !((passive.passiveName == "StrongAllies" || passive2 == "StrongAllies") && enemy1.Highest == "strength")
            && !((passive.passiveName == "SmartAllies" || passive2 == "SmartAllies") && enemy1.Highest == "intelligence")
            && !((passive.passiveName == "StrongAllies" || passive2 == "StrongAllies") && enemy1.Highest == "cunning")
            && !((enemy1.passiveName == "StrongAllies" || enemy1.passive2 == "StrongAllies") && Highest == "strength")
            && !((enemy1.passiveName == "SmartAllies" || enemy1.passive2 == "SmartAllies") && Highest == "intelligence")
            && !((enemy1.passiveName == "ShadyAllies" || enemy1.passive2 == "ShadyAllies") && Highest == "cunning")))//enemy1
            bntRight.SetActive(true);
        if ((passive.passiveName == "Precise" || passive2 == "Precise")
            || ((!enemy2.untargetable) && ((enemy2.passive.passiveName != "Scrapper" && enemy2.passive2 != "Scrapper")
            || ((enemy2.passive.passiveName == "Scrapper" || enemy2.passive2 == "Scrapper") && enemy2.Power >= power))
            && !((passive.passiveName == "StrongAllies" || passive2 == "StrongAllies") && enemy2.Highest == "strength")
            && !((passive.passiveName == "SmartAllies" || passive2 == "SmartAllies") && enemy2.Highest == "intelligence")
            && !((passive.passiveName == "StrongAllies" || passive2 == "StrongAllies") && enemy2.Highest == "cunning")
            && !((enemy2.passiveName == "StrongAllies" || enemy2.passive2 == "StrongAllies") && Highest == "strength")
            && !((enemy2.passiveName == "SmartAllies" || enemy2.passive2 == "SmartAllies") && Highest == "intelligence")
            && !((enemy2.passiveName == "ShadyAllies" || enemy2.passive2 == "ShadyAllies") && Highest == "cunning")))//enemy2
            bntLeft.SetActive(true);
        if ((passive.passiveName == "Precise" || passive2 == "Precise")
            || ((!enemy3.untargetable) && ((enemy3.passive.passiveName != "Scrapper" && enemy3.passive2 != "Scrapper")
            || ((enemy3.passive.passiveName == "Scrapper" || enemy3.passive2 == "Scrapper") && enemy3.Power >= power))
            && !((passive.passiveName == "StrongAllies" || passive2 == "StrongAllies") && enemy3.Highest == "strength")
            && !((passive.passiveName == "SmartAllies" || passive2 == "SmartAllies") && enemy3.Highest == "intelligence")
            && !((passive.passiveName == "StrongAllies" || passive2 == "StrongAllies") && enemy3.Highest == "cunning")
            && !((enemy3.passiveName == "StrongAllies" || enemy3.passive2 == "StrongAllies") && Highest == "strength")
            && !((enemy3.passiveName == "SmartAllies" || enemy3.passive2 == "SmartAllies") && Highest == "intelligence")
            && !((enemy3.passiveName == "ShadyAllies" || enemy3.passive2 == "ShadyAllies") && Highest == "cunning")))  //enemy3
            bntTop.SetActive(true);

        if(!bntLeft.activeSelf && !bntRight.activeSelf && !bntTop.activeSelf)
        {
            bntTop.SetActive(true);
            bntRight.SetActive(true);
            bntLeft.SetActive(true);
        }
    }

    public void hideButtons() {
        if (bntRight.activeInHierarchy == true)  //enemy1
            bntRight.SetActive(false);
        if (bntLeft.activeInHierarchy == true)  //enemy2
            bntLeft.SetActive(false);
        if (bntTop.activeInHierarchy == true)  //enemy3
            bntTop.SetActive(false);
    }

        [Command(requiresAuthority = false)]
        public void ModifyStats(string type, int amount) {
        if (cantLoseStats && amount < 0)
            return;
        switch (type) {
                case "strength":
                   Strength = amount;
                    break;
                case "charisma":
                    Charisma = amount;
                    break;
                case "intelligence":
                    Intelligence = amount;
                    break;
                case "cunning":
                    Cunning = amount;
                    break;
            }
            MaxPoints = amount;
        CalcLowest();
        CalcHighest();
    }

    [Command(requiresAuthority = false)]
    public void ResetStats()
    {
        AvailablePoints = Charisma + Strength + Cunning + Intelligence;
        Strength = 0;
        Charisma = 0;
        Intelligence = 0;
        Cunning = 0;
    }

    [Command(requiresAuthority = false)]
    public void ModifyPower(int amount)
    {
        if (cantLosePower && amount < 0)
            return;
        if (amount < 0 && (passive.passiveName == "Taunt" || passive2 == "Taunt"))
        {
            Strength = 1;
        }
        if (passive.passiveName == "Unstable" || passive2 == "Unstable")
        {
            amount *= 2;
        }
        Power = amount;
        if (amount > 0)
            powerGained += amount;
        else
            powerLost += -amount;
    }

    [Command(requiresAuthority = false)]
    public void AddPoints(int amount)
    {
        maxPoints = amount;
        availablePoints = amount;
    }

    [Command(requiresAuthority = false)]
    public void DiscardCard(int index, GameObject[] slots)
    {
        if (!FSM.EventTwo) {
            int rand = UnityEngine.Random.Range(0, cards.Count - 1);
            cards.Add(hand[index]);
            hand[index] = cards[rand];
            cards.Remove(cards[rand]);
            RpcFillSlot(connectionToClient, slots, hand[index][1], hand[index][0], hand[index][4]);
        }
        else
        {
            PlayerScript[] enemies = { enemy1, enemy2, enemy3 };
            int randEnemy = UnityEngine.Random.Range(0, 3);
            PlayerScript temp = enemies[randEnemy];
            int rand = UnityEngine.Random.Range(0, cards.Count - 1);
            cards.Add(hand[index]);
            hand[index] = temp.cards[rand];
            temp.cards.Remove(cards[rand]);
            RpcFillSlot(connectionToClient, slots, hand[index][1], hand[index][0], hand[index][4]);
        }
    }

    [Command(requiresAuthority = false)]
    public void DiscardRevealed(PlayerScript p, GameObject[] slots, PlayerScript targetPlayer, List<int> indexes)
    {
        for (int i = 0; i < 6; i++)
        {
                int rand = UnityEngine.Random.Range(0, 33);
                targetPlayer.cards.Add(targetPlayer.hand[indexes[i]]);
                targetPlayer.hand[indexes[i]] = targetPlayer.cards[rand];
                targetPlayer.cards.Remove(targetPlayer.cards[rand]);
                RpcFillSlot(targetPlayer.connectionToClient, slots, targetPlayer.hand[indexes[i]][1], targetPlayer.hand[indexes[i]][0], targetPlayer.hand[indexes[i]][4]);
        }
    }

    [TargetRpc]
    public void RpcFillSlot(NetworkConnection conn, GameObject[] slots, string title, string type, string id)
    {
        foreach (GameObject g in slots)
        {
            if (g.GetComponent<CardScript>().Title == "")
            {
                g.GetComponent<CardScript>().Title = title;
                g.GetComponent<CardScript>().Type = type;
                g.GetComponent<CardScript>().ID = id;
            }
        }
    }

    [TargetRpc]
    public void Turn(NetworkConnection conn)
    {
        if (passive.passiveName == "SeeDeck" || passive2 == "SeeDeck")
        {
            deck.SeeDeck();
        }
    }

    [Command(requiresAuthority = false)]
    public void setUntargetable(bool b)
    {
        untargetable = b;
    }

    [Command(requiresAuthority = false)]
    public void CmdturnIncrease()
    {
        FSM.turn++;
        turnTaken = false;
    }

    [Command (requiresAuthority = false)]
    public void CmdDisablePLoss(bool b)
    {
        cantLosePower = b;
    }
    [Command(requiresAuthority = false)]
    public void CmdDisableSLoss(bool b)
    {
        cantLoseStats = b;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetAllyStat(string s)
    {
        allyStat = s;
    }

    [TargetRpc]
    public void CheckGuess(NetworkConnection conn)
    {
            if (guess1 == enemy1.Highest)
            {
                enemy1.ModifyPower(-50);
                ModifyPower(50);
            }
            else
            {
                enemy1.ModifyPower(50);
                ModifyPower(-50);
            }

            if (guess2 == enemy2.Highest)
            {
                enemy2.ModifyPower(-50);
                ModifyPower(50);
            }
            else
            {
                enemy2.ModifyPower(50);
                ModifyPower(-50);
            }

            if (guess3 == enemy3.Highest)
            {
                enemy3.ModifyPower(-50);
                ModifyPower(50);
            }
            else
            {
                enemy3.ModifyPower(50);
                ModifyPower(-50);
            }
    }
}

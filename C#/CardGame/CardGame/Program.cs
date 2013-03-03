using System;
using System.Collections.Generic;
using System.Threading;

namespace CardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Process p = new Process(2, new GenVar.PLAYER[] { GenVar.PLAYER.HUMAN, GenVar.PLAYER.AI, GenVar.PLAYER.AI });

            do
            {
                p.Start();
            } while (p.End());
            string i = Console.ReadLine();
        }
    }
    public class HandleUI
    {
        public static void ShowMsg(string msg)
        {
            Console.WriteLine(msg);
        }
        public static void ShowPlayerState()
        {
            foreach(Player p in Process.players){
                if (p == Process.players[Process.CurrentPlayer])
                    Console.WriteLine("*" + p.GetState());
                else
                    Console.WriteLine(p.GetState());
            }
        }
        public static void ShowTableState()
        {
            Console.WriteLine(Table.GetState());
        }
    }
    public class GenVar
    {
        public static string[] CARDNUM = new string[14] { "Jocker", "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        public enum ACTIONTYPE
        {
            Deal, Choose, Check, Follow, Pass, NoCheck
        }
        public enum CARDPATTERN
        {
            H, D, S, C, J, j
        }
        public enum PLAYER
        {
            HUMAN, AI
        }
    }
    
    public class Process
    {
        public static int CurrentPlayer = 0;
        public Deck deck;
        public static List<Player> players = new List<Player>();
        Random r;
        public bool IsEnd
        {
            get
            {
                foreach (Player p in players)
                {
                    if (p.cards.TotalCards == 0)
                    {
                        CurrentPlayer = players.IndexOf(p);
                        return true;
                    }
                        
                }
                return false;
            }
        }
        public Process(int numDeck, GenVar.PLAYER[] pls)
        {
            r = new Random();
            deck = new Deck(numDeck);
            int i = 0;
            foreach (GenVar.PLAYER pl in pls)
            {
                string name;
                if (pl == GenVar.PLAYER.AI)
                    name = "AI-" + i;
                else
                    name = "felix";
                players.Add(new Player(pl, name));
                i++;
            }
        }
        public void Init()
        {
            foreach (Player p in players)
            {
                p.Init();
            }
            Table.Init();
        }
        public void Start()
        {
            Init();
            //deck.Shuffle_Swap();
            deck.Shuffle_Pick();

            HandleUI.ShowMsg("Game Start!");
            int numInitCards = deck.deckShuffled.Count / players.Count;
            foreach (Player p in players)
            {
                p.DrawCard(deck.deckShuffled.GetRange(0, numInitCards));
                deck.deckShuffled.RemoveRange(0, numInitCards);
            }
            if (deck.deckShuffled.Count > 0)
            {
                Table.temporary.AddRange(deck.deckShuffled);
                Table.AllPass();
            }
            HandleUI.ShowMsg("Cards have been drawed!");
            HandleUI.ShowPlayerState();
            HandleUI.ShowMsg("-------------------------------------");
            HandleUI.ShowMsg("Your Cards is: "+players[0].cards.toString());
            CurrentPlayer = 0;
            ChooseNumber();
        }

        public void Draw()
        {

        }
        public void ChooseNumber()
        {
            Table.numThisTurn = players[CurrentPlayer].ChooseNumber();
            HandleUI.ShowMsg(players[CurrentPlayer].name + " has chosen " + GenVar.CARDNUM[Table.numThisTurn] + " for this turn");
            Deal();
        }
        public void Deal()
        {
            Table.PassCount = 0;
            HandleUI.ShowPlayerState();
            HandleUI.ShowTableState();
            HandleUI.ShowMsg("-------------------------------------");
            HandleUI.ShowMsg(players[0].cards.toString());
            players[CurrentPlayer].Deal();
            if (!Check()&&!IsEnd)//if no one checked then ask for following
            {
                Table.NoCheck();
                CurrentPlayer = (CurrentPlayer + 1) % players.Count;

                Follow();
            }
            else if(!IsEnd)//if someone checked then start a new turn!
                ChooseNumber();
        }
        public bool Check()
        {
            int index = (CurrentPlayer + 1) % players.Count;
            while (index != CurrentPlayer)
            {
                if (!players[index].Check())//player not checking
                {
                    HandleUI.ShowMsg(players[index].name + " does not check!");
                    index = (index + 1) % players.Count;
                    Thread.Sleep(2);
                }
                else//player is checking
                        return true;
            }
            return false;

        }
        public void Follow()
        {
            if (players[CurrentPlayer].Follow())
            {
                Deal();
            }
            else
            {
                if (Table.PassCount == Process.players.Count)
                {
                    Table.AllPass();
                    ChooseNumber();
                }
                else
                {
                    CurrentPlayer = (CurrentPlayer + 1) % players.Count;
                    Follow();
                }

            }
        }
        public bool End()
        {
            HandleUI.ShowMsg(players[CurrentPlayer].name + " win! Are you going to replay? Y/N");
            string replay=Console.ReadLine();
            if (replay == "Y" || replay == "y")
                return true;
            return false;
        }
    }
    
    public static class Table
    {
        public static int PassCount = 0;
        public static int numThisTurn = -1;
        private static List<int> passed = new List<int>();
        public static List<int> temporary = new List<int>();
        public static Cards dealed = null;
        public static void Init()
        {
            PassCount = 0;
            numThisTurn = -1;
            passed = new List<int>();
            temporary = new List<int>();
            dealed = null;
        }
        public static string GetState()
        {
            return "Total Passed:" + passed.Count + ", Temp:" + temporary.Count;
        }
        public static void NoCheck()
        {
            temporary.AddRange(dealed.GetAllCards());
            dealed = null;
        }
        public static void AllPass()
        {
            passed.AddRange(temporary);
            temporary.Clear();
            HandleUI.ShowMsg("All Player have passed this turn! ");
        }
        public static List<int> Checked()
        {
            List<int> re = new List<int>();
            re.AddRange(temporary);
            re.AddRange(dealed.GetAllCards());
            temporary.Clear();
            dealed = null;
            numThisTurn = -1;
            return re;
        }
    }

    public class Action
    {
        public Player target;
        public Player actor;
        public GenVar.ACTIONTYPE type;
        public int number;
        public Cards cards;
        public Action(Player target, GenVar.ACTIONTYPE type, int number, Cards cards, Player actor)
        {
            init(target, type, number, cards, actor);
        }
        public Action(GenVar.ACTIONTYPE type, int number, Cards cards, Player actor)
        {
            init(Process.players[Process.CurrentPlayer], type, number, cards, actor);
        }
        public Action(GenVar.ACTIONTYPE type, int number, Player actor)
        {
            init(Process.players[Process.CurrentPlayer], type, number, null, actor);
        }
        public Action(GenVar.ACTIONTYPE type, Player actor)
        {
            init(Process.players[Process.CurrentPlayer], type, -1, null, actor);
        }
        public void init(Player target, GenVar.ACTIONTYPE type, int number, Cards cards, Player actor)
        {
            this.target = target;
            this.type = type;
            this.number = number;
            this.cards = cards;
            this.actor = actor;
        }
        public int Handle()
        {
            switch (type)
            {
                case GenVar.ACTIONTYPE.Check:
                    {
                        if (Table.dealed.IsAll(number))//player not cheated
                        {
                            actor.DrawCard(Table.Checked());
                            HandleUI.ShowMsg(actor.name + " checked " + target.name + " Failed!");
                            HandleUI.ShowMsg("It's " + target.name + "'s turn!");
                            Process.CurrentPlayer = Process.players.IndexOf(target);
                        }
                        else//player cheated
                        {
                            target.DrawCard(Table.Checked());
                            HandleUI.ShowMsg(actor.name + " checked " + target.name + " Succeed!");
                            
                            HandleUI.ShowMsg("It's " + actor.name + "'s turn!");
                            Process.CurrentPlayer = Process.players.IndexOf(actor);
                        }
                        break;
                    }
                case GenVar.ACTIONTYPE.Deal:
                    {
                        HandleUI.ShowMsg(actor.name + ":" + cards.TotalCards + " pieces of " +GenVar.CARDNUM[number]);
                        Table.numThisTurn = number;
                        Table.dealed = cards;
                        break;
                    }
                case GenVar.ACTIONTYPE.Follow:
                    {
                        HandleUI.ShowMsg(actor.name + " is Following");
                        return 1;
                    }
                case GenVar.ACTIONTYPE.Pass:
                    {
                        HandleUI.ShowMsg(actor.name + " pass");
                        Table.PassCount++;
                        return 0;
                    }
                case GenVar.ACTIONTYPE.NoCheck:
                    {
                        break;
                    }
                case GenVar.ACTIONTYPE.Choose:
                    {
                        return 3;
                    }
                default:
                    {
                        break;
                    }
            }
            return 1;
        }
    }
}


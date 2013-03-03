using System;
using System.Collections.Generic;

namespace CardGame
{
    public class Player
    {
        public String name;
        GenVar.PLAYER pl;
        public Cards cards;
        public Player(GenVar.PLAYER pl, string name)
        {
            this.pl = pl;
            this.name = name;
            cards = new Cards(new List<int>());
        }
        public void Init()
        {
            cards = new Cards(new List<int>());
        }
        public void DrawCard(List<int> c)
        {
            cards.AddCard(c);
        }
        public Action makeDesision(GenVar.ACTIONTYPE type)
        {
            switch (type)
            {
                case GenVar.ACTIONTYPE.Check:
                    {
                        Random r = new Random();
                        if (r.Next(100) > 50)
                            return new Action(GenVar.ACTIONTYPE.Check, Table.numThisTurn, this);
                        else
                            return new Action(GenVar.ACTIONTYPE.NoCheck, this);
                    }
                case GenVar.ACTIONTYPE.Deal:
                    {
                        int count=2;
                        return new Action(GenVar.ACTIONTYPE.Deal, Table.numThisTurn, new Cards(cards.GetRandomCards(count)), this);
                    }
                case GenVar.ACTIONTYPE.Follow:
                    {
                        Random r = new Random();
                        if (r.Next(100) > 50)
                            return new Action(GenVar.ACTIONTYPE.Follow, this);
                        else
                            return new Action(GenVar.ACTIONTYPE.Pass, this);
                    }
                case GenVar.ACTIONTYPE.Choose:
                    {
                        return new Action(GenVar.ACTIONTYPE.Choose, this);
                    }
                default:
                    {
                        return null;
                    }
            }
        }
        public int ChooseNumber()
        {
            if (pl == GenVar.PLAYER.HUMAN)
            {
                HandleUI.ShowMsg("Please Choose a number of Card:");
                return int.Parse(Console.ReadLine());
            }
            else
                return makeDesision(GenVar.ACTIONTYPE.Choose).Handle();
        }
        public int Deal()
        {
            Action act;
            if (pl == GenVar.PLAYER.HUMAN)
            {
                HandleUI.ShowMsg("Please select cards to deal ["+GenVar.CARDNUM[Table.numThisTurn]+"]");
                string[] input = Console.ReadLine().Split(',');

                List<int> c = new List<int>();
                foreach (string s in input)
                {
                    int card = int.Parse(s);
                    int index = cards.GetCard(card);
                    if (index != -1)
                        c.Add(index);
                }
                act = new Action( GenVar.ACTIONTYPE.Deal, Table.numThisTurn, new Cards(c), this);
            }
            else
                act = makeDesision(GenVar.ACTIONTYPE.Deal);
            act.Handle();
            return cards.TotalCards;
        }
        public bool Check()
        {
            Action act;
            if (pl == GenVar.PLAYER.HUMAN)
            {
                HandleUI.ShowMsg("Do you want Check? Y/N");
                string s = Console.ReadLine();
                if (s == "Y" || s == "y")
                    act = new Action(GenVar.ACTIONTYPE.Check, Table.numThisTurn, this);
                else
                    act = new Action(GenVar.ACTIONTYPE.NoCheck, this);
            }
            else
                act = makeDesision(GenVar.ACTIONTYPE.Check);
            act.Handle();
            if (act.type == GenVar.ACTIONTYPE.NoCheck)
                return false;
            else
                return true;
        }
        public bool Follow()
        {
            Action act;
            if (pl == GenVar.PLAYER.HUMAN)
            {
                if (Table.PassCount == Process.players.Count - 1)
                {
                    HandleUI.ShowMsg("All other Players have passed");
                }
                HandleUI.ShowMsg("Do you want Folow? Y/N");
                string s = Console.ReadLine();
                if (s == "Y" || s == "y")
                    act = new Action(GenVar.ACTIONTYPE.Follow,this);
                else
                    act = new Action(GenVar.ACTIONTYPE.Pass, this);
            }
            else
                act = makeDesision(GenVar.ACTIONTYPE.Follow);
            return act.Handle()==1;
            
        }
        public string GetState()
        {
            return name+" left ["+cards.TotalCards+"]";
        }
    }
}

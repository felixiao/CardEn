using System;
using System.Collections.Generic;
using System.Text;

namespace CardGame
{
    public class Deck
    {
        public static List<Card> deckSorted = new List<Card>();
        private List<int> sorted = new List<int>();
        private int numDeck = 1;
        public List<int> deckShuffled = new List<int>();

        public Deck(int numDeck)
        {
            this.numDeck = numDeck;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j < 14; j++)
                {
                    deckSorted.Add(new Card(j, (GenVar.CARDPATTERN)i));
                }
            }
            deckSorted.Add(new Card(0, GenVar.CARDPATTERN.J));
            deckSorted.Add(new Card(0, GenVar.CARDPATTERN.j));
        }

        public void Shuffle_Pick()
        {
            deckShuffled.Clear();
            sorted.Clear();
            for (int i = 0; i < numDeck * 54; i++)
            {
                sorted.Add(i%54);
            }
            Random r = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < numDeck * 54; i++)
            {
                int index = r.Next(sorted.Count);
                deckShuffled.Add(sorted[index]);
                sorted.RemoveAt(index);
            }
        }
        public void Shuffle_Swap()
        {
            deckShuffled.Clear();
            for (int i = 0; i < numDeck * 54; i++)
            {
                deckShuffled.Add(i%54);
            }
            Random r = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < deckShuffled.Count; i++)
            {
                int index = r.Next(sorted.Count);
                deckShuffled[index] = deckShuffled[i] + deckShuffled[index];
                deckShuffled[i] = deckShuffled[index] - deckShuffled[i];
                deckShuffled[index] = deckShuffled[index] - deckShuffled[i];
            }
        }
        public void Shuffle_Sort()
        {

        }
        public void Shuffle_Real()
        {
            deckShuffled = new List<int>();
            while (true)
            {

            }
        }

    }
    public class Cards
    {
        private List<int> m_index;
        private List<int>[] m_classified = new List<int>[14];
        public int TotalCards { get { return m_index.Count; } }
        public Cards(List<int> index)
        {
            this.m_index = index;
            for (int i = 0; i < 14; i++)
            {
                m_classified[i] = new List<int>();
            }
            Classify(index);
        }
        public void AddCard(List<int> c)
        {
            m_index.AddRange(c);
            Classify(c);
        }
        public void Classify(List<int> cards)
        {
            foreach (int i in cards)
            {
                m_classified[Deck.deckSorted[i].number].Add(i);
            }
        }
        public List<int> GetAllCards()
        {
            return m_index;
        }
        public int GetRandomCard()//return random index of card in cards
        {
            Random r = new Random();
            int ind=r.Next(m_index.Count);
            int index = m_index[ind];
            m_classified[Deck.deckSorted[index].number].Remove(index);
            m_index.RemoveAt(ind);
            return index;
        }
        public List<int> GetRandomCards(int count)
        {
            List<int> randCards = new List<int>();
            for (int i = 0; i < count;i++ )
                randCards.Add(GetRandomCard());
            return randCards;
        }
        public int GetCard(int num)//return the index of the given number
        {
            if (m_classified[num].Count <= 0 || m_classified[num] == null)
                return -1;
            else
            {
                int index = m_classified[num][0];
                m_classified[num].RemoveAt(0);
                m_index.Remove(index);
                return index;
            }
        }
        public bool IsAll(int num)
        {
            return m_index.Count == m_classified[num].Count;
        }
        public string toString()
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (List<int> cl in m_classified)
            {
                sb.Append(GenVar.CARDNUM[i]+":[" + cl.Count + "], ");
                i++;
            }
            return sb.ToString();
        }
    }
    public class Card
    {
        public int number { get; set; }
        public GenVar.CARDPATTERN pattern { get; set; }
        public string Number
        {
            get{ return GenVar.CARDNUM[number];}
        }
        public string Pattern
        {
            get{ return pattern.ToString(); }
        }
        
        public Card(int number, GenVar.CARDPATTERN pattern)
        {
            this.number = number;
            this.pattern = pattern;
        }

        public string toString()
        {
            return Number+Pattern;
        }
    }

}

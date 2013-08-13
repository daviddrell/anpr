using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationDataClass;


namespace Utilities
{

    public class ThreadControlTable 
    {
        ThreadSafeHashTable table;

        public ThreadControlTable()
        {
            table = new ThreadSafeHashTable(100);
        }
        public int Count
        { get { return table.Count; } }

        public void Add(string key, bool IsStopped)
        {
            table.Add(key, IsStopped);
        }

        public bool this[int index]
        {
            get
            {
                string[] keys = table.Keys;
                return ( (bool) table[keys[index]] );
            }
            set{
                string[] keys = table.Keys;
                this[keys[index]] = value;
            }
        }

        public bool this[string key]
        {
            get
            {
                return (bool)table[key];
            }
            set
            {
                table.Remove(key);
                table.Add(key, value);
            }
        }
       
    }

    public class ThreadSafeQueue<T>

    {

        Queue<T> tsQ;
        object singleton;
        int Limit;
        string queueName = null;
        APPLICATION_DATA.HEALTH_STATISTICS healthStats;

        public ThreadSafeQueue(int limit, string qName, APPLICATION_DATA appData)
        {
            queueName = qName;

            healthStats = (APPLICATION_DATA.HEALTH_STATISTICS) appData.HealthStatistics;

            tsQ = new Queue<T>();
            singleton = new object();
            Limit = limit;
        }

        public ThreadSafeQueue(int limit)
        {
            tsQ = new Queue<T>();
            singleton = new object();
            Limit = limit;
        }

        public int Count
        {
            get
            {
                lock (singleton)
                {
                    return (tsQ.Count);
                }
            }
        }

        public bool Enqueue (T o)
        {
            lock (singleton)
            {

                if (tsQ.Count < Limit)
                {
                    tsQ.Enqueue(o);
                    return (true);
                }
                else
                {
                    if (healthStats != null)
                    {
                        try
                        { 
                            int index = healthStats.GetStatIndexByName(queueName);
                            if ( index != -1 )
                                healthStats[index].HitMe++;
                        }
                        catch (Exception ex) { string s = ex.Message; }
                    }
                    return (false);
                }
            }
        }

        public void Clear()
        {
            lock (singleton)
            {
                tsQ.Clear();
            }
        }

        public T Dequeue()
        {
            T o = default(T);

            lock (singleton)
            {
                if (tsQ.Count > 0)
                {
                    o = tsQ.Dequeue();
                    return (o);
                }
            }
            return (o);
        }
    }
   
    public class ThreadSafeHashTable
    {
        

        Hashtable ht;
        object singleton;
        int Limit;

        public ThreadSafeHashTable(int limit)
        {
            ht = new Hashtable();
            singleton = new object();
            Limit = limit;
        }

        public string[] Keys
        {
            get
            {
                lock(singleton)
                {
                    string [] keys = new string[ht.Keys.Count];
                    ht.Keys.CopyTo(keys, 0);
                    return (keys);
                }
            }
        }

        public int Count
        {
            get
            {
                lock (singleton)
                {
                    return (ht.Count);
                }
            }

        }

        public void Remove(object key)
        {
            if ( ht.Contains(key))
                ht.Remove(key);
        }

        public bool Contains(object key)
        {
            lock (singleton)
            {
                return (ht.Contains(key));
            }
        }

        public bool Add (object key, object val)
        {
            lock (singleton)
            {
                if (ht.Count == Limit)
                {
                    IEnumerator myEnum = ht.GetEnumerator();
                    ht.Remove(myEnum.Current);

                }
                if (ht.Count < Limit && !ht.Contains(key))
                {

                    ht.Add(key, val);
                    return (true);

                }
                else
                {
                    return (false);
                }
            }
        }

        public void Clear()
        {
            lock (singleton)
            {
                ht.Clear();
            }
        }

        public object this [object key]
        {
            get { lock (singleton) { if (ht.Contains(key)) return (ht[key]); else return (default(object)); } }
        }


    }


    /// <summary>
    /// This allows look up like a hash table, but removes the first in when a new oject is added and the table is at its limit.
    /// This allows adding ad infinitum - a table that allows the fast lookup of the most recent N items.
    /// </summary>
  
    public class ThreadSafeHashableQueue
    {
        

        Hashtable ht;
        object singleton;
        int Limit;
        ThreadSafeQueue<object> matchingQ;

        public ThreadSafeHashableQueue(int limit)
        {
            ht = new Hashtable();
            singleton = new object();
            Limit = limit;
            matchingQ = new ThreadSafeQueue<object>(limit);
        }

        public string[] Keys
        {
            get
            {
                lock(singleton)
                {
                    string [] keys = new string[ht.Keys.Count];
                    ht.Keys.CopyTo(keys, 0);
                    return (keys);
                }
            }
        }

        public int Count
        {
            get
            {
                lock (singleton)
                {
                    return (ht.Count);
                }
            }

        }

      

        public bool Contains(object key)
        {
            lock (singleton)
            {
                return (ht.Contains(key));
            }
        }

        public bool Add (object key, object val)
        {
            lock (singleton)
            {

                if (ht.Count == Limit)
                {
                    object keyToRemove = matchingQ.Dequeue();
                    if (keyToRemove != null)
                    {
                        ht.Remove(keyToRemove);
                    }
                }
                if (ht.Count < Limit && !ht.Contains(key))
                {
                    matchingQ.Enqueue(key);
                    ht.Add(key, val);
                    return (true);
                }
                else
                {
                    return (false);
                }
            }
        }

        public void Clear()
        {
            lock (singleton)
            {
                ht.Clear();
                matchingQ.Clear();
            }
        }

        public object this [object key]
        {
            get { lock (singleton) { if (ht.Contains(key)) return (ht[key]); else return (default(object)); } }
        }


    }






    public class ThreadSafeList<T> : IEnumerable<T>
    {
         
        List<T> list;
        object singleton;
        int Limit;

        public ThreadSafeList(int limit)
        {
            list = new List<T>();
            singleton = new object();
            Limit = limit;
        }

        public  void Remove (T item)
        {
            lock (singleton)
            {
                if (list.Contains(item))
                    list.Remove(item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (singleton)
            {
                if (index >= 0 && index < list.Count)
                    list.RemoveAt(index);
            }
        }

        public int Count
        {
            get
            {
                lock (singleton)
                {
                    return (list.Count);
                }
            }

        }

        public bool Contains(T val)
        {
            lock (singleton)
            {
                return (list.Contains((T)val));
            }
        }

        public bool Add ( T val)
        {
            lock (singleton)
            {
                if (list.Count < Limit && !list.Contains((T)val))
                {

                    list.Add((T)val);
                    return (true);
                   
                }
                else
                    return (false);
            }
        }

        public void Clear()
        {
            lock (singleton)
            {
                list.Clear();
            }
        }

        public T this [int index]
        {
            get { lock (singleton) { return (list[index]); } }
        }

        //Implementation of method GetEnumerator of IEnumerable interface
        public virtual IEnumerator<T> GetEnumerator()
        {
            lock (singleton)
            {
                return new ItemIterator<T>(list);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class ItemIterator<J> : IEnumerator<J>
        {
            List<J> m_List;
            int index = -1;
            public ItemIterator(List<J> list)
            {
                m_List = list;
            }
            object IEnumerator.Current
            {
                get
                {
                    if (index < 0 || index >= m_List.Count)
                        throw new InvalidOperationException();

                    return (m_List[index]);
                } 
            }
            public J Current 
            { 
                get
                {
                    if (index < 0 || index >= m_List.Count)
                        throw new InvalidOperationException();

                    return (m_List[index]);
                } 
            }
            public bool MoveNext()
            {
                if (index < m_List.Count) 
                    index++;
                if (index == m_List.Count) return (false);
                else return (true);
            }
            public void Reset()
            { index = -1; }

            public void Dispose()  {}


        }
        
    }




    //File ItemCollection.cs

    //Class ItemCollection implements IEnumerable interface
    class ThreadSafeList : IEnumerable
    {
        String[] itemId;
        //Constructor to create and populate itemId String array
        public ThreadSafeList(int noOfItem)
        {
            itemId = new String[noOfItem];
            for (int i = 0; i < itemId.Length; i++)
            {
                itemId[i] = i.ToString();
            }
        }
        //Implementation of method GetEnumerator of IEnumerable interface
        public virtual IEnumerator GetEnumerator()
        {
            return new ItemIterator(this);
        }
        //Inner class ItemIterator, implements IEnumerator
        public class ItemIterator : IEnumerator
        {
            //Declare a variable of type ItemCollection,
            //to keep reference to enclosing class instance
            private ThreadSafeList itemCollection;
            //Declare a integer pointer and Set to -1, so that
            //first call to MoveNext moves the enumerator over
            //the first element of the collection.
            private int index = -1;
            //Pass an instance of enclosing class
            public ItemIterator(ThreadSafeList ic)
            {
                //Save enclosing class reference
                itemCollection = ic;
            }
            //After an enumerator is created or after a Reset,
            //an enumerator is positioned before the first element
            //of the collection, and the first call to MoveNext
            //moves the enumerator over the first element of the
            //collection.
            public bool MoveNext()
            {
                index++;
                if (index < itemCollection.itemId.Length)
                {
                    return true;
                }
                else
                {
                    index = -1;
                    return false;
                }
            }
            //Return the current object, in our case Item Id string
            //from itemId[] array. Throws InvalidOperationException exception
            //if index pointing to wrong position
            public object Current
            {
                get
                {
                    if (index <= -1)
                    {
                        throw new InvalidOperationException();
                    }
                    return itemCollection.itemId[index];
                }
            }
            //Reset pointer to -1
            public void Reset()
            {
                index = -1;
            }
        }
     
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PartyInc
{
    public static class Stt_TADHelpers
    {
        public static T Remove<T, P>(T tad, P toRemove)
        {
            T ret = default(T);
            System.Type type = typeof(T);
            System.Type queueType = typeof(Queue<P>);
            System.Type stackType = typeof(Stack<P>);

            if (type == queueType)
            {
                object aux = tad;
                Queue<P> q = (Queue<P>)aux;
                Queue<P> aux2 = new Queue<P>();

                while (q.Count > 0)
                {
                    P elem = q.Dequeue();
                    if (!elem.Equals(toRemove))
                    {
                        aux2.Enqueue(elem);
                    }
                }

                ret = (T)(object)aux2;
            }

            if(type == stackType)
            {
                // Implement for this, etc
            }

            if (ret.Equals(default(T)))
            {
                Debug.LogError("PartyInc/Stt_TADHelpers/Remove: ERROR: Type not supported.");
            }

            return ret;
        }

        public static Queue<P> ToQueue<P>(List<P> list)
        {
            // We assume the first element in the list is the first item in the queue
            Queue<P> ret = new Queue<P>();
            foreach(P p in list)
            {
                ret.Enqueue(p);
            }
            return ret;
        }

        public static Queue<LL.FPSync> Order(Queue<LL.FPSync> q)
        {
            Queue<LL.FPSync> q2 = new Queue<LL.FPSync>();
            foreach (LL.FPSync i in q.OrderBy(x => x.timeSent))
                q2.Enqueue(i);
            return q2;
        }
    }
}



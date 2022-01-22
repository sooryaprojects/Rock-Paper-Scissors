using System.Collections.Generic;

public class KVPList<Tkey, TValue> : List<KeyValuePair<Tkey, TValue>>
{
    public void Add(Tkey key, TValue value)
    {
        base.Add(new KeyValuePair<Tkey, TValue>(key, value));
    }

    public TValue GetValueOf(KVPList<Tkey, TValue> kvpList, Tkey key)
    {
        for (int i = 0; i < kvpList.Count; i++)
        {
            if (kvpList[i].Key.Equals(key))
            {
                return kvpList[i].Value;
            }
        }
        return default(TValue);
    }

    public static bool Compare<T>(T x, T y) where T : class
    {
        return x == y;
    }


}


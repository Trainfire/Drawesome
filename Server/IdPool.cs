using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class IdPool
    {
        List<uint> AvailableIDs { get; set; }

        public IdPool(int size)
        {
            AvailableIDs = new List<uint>();
            for (int i = 0; i < size; i++)
            {
                AvailableIDs.Add((uint)i);
            }
        }

        public uint GetValue()
        {
            var id = AvailableIDs.First();
            AvailableIDs.Remove(id);
            return id;
        }

        public void ReturnValue(uint id)
        {
            if (!AvailableIDs.Contains(id))
                AvailableIDs.Add(id);
        }
    }
}

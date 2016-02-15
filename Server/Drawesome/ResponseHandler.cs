using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Drawesome
{
    public class ResponseHandler<T>
    {
        Dictionary<T, bool> Respondants { get; set; }

        public ResponseHandler()
        {
            Respondants = new Dictionary<T, bool>();
        }

        public void Add(T respondant)
        {
            if (!Respondants.ContainsKey(respondant))
                Respondants.Add(respondant, false);
        }

        public void Register(T respondant)
        {
            if (Respondants.ContainsKey(respondant))
                Respondants[respondant] = true;
        }

        public bool HaveAllRespondend()
        {
            return Respondants.All(x => x.Value == true);
        }

        public void Clear()
        {
            Respondants.Clear();
        }
    }
}

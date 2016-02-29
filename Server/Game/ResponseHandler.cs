using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class ResponseHandler<T>
    {
        Dictionary<T, bool> Respondants { get; set; }

        public ResponseHandler()
        {
            Respondants = new Dictionary<T, bool>();
        }

        public void AddRespondant(T respondant)
        {
            if (!Respondants.ContainsKey(respondant))
                Respondants.Add(respondant, false);
        }

        public void RegisterResponse(T respondant)
        {
            if (Respondants.ContainsKey(respondant) && Respondants[respondant] == false)
                Respondants[respondant] = true;
        }

        public bool AllResponded()
        {
            return Respondants.All(x => x.Value == true);
        }

        public void Clear()
        {
            Respondants.Clear();
        }
    }
}

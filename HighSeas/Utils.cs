using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace HighSeas
{
    public static class Utils
    {
        public static RagdollPart GetRandomRagdollPart(this Creature creature, int mask = 0b00011111111111)
        {
            Array values = Enum.GetValues(typeof(RagdollPart.Type));
            RagdollPart.Type[] type = new RagdollPart.Type[values.Length];
            type.CopyTo(values, 0);
            List<RagdollPart.Type> list = type.Where(part => (mask & (int)part) > 0).ToList();
            return creature.ragdoll.GetPart(list[UnityEngine.Random.Range(0, list.Count())]);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyRoundPlugin
{
    class Randomizer
    {
        Random rng = new Random();

        int[] probs;
        public Randomizer(int max)
        {
            probs = new int[max];
            for(int i = 0; i < max; i++)
            {
                probs[i] = 1;
            }
        }

        public int PickRandom()
        {
            int sum = 0;
            for(int x = 0; x < probs.Length; x++)
            {
                sum += probs[x];
            }
            int value = rng.Next(sum);
            int result = -1;
            int i = 0;
            int build = 0;
            while(i < probs.Length)
            {
                if (build + probs[i] >= value && result == -1)
                {
                    result = i;
                    build += probs[i];
                    probs[i] = 1;
                } else
                {
                    build += probs[i];
                    probs[i] += 1;
                }
                i++;
            }
            return result;
        }
    }
}

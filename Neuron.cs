using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neuron
{

    class Neuron
    {
        public List<float> weight;
        float threshold;

        public int Activate(List<int> inp)
        {
            if (inp.Count == weight.Count)
            {
                float t = 0;
                for (int i = 0; i < inp.Count; i++)
                {
                    t = t + inp[i] * weight[i];
                }
                if (t >= threshold)
                    return 1;
            }
            return 0;
        }

        Neuron(List<float> weights, float t)
        {
            weight = weights;
            threshold = t;
        }

        Neuron()
        {
            weight = new List<float>();
        }
    }

    class NeuralNetwork
    {
        public List<List<Neuron>> neuron;
        public List<List<List<int>>> next;
        public List<List<List<int>>> prew;

        public NeuralNetwork()
        {
            neuron = new List<List<Neuron>>();
        }

        public NeuralNetwork(List<List<Neuron>> neu, List<List<List<int>>> nxt, List<List<List<int>>> prw)
        {
            neuron = neu;
            next = nxt;
            prew = prw;
        }

        public void InsertLayer(int n, List<Neuron> neu, List<List<int>> prw, List<List<int>> nxt)
        {
            neuron.Insert(n, neu);
            next.Insert(n, nxt);
            prew.Insert(n, prw);
            for (int i = 0; i < prew[n + 1].Count; i++)
            {
                prew[n + 1][i].Clear();
            }
            for (int i = 0; i < nxt.Count; i++)
            {
                for (int j = 0; j < nxt[i].Count; j++)
                {
                    prew[n + 1][nxt[i][j]].Add(i);
                }
            }
            if (n > 0)
            {
                for (int i = 0; i < next[n - 1].Count; i++)
                {
                    next[n - 1][i].Clear();
                }
                for (int i = 0; i < prw.Count; i++)
                {
                    for (int j = 0; j < prw[i].Count; j++)
                    {
                        next[n - 1][prw[i][j]].Add(i);
                    }
                }
            }
        }

        public List<int> Activate(List<int> inp)
        {
            List<int> r = new List<int>();
            if (inp.Count == neuron[0].Count)
            {
                r = inp;
                for (int i = 0; i < neuron.Count; i++)
                {
                    List<int> temp = r;
                    r.Clear();
                    for (int j = 0; j < neuron[i].Count; j++)
                    {
                        r.Add(neuron[i][j].Activate(temp));
                    }
                }
            }
            return r;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            NeuralNetwork test = new NeuralNetwork();
        }
    }
}

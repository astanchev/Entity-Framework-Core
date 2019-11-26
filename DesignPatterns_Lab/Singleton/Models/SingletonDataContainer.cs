namespace Singleton.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Contracts;

    public sealed class SingletonDataContainer : ISingletonContainer
    {
        private static Dictionary<string, int> capitals = new Dictionary<string, int>();
        private static volatile SingletonDataContainer instance = new SingletonDataContainer();

        protected SingletonDataContainer()
        {
            Console.WriteLine("Initializing singleton object");

            //var elements = File.ReadAllLines("../../capitals.txt");

            //for (int i = 0; i < 3/*elements.Length*/; i += 2)
            //{
            //    capitals.Add(elements[i], int.Parse(elements[i + 1]));
            //}

            capitals.Add("Washington, D.C.", 633427);
            capitals.Add("London", 8908081);
            capitals.Add("Sofia", 2500000);
        }

        public static SingletonDataContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (capitals)
                    {
                        if (instance == null)
                        {
                            instance = new SingletonDataContainer();
                        }
                    }
                }

                return instance;
            }
        }

        public int GetPopulation(string name)
        {
            return capitals[name];
        }
    }
}
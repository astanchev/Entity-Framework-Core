namespace Singleton
{
    using System;
    using Models;

    class StartUp
    {
        static void Main()
        {
            var db = SingletonDataContainer.Instance;
            Console.WriteLine(db.GetPopulation("Washington, D.C."));
            var db2 = SingletonDataContainer.Instance;
            Console.WriteLine(db.GetPopulation("London"));
            var db3 = SingletonDataContainer.Instance;Console.WriteLine(db.GetPopulation("Sofia"));
        }
    }
}

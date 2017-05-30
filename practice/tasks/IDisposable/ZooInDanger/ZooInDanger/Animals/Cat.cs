using System;

namespace Zoo.Animals
{
    public class Cat : Animal
    {
        public Cat(IAnimalStatusTracker statusTracker) : base(statusTracker)
        {
        }

        public override int LifeInterval
        {
            get { return 13; }
        }

        public override int InfectionDeathInterval
        {
            get { return 300; }
        }

        ~Cat()
        {
            Logger.LogYellow("Finalizing cat!");

            //this check leads to issues with garbage collector
            //after "Numcorpses" is more than 200 we can not finish this finalizer
            //"Numcorpses" is decrementing in parent class -> Animal
            //"Interlocked.Decrement(ref Zoo.NumCorpses);"

            // the fix is just to skip this check
            
            // Release the object only in case number of corpses > 100
            //while (Zoo.NumCorpses > 200) { }
        }
    }
}
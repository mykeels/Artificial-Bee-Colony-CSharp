using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abc
{
    public partial class Hive<FoodType>
    {
        public class BeeLahc<FoodSource> : Bee<FoodSource>
        {
            protected override int _timeSinceLastImprovement { get; set; }
            protected override int _nonImprovementLimit { get; set; }
            protected override Func<FoodSource, FoodSource> _mutationFunc { get; set; }
            protected override Func<FoodSource, double> _fitnessFunc { get; set; }
            private LAHC lahc { get; set; }
            public bool useLahc { get; set; }

            public BeeLahc()
            {
                this.Fitness = double.MaxValue;
                this.defaultFitness = double.MaxValue;
                this.Type = TypeClass.Scout;
                this.lahc = new LAHC(20, this.Fitness, Search.Direction.Optimization);
            }

            public BeeLahc(Func<FoodSource, FoodSource> mFunc, Func<FoodSource, double> fFunc, TypeClass _type,
                int ID = 0, int _failureLimit = 20, int _tableSize = 50)
            {
                if ((Hive<FoodSource>.Movement == Search.Direction.Divergence))
                {
                    this.Fitness = double.MinValue;
                    defaultFitness = double.MinValue;
                }
                else if (Hive<FoodSource>.Movement == Search.Direction.Optimization)
                {
                    this.Fitness = double.MaxValue;
                    defaultFitness = double.MaxValue;
                }
                if (mFunc == null | fFunc == null)
                {
                    throw new Exception(string.Format("Ogbeni, na Bee #{0} be this. How i wan take mutate na?", ID));
                }
                this._nonImprovementLimit = _failureLimit;
                this._mutationFunc = mFunc;
                this._fitnessFunc = fFunc;
                this.Type = _type;
                this.lahc = new LAHC(_tableSize, this.Fitness, Hive<FoodSource>.Movement);
            }

            public override FoodSource Mutate()
            {
                FoodSource ret = _mutationFunc(this.Food);
                double _fitness = Bee<FoodSource>.GetFitness(ret, _fitnessFunc);
                if (this.Fitness.Equals(defaultFitness) || _fitness < this.Fitness || (useLahc && this.lahc.Update(_fitness)))
                {
                    this.Fitness = _fitness;
                    this.Food = ret;
                    this._timeSinceLastImprovement = 0;
                }
                else
                {
                    this._timeSinceLastImprovement += 1;
                    //Solution has not improved
                    if (this._timeSinceLastImprovement >= this._nonImprovementLimit)
                    {
                        this.ChangeToOnlooker();
                    }
                }
                return this.Food;
            }

        }
    }
}

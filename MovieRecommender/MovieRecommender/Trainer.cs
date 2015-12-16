using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender
{
    class Trainer
    {
        double alfa = 0.065;

        //series of x values and an y value at the end (x1, x2, x3, ... , y)
        public Double[][] trainSetElements;
        //(p0, p1, p2, ...)
        public Double[] parameters;

        public void train(){
            int N = 1000; //nr of iterations

            int iter = 0;
            double error = 1.0;
            double x;
            bool proceed = true;

            while(iter < N && proceed) {
                Double[] oldParameters = (Double[])parameters.Clone();

                foreach (Double[] set in trainSetElements) {
                    for (int i = 0; i < parameters.Count(); i++) {
                        error = polynomialValue(set, parameters) - set[set.Count()-1];
                        x = (i == 0 ? 1 : set[i-1]);
                        parameters[i] = parameters[i] - alfa * error * x / parameters.Count();
                    }
                }
                for(int i = 0; i < parameters.Count(); i++){
                    if(Math.Abs(oldParameters[i] - parameters[i]) < 0.0001){ proceed = false; }
                }
                iter++;
            }

        }

        public double polynomialValue(Double[] _set, Double[] _parameters){
            double result = 0.0;

            for(int i = 0; i < _set.Count() - 1; i++){
                result += _set[i] * _parameters[i+1];
            }

            result += _parameters[0];

            return result;
        }
    }
}

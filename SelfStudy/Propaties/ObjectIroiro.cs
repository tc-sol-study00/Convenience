using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStudy.Propaties {

    public class Animal : IAnimal {
        public string Name { get; set; }

        public bool Mocho { get; set; }


    }
    public class Bird : Animal, IAnimal{
        public decimal TondaKyori { get; set; }
    }

    public class HachuRui : Animal, IAnimal, IHachuRui {
        public string Hadairo { get; set; }
    }

    public interface IAnimal {
        public string Name { get; set; }

        //public string TokuCho { get; }
    }

    public interface IHachuRui {
        public string Name { get; set; }

        public string Hadairo { get; set; }
    }

    internal class ObjectIroiro {

        public void MainProcess() {
            Animal animal = Kyohensei();

            HanpenSei(new Bird());

            IAnimal admin= new Animal();
            IAnimal bird= new Bird();

            HachuRui hachuRui = new HachuRui();
            var mocho=hachuRui.Mocho;

            IHachuRui hachuRui2 = new HachuRui();
            //Mochoは見えない

        }
        Animal Kyohensei() {
            return new Bird();
        }

         void HanpenSei (Bird animal) {
            Console.WriteLine(animal.Name);
        }
    }
}

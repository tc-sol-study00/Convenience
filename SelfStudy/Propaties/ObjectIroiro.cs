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
    public class Bird : Animal, IAnimal {
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

            IAnimal admin = new Animal();
            IAnimal bird = new Bird();

            HachuRui hachuRui = new HachuRui();
            var mocho = hachuRui.Mocho;

            IHachuRui hachuRui2 = new HachuRui();
            //Mochoは見えない

        }
        Animal Kyohensei() {
            return new Bird();
        }

        void HanpenSei(Animal animal) {
            Console.WriteLine(animal.Name);
        }
    }

    /// <summary>
    /// 移譲（集約）と合成（コンポジット）
    /// </summary>
    public class DelegateCompositService {

        Animal animal { get; set; }
        public DelegateCompositService() {
            Animal animal = new Animal();
        }

        public void DelegateCompositServiceExecution() {

            DelegateCompositProperty delegateCompositProperty = new DelegateCompositProperty(animal);

            delegateCompositProperty = null;

            //delegateCompositPropertyのBirdプロパティは親と一緒に消えている

            //移譲で渡したanimalはそのまま生きている
            var xxx = animal;

        }
    }

    public class DelegateCompositProperty {

        Animal Animal { get; set; }
        Bird Bird { get; set; }
        public DelegateCompositProperty(Animal inAnimal) {
            this.Animal = inAnimal; //移譲
            this.Bird = new Bird(); //合成
        }

        ~DelegateCompositProperty() {
            Console.WriteLine("I named DelegateCompositProperty has been dead !");
        }

    }

}


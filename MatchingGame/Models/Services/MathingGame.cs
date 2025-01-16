using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MachingGame.Models.Abstracts;

namespace MachingGame.Models.Services {
    public class MathingGame : AMathingGame {

        protected override void Display(string title) {

            Action<int> displayHeader = length => Enumerable.Range(0, length).ToList().ForEach(x => Console.Write("●〇"));
            Action cr = () => Console.WriteLine();
            // 上部の装飾
            displayHeader(5); cr();
            // 中央部分の装飾とタイトル表示
            displayHeader(1); Console.Write(title); displayHeader(1); cr();
            // 下部の装飾
            displayHeader(5); cr();
        }
    }
}

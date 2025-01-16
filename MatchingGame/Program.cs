using System;
using System.ComponentModel;
using MachingGame.Interfaces;
using MachingGame.Models.Abstracts;
using MachingGame.Models.Services;

namespace MachingGame {

    /// <summary>
    /// メイン
    /// </summary>
    internal class Program {

        private static readonly IMathingGame _mathingGame = new MathingGame();
        private static readonly IMathingGame _mathingGameNew = new MathingGameNew();

        private enum Status {
            MATCHINGGAME,
        }

        private static Status selectSwitch = Status.MATCHINGGAME;

        static void Main() {

            switch (selectSwitch) {
                case Status.MATCHINGGAME:
                    _mathingGame.GamePlay();
                    _mathingGameNew.GamePlay();
                    break;
                default:
                    throw new InvalidEnumArgumentException("実行スイッチが不正です");
            }
        }

    }
}
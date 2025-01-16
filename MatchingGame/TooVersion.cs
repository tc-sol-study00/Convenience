using System;
using System.Linq;
using System.Collections.Generic;

namespace Tabuchi20250114 {


    //Program.cs
    //（合成）　
    //IDisplay consoleDisplay = new ConsoleDisplay();
    //IInputHandler inputHandler = new InputHandler();
    //IRandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();
    //IPlayGame playGame = new PlayGame();

    // 乱数生成インターフェース
    public interface IRandomNumberGenerator {
        // ランダムな3桁の数字を生成するメソッド
        string GenerateUniqueThreeDigitNumber();
    }

    // 表示インターフェース
    public interface IDisplay {
        // メッセージを装飾して表示するメソッド
        void Display(string title);
    }

    // 入力インターフェース
    public interface IInputHandler {
        // ユーザーからの3桁の数字入力を受け付けるメソッド
        string UserInput();
    }

    // ゲームプレイインターフェース
    public interface IPlayGame {
        // ユーザーが入力した数字と乱数を比較して結果を表示するメソッド
        void UserPlayGame(string randomNumber, string userInputNumber);
    }

    // 乱数生成クラス
    public class RandomNumberGenerator : IRandomNumberGenerator {
        private Random random = new Random();

        public string GenerateUniqueThreeDigitNumber() {
            // 0から9の範囲で重複しない3つの数字をランダムに選ぶ
            var digits = Enumerable.Range(0, 10).OrderBy(x => random.Next()).Take(3).ToList();

            // 選ばれた数字を文字列として結合して返す
            return string.Join("", digits);
        }
    }

    // コンソール表示クラス
    public class ConsoleDisplay : IDisplay {
        // 指定された数だけ「●」と「〇」を交互に表示するメソッド
        private void PrintSymbols(int count) {
            for (int i = 0; i < count; i++) {
                Console.Write("●");
                Console.Write("〇");
            }
        }

        public void Display(string title) {
            // 上部の装飾
            PrintSymbols(5);
            Console.WriteLine();  // 改行

            // 中央部分の装飾とタイトル表示
            PrintSymbols(1);
            Console.Write(title);  // タイトル
            PrintSymbols(1);
            Console.WriteLine();  // 改行

            // 下部の装飾
            PrintSymbols(5);
            Console.WriteLine();  // 改行
        }
    }



    // 入力処理クラス
    public class InputHandler : IInputHandler {
        public string UserInput() {
            string userinput;

            while (true) {
                Console.WriteLine("3桁の数字を入力してください！（例: 012）");
                Console.WriteLine("※数字は重複なし！");
                userinput = Console.ReadLine();  // ユーザー入力を取得


                // 入力が3文字かつすべて数字であり、重複がない場合
                if (userinput.Length == 3                   //入力が3文字
                    && userinput.All(char.IsDigit)          //入力が全て数字
                    && userinput.Distinct().Count() == 3) { //入力された数字に重複なし

                    return userinput;  // 正常に入力された場合、そのまま返す
                }

                // 入力が正しくない場合はエラーメッセージを表示
                Console.WriteLine("入力が正しくありません！3桁で重複なしの数字を入力してください！");
            }
        }
    }



    // ゲームプレイクラス
    public class PlayGame : IPlayGame {
        public void UserPlayGame(string randomNumber, string userInputNumber) {
            // 入力された数字と乱数をリストに変換
            var userNumberList = userInputNumber.Select(digit => int.Parse(digit.ToString())).ToList();
            var randomNumberList = randomNumber.Select(digit => int.Parse(digit.ToString())).ToList();

            int exactMatchCount = 0;  // 完全一致のカウント
            int partialMatchCount = 0;  // 部分一致のカウント

            // 桁ごとの比較
            for (int i = 0; i < 3; i++) {
                if (userNumberList[i] == randomNumberList[i]) {
                    exactMatchCount++;  // 桁と数字が一致した場合
                }
                else if (randomNumberList.Contains(userNumberList[i])) {
                    partialMatchCount++;  // 数字のみ一致した場合
                }
            }

            // 結果を表示
            Console.WriteLine($"完全一致: {exactMatchCount}個");
            Console.WriteLine($"数字一致: {partialMatchCount}個");
        }
    }


    internal class Program {
        static void Main(string[] args) {
            // 各インターフェースのインスタンスを生成
            IDisplay consoleDisplay = new ConsoleDisplay();
            IInputHandler inputHandler = new InputHandler();
            IRandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();
            IPlayGame playGame = new PlayGame();

            // ゲーム開始時の装飾とメッセージ表示
            consoleDisplay.Display("ゲーム開始");

            // ランダムな3桁の数字を生成
            string randomNumber = randomNumberGenerator.GenerateUniqueThreeDigitNumber();

            // デバッグ用：生成された乱数を表示
            Console.WriteLine($"デバッグ用 正解は：{randomNumber}");

            string userInput;

            // ゲームのメインループ
            while (true) {
                // ユーザーからの入力を受け付け
                userInput = inputHandler.UserInput();

                // ゲームロジックを実行
                playGame.UserPlayGame(randomNumber, userInput);

                // 完全一致した場合、ゲーム終了
                if (randomNumber == userInput) {
                    Console.WriteLine("正解です！");
                    break;
                }
                else {
                    Console.WriteLine("再度挑戦してください！");
                }
            }

            // ゲーム終了時の装飾とメッセージ表示
            consoleDisplay.Display("ゲーム終了！");
        }
    }
}
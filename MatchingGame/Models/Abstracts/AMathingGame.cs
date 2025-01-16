using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MachingGame.Interfaces;
using MachingGame.Models.Properties;

namespace MachingGame.Models.Abstracts {
    /// <summary>
    /// マッチングゲームクラス
    /// </summary>
    public abstract class AMathingGame : IMathingGame {

        private const int DATALENGTH = 3;
        public IRandomNumberGenerator RandomNumberGenerator { get; private set; }

        public string UserNumber { get; private set; } = string.Empty;
        public string RandomNumber { get; private set; } = string.Empty;
        public int ExactMatchCount { get; private set; } = 0;  // 完全一致のカウント
        public int PartialMatchCount { get; private set; } = 0;  // 部分一致のカウント
        public AMathingGame() {
            RandomNumberGenerator = new RandomNumberGenerator(DATALENGTH);
        }

        //具象メソッド
        public bool GamePlay() {

            // ゲーム開始時の装飾とメッセージ表示
            Display(" ゲーム開始 ");

            // ランダムな3桁の数字を生成
            string RandomNumber = RandomNumberGenerator.GenerateUniqueThreeDigitNumber();

            // デバッグ用：生成された乱数を表示
            Console.WriteLine($"デバッグ用 正解は：{RandomNumber}");

            // ゲームのメインループ
            while (true) {
                try {
                    // ユーザーからの入力を受け付け
                    UserNumber = UserInput(DATALENGTH);
                }
                catch (OperationCanceledException e) {
                    Console.WriteLine(e.Message);
                    return false;
                }
                // 答え合わせ
                (ExactMatchCount, PartialMatchCount) = HitJudgement(RandomNumber, UserNumber, this.RandomNumberGenerator.DataLength);

                // 結果を表示
                Console.WriteLine($"完全一致: {ExactMatchCount}個");
                Console.WriteLine($"数字一致: {PartialMatchCount}個");

                // 完全一致した場合、ゲーム終了
                if (RandomNumber == UserNumber) {
                    Console.WriteLine("正解です！");
                    break;
                }
                else {
                    Console.WriteLine("再度挑戦してください！");
                }
            }

            // ゲーム終了時の装飾とメッセージ表示
            Display("ゲーム終了！");
            return true;
        }

        /// <summary>
        /// 結果判定
        /// </summary>
        /// <param name="randomNumber">乱数の値</param>
        /// <param name="userInputNumber">入力された値</param>
        /// <param name="DataLength">問題の数</param>
        /// <returns>(ExactMatchCount, PartialMatchCount)</returns>
        private (int, int) HitJudgement(string randomNumber, string userInputNumber, int DataLength) {
            // 入力された数字と乱数をリストに変換
            var userNumberList = userInputNumber.Select(digit => int.Parse(digit.ToString())).ToList();
            var randomNumberList = randomNumber.Select(digit => int.Parse(digit.ToString())).ToList();

            ExactMatchCount = 0;  // 完全一致のカウント
            PartialMatchCount = 0;  // 部分一致のカウント

            // 桁ごとの比較
            for (int i = 0; i < DataLength; i++) {
                if (userNumberList[i] == randomNumberList[i]) {
                    ExactMatchCount++;  // 桁と数字が一致した場合
                }
                else if (randomNumberList.Contains(userNumberList[i])) {
                    PartialMatchCount++;  // 数字のみ一致した場合
                }
            }
            return (ExactMatchCount, PartialMatchCount);
        }

        /// <summary>
        /// 入力処理
        /// </summary>
        /// <returns>入力された文字</returns>
        /// <exception cref="OperationCanceledException">中断用例外</exception>
        protected string UserInput(int DataLength) {
            string? userinput;

            while (true) {
                Console.WriteLine("3桁の数字を入力してください！（例: 012）");
                Console.WriteLine("※数字は重複なし！");
                userinput = Console.ReadLine();  // ユーザー入力を取得

                if (userinput == "e" || userinput == "E" || string.IsNullOrEmpty(userinput))
                    throw new OperationCanceledException("中断します！");

                // 入力が3文字かつすべて数字であり、重複がない場合
                if (userinput.Length != DataLength    //入力が3文字じゃないか
                    || !userinput.All(char.IsDigit)                         //入力が全て数字じゃないか
                    || userinput.Distinct().Count() != DataLength) {
                    //入力された数字に重複がない場合

                    // 入力が正しくない場合はエラーメッセージを表示
                    Console.WriteLine("入力が正しくありません！3桁で重複なしの数字を入力してください！");
                    continue;
                }
                //Check　OKの場合は入力値を返す
                //最終行にreturnを置いたほうが分かりやすい
                return userinput;  // 正常に入力された場合、そのまま返す
            }
        }

        //抽象メソッド
        protected abstract void Display(string title);

    }
}

using MachingGame.Interfaces;

namespace MachingGame.Models.Properties {
    /// <summary>
    /// 乱数クラス
    /// </summary>
    public class RandomNumberGenerator : IRandomNumberGenerator {
        public IList<int> RamdomDigitList { get; private set; }
        public int DataLength { get; private set; }
        private readonly Random random;

        public RandomNumberGenerator(int DataLength) {
            this.DataLength = DataLength;
            random = new Random();
            RamdomDigitList = new List<int>();
        }

        public string GenerateUniqueThreeDigitNumber() {
            // 0から9の範囲で重複しない3つの数字をランダムに選ぶ
            this.RamdomDigitList = Enumerable.Range(0, 10).OrderBy(x => random.Next()).Take(DataLength).ToList();

            // 選ばれた数字を文字列として結合して返す
            return string.Join("", RamdomDigitList);
        }

    }
}

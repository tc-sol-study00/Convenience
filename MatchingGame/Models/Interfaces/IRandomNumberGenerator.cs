using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachingGame.Interfaces {
    /// <summary>
    /// 乱数クラス用インターフェース
    /// </summary>
    public interface IRandomNumberGenerator {
        public int DataLength { get; }
        public string GenerateUniqueThreeDigitNumber();
    }
}

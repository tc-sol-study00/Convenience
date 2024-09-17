using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using static Convenience.Models.ViewModels.Zaiko.ZaikoViewModel;


namespace Convenience.Models.Services {

    /// <summary>
    /// 倉庫在庫検索（サービス） 
    /// </summary>
    public class ZaikoService : IZaikoService {
        //DBコンテキスト
        private readonly ConvenienceContext _context;
        //在庫クラス（プロパティレイヤ）用変数
        private readonly IZaiko Zaiko;

        /// <summary>
        /// コンストラクター（ＡＳＰ用） 
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="zaiko">在庫クラスＤＩ注入用</param>
        public ZaikoService(ConvenienceContext context,IZaiko zaiko) {
            //DBコンテキストセット
            this._context = context;
            //在庫クラスインスタンス化
            this.Zaiko = zaiko;
            //Zaiko = new Zaiko(_context);
        }

        //コンストラクター（デバッグ用）
        public ZaikoService() {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var contextOptions = new DbContextOptionsBuilder<ConvenienceContext>()
                .UseNpgsql(configuration["ConvenienceContext"])
                .Options;

            _context = new ConvenienceContext(contextOptions);

            //在庫クラスインスタンス化
            Zaiko = new Zaiko(_context);
        }
        /// <summary>
        /// <para>検索キー画面の情報取得</para>
        /// </summary>
        /// <param name="inKeySetOrderArray"></param>
        /// <param name="inSelectWhereItemArray"></param>
        /// <returns></returns>
        public async Task<IList<ZaikoListLine>> KeyInput(KeyEventRec[] inKeySetOrderArray, SelecteWhereItem[] inSelectWhereItemArray) {

            const string doubleQuotation = "\"";
            string lambdaString = string.Empty;
            string setOrNotDoubleQuotation = string.Empty;
            string[] setOrNotKakko = new string[2];
            Type? typeForUseObjToWhere;

            //機能（１）  ：ラムダ式を作る
            //入力        ：Ｗｈｅｒｅ用キー（配列）
            //出力        ：Ｗｈｅｒｅ指示用ラムダ式

            //画面のWhereの一覧をひとつづつ取り出す
            foreach (var selecteWhereItem in inSelectWhereItemArray) {
                //左辺・比較演算時・右辺が揃わない場合は処理しない
                if (!string.IsNullOrEmpty(selecteWhereItem.LeftSide) &&
                    !string.IsNullOrEmpty(selecteWhereItem.ComparisonOperator) &&
                    !string.IsNullOrEmpty(selecteWhereItem.RightSide)) {

                    //画面の指示から左辺をセット

                    string leftSide = selecteWhereItem.LeftSide;

                    //左辺の変数の型を調べる
                    string? assemblyQualifiedName = typeof(ZaikoListLine).AssemblyQualifiedName;
                    if (assemblyQualifiedName is not null &&
                        Type.GetType(assemblyQualifiedName) is Type assemblyQualifiedNameType &&
                        assemblyQualifiedNameType.GetProperty(leftSide) is PropertyInfo propertyInfo) {
                        typeForUseObjToWhere = propertyInfo.PropertyType;
                    }
                    else {
                        continue;   //型がもし取得できなければ諦める
                    }

                    //変数の型にあわせた右辺のセット

                    //もし、型が文字型なら、右辺のデータにダブルコーテーションで囲む
                    setOrNotDoubleQuotation = typeForUseObjToWhere == typeof(string) ? doubleQuotation : "";

                    //もし、日付型なら、右辺のデータを日付比較するようにパースする
                    if (typeForUseObjToWhere == typeof(DateOnly)) {
                        if (!DateOnly.TryParse(selecteWhereItem.RightSide, out DateOnly w)) {
                            continue;
                        }
                        setOrNotKakko = new string[] { "DateOnly.Parse(", ")" };    //パース用関数
                        setOrNotDoubleQuotation = doubleQuotation;                  //ダブルクオーテーションセット
                    }
                    else {
                        //上記以外は数値として、ダブルクオーテーショを付けない
                        setOrNotKakko = new string[] { "", "" };
                    }

                    //ラムダ式の生成
                    lambdaString
                        += (string.IsNullOrEmpty(lambdaString) ? "s => s." : " && s.")
                        + $"{leftSide} {selecteWhereItem.ComparisonOperator} " 
                        + $"{setOrNotKakko[0]}{setOrNotDoubleQuotation}" 
                        + $"{selecteWhereItem.RightSide}" 
                        + $"{setOrNotDoubleQuotation}{setOrNotKakko[1]}";
                }
            }

            //機能（２）：倉庫在庫・注文実績明細検索用のQueryableを作成
            //入力      ：Ｗｈｅｒｅ指示用ラムダ式
            //          ：倉庫在庫（ＤＢ）・注文実績明細（ＤＢ）
            //出力      ：Ｗｈｅｒｅ指示追加後のQueryable

            //ラムダ式を条件に倉庫在庫・注文実績明細を検索するためのQueryableを構築
            IQueryable<ZaikoListLine> sokoZaikos = Zaiko.CreateSokoZaikoList(lambdaString);

            //上記Queryableにソート順指示を追加
            if (inKeySetOrderArray != null && inKeySetOrderArray.Any()) {
                foreach (var item in inKeySetOrderArray.Where(item => !string.IsNullOrEmpty(item.KeyEventData))) {
                    if (item.KeyEventData != null) {
                        sokoZaikos = Zaiko.AddOrderby(item.KeyEventData, item.Descending);
                    }
                }
            }

            //機能（３）  ：倉庫在庫・注文実績明細実際の問い合わせ実行
            //入力        ：倉庫在庫・注文実績明細検索用のQueryable
            //出力        ：入力の内容の実行結果（リスト）

            //上記までのQueryableで実際にＤＢ問い合わせを行い、リスト化
            return await sokoZaikos.ToListAsync();
        }
    }
}
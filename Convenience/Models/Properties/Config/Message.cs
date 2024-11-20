namespace Convenience.Models.Properties.Config {

    /// <summary>
    /// 正常・エラーメッセージ用クラス
    /// </summary>
    public class Message {
        /// <summary>
        /// メッセ―ジ表示用データクラス
        /// </summary>
        public class MessageDataClass {
            /// <summary>
            /// メッセージ番号
            /// </summary>
            public ErrDef MessageNo { get; set; }
            /// <summary>
            /// 表示メッセージ
            /// </summary>
            public string? MessageText { get; set; }
        }
        /// <summary>
        /// メッセ―ジ表示用データクラスのオブジェクト変数
        /// </summary>
        /// <remarks>
        /// NULL許容
        /// </remarks>
        public static MessageDataClass? MessageData { get; set; }

        /// <summary>
        /// エラーコードenum設定（0から付与）
        /// </summary>
        public enum ErrDef {
            DataValid = 0,
            NormalUpdate,
            CanNotlUpdate,
            ChumonIdError,
            ChumonDateError,
            ChumonIdRelationError,
            ChumonSuIsNull,
            ChumonSuBadRange,
            ChumonZanIsNull,
            SuErrorBetChumonSuAndZan,
            NothingChumonJisseki,
            OtherError
        }

        /// <summary>
        /// エラー番号とメッセージ表示のリンク
        /// </summary>
        private static readonly ICollection<MessageDataClass> MessageList = new List<MessageDataClass>
        {
            new (){ MessageNo=ErrDef.DataValid, MessageText="データチェックＯＫ" },
            new (){ MessageNo=ErrDef.NormalUpdate, MessageText="更新しました" },
            new (){ MessageNo=ErrDef.CanNotlUpdate, MessageText="更新できませんでした" },
            new (){ MessageNo=ErrDef.ChumonIdError, MessageText="注文コード書式エラー" },
            new (){ MessageNo=ErrDef.ChumonDateError, MessageText="注文日付エラー" },
            new (){ MessageNo=ErrDef.ChumonIdRelationError, MessageText="注文コードアンマッチ" },
            new (){ MessageNo=ErrDef.ChumonSuIsNull,MessageText="注文数が設定されていません" },
            new (){ MessageNo=ErrDef.ChumonSuBadRange,MessageText="注文数の数値範囲エラーです" },
            new (){ MessageNo=ErrDef.ChumonZanIsNull,MessageText="注文残が設定されていません" },
            new (){ MessageNo=ErrDef.SuErrorBetChumonSuAndZan,MessageText="注文数と注文残がアンマッチです" },
            new (){ MessageNo=ErrDef.OtherError, MessageText="その他エラー" }
        };

        /// <summary>
        /// エラーメッセージのセット
        /// </summary>
        /// <remarks>
        /// NULL返却あり
        /// </remarks>
        /// <param name="inErrCd">表示したいメッセージ内容に対応したエラーコード</param>
        /// <returns>メッセ―ジ表示用データクラスがセットされたオブジェクト変数</returns>
        public MessageDataClass? SetMessage(ErrDef inErrCd) {
            MessageData = MessageList.FirstOrDefault(m => m.MessageNo == inErrCd) ?? null;
            return MessageData;
        }
    }
}
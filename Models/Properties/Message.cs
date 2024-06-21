namespace Convenience.Models.Properties {

    /// <summary>
    /// 正常・エラーメッセージ用クラス
    /// </summary>
    public class Message {
        /// <summary>
        /// メッセ―ジ表示用データクラス
        /// </summary>
        public class MessageData {
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
        public static MessageData? messageData { get; set; }

        /// <summary>
        /// エラーコードenum設定（0から付与）
        /// </summary>
        public enum ErrDef {
            DataValid=0,
            NormalUpdate,
            CanNotlUpdate,
            ChumonIdError,
            ChumonDateError,
            ChumonIdRelationError,
            ChumonSuIsNull,
            ChumonSuBadRange,
            ChumonZanIsNull,
            SuErrorBetChumonSuAndZan,
            OtherError
        }

        /// <summary>
        /// エラー番号とメッセージ表示のリンク
        /// </summary>
        private static readonly ICollection<MessageData> MessageList = new List<MessageData>
        {
            new MessageData { MessageNo=ErrDef.DataValid, MessageText="データチェックＯＫ" },
            new MessageData { MessageNo=ErrDef.NormalUpdate, MessageText="更新しました" },
            new MessageData { MessageNo=ErrDef.CanNotlUpdate, MessageText="更新できませんでした" },
            new MessageData { MessageNo=ErrDef.ChumonIdError, MessageText="注文コード書式エラー" },
            new MessageData { MessageNo=ErrDef.ChumonDateError, MessageText="注文日付エラー" },
            new MessageData { MessageNo=ErrDef.ChumonIdRelationError, MessageText="注文コードアンマッチ" },
            new MessageData { MessageNo=ErrDef.ChumonSuIsNull,MessageText="注文数が設定されていません" },
            new MessageData { MessageNo=ErrDef.ChumonSuBadRange,MessageText="注文数の数値範囲エラーです" },
            new MessageData { MessageNo=ErrDef.ChumonZanIsNull,MessageText="注文残が設定されていません" },
            new MessageData { MessageNo=ErrDef.SuErrorBetChumonSuAndZan,MessageText="注文数と注文残がアンマッチです" },
            new MessageData { MessageNo=ErrDef.OtherError, MessageText="その他エラー" }
        };

        /// <summary>
        /// エラーメッセージのセット
        /// </summary>
        /// <remarks>
        /// NULL返却あり
        /// </remarks>
        /// <param name="inErrCd">表示したいメッセージ内容に対応したエラーコード</param>
        /// <returns>メッセ―ジ表示用データクラスがセットされたオブジェクト変数</returns>
        public MessageData? SetMessage(ErrDef inErrCd) {
            messageData = MessageList.FirstOrDefault(m => m.MessageNo == inErrCd) ?? null;
            return (messageData);
        }
    }
}
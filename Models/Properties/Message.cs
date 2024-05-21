namespace Convenience.Models.Properties {

    public class Message {

        public class MessageData {
            public ErrDef MessageNo { get; set; }
            public string? MessageText { get; set; }
        }

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

        public static MessageData messageData { get; set; }

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

        public MessageData SetMessage(ErrDef inErrCd) {
            messageData = MessageList.FirstOrDefault(m => m.MessageNo == inErrCd) ?? null;

            return (messageData);
        }
    }
}
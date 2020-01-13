/*******************************************************************************
 * 電話機
 * ---------------------------------------------------------------------------
 * TODO:
 * ○ 画面表示用の情報出力部分は出力先の実体を隠蔽する
 * ○ 話し中対応を行う（状態に応じてディスパッチャのハンドラセットを替える）
 * ○ デバッグログ生成が面倒なのでリフレクションに替える
 * 
 ******************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelephoneSimulator.Src.Common;
using TelephoneSimulator.Src.Switcher;

namespace TelephoneSimulator.Src.Telephone
{
    /// <summary>
    /// 電話
    /// </summary>
    class TelephoneMain : Agent
    {
        public string MyNumber { get; private set; } = null;
        public string MyName { get; set; } = null;
        public string TransportNumber { get; set; } = null;
        public string SecretaryNumber { get; set; } = null;
        public bool IsBusy { get; set; } = false;
        public DateTime? RejectTimeStart { get; set; } = null;
        public DateTime? RejectTimeEnd { get; set; } = null;

        private SwitcherMain switcher = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TelephoneMain(string myNumber, SwitcherMain _switcher)
        {
            MyNumber = myNumber;
            switcher = _switcher;
        }

        /// <summary>
        /// メッセージディスパッチャの作成
        /// </summary>
        /// <returns></returns>
        protected override IDictionary<string, Func<IList<object>, int>> CreateMessageDispathcer()
        {
            var disp = base.CreateMessageDispathcer();

            disp.Add("LogIn", LoginReceived);
            disp.Add("Call", CallReceived);
            disp.Add("CallResult", CallResultReceived);
            disp.Add("CallRequest", CallRequestReceived);

            return disp;
        }

        /// <summary>
        /// メッセージループ前処理
        /// </summary>
        /// <returns></returns>
        protected override int PreProcess()
        {
            base.PreProcess();

            // 交換機への登録要求を最初にポストしておく
            PostMessage(new List<object>() { this, "LogIn" });

            return 0;
        }

        /// <summary>
        /// 交換機への自機の登録要求
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private int LoginReceived(IList<object> command)
        {
            switcher.LogIn(this);
            return 0;
        }

        /// <summary>
        /// ユーザーから電話の要求 -> 交換機へ照会
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private int CallReceived(IList<object> command)
        {
            Console.WriteLine("Telephone.CallReceived();");
            Console.WriteLine("Calling to " + (string)command[2] + " ...");

            command[0] = this;
            switcher.PostMessage(command);
            return 0;
        }

        /// <summary>
        /// 交換機からの照会応答受信
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private int CallResultReceived(IList<object> command)
        {
            Console.WriteLine("Telephone.CallResultReceived();");
            if((int)command[2] < 0)
                Console.WriteLine((string)command[3]);
            return 0;
        }

        /// <summary>
        /// 交換機からの通話要求受信
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private int CallRequestReceived(IList<object> command)
        {
            Console.WriteLine("Telephone.CallRequestReceived();");
            return 0;
        }
    }
}
/*******************************************************************************
 * EOF
 ******************************************************************************/

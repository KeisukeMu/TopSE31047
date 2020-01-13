using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelephoneSimulator.Src.Common;
using TelephoneSimulator.Src.Telephone;

namespace TelephoneSimulator.Src.Switcher
{
    /// <summary>
    /// 交換機
    /// </summary>
    class SwitcherMain : Agent
    {
        private IDictionary<string, TelephoneMain> tels = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SwitcherMain()
        {
            tels = new Dictionary<string, TelephoneMain>();
        }

        /// <summary>
        /// メッセージディスパッチャの作成
        /// </summary>
        /// <returns></returns>
        protected override IDictionary<string, Func<IList<object>, int>> CreateMessageDispathcer()
        {
            var disp = base.CreateMessageDispathcer();

            disp.Add("Call", CallReceived);

            return disp;
        }

        /// <summary>
        /// 起動した電話の登録
        /// </summary>
        /// <param name="telephone"></param>
        public void LogIn(TelephoneMain telephone)
        {
            Console.WriteLine("Switcher.LoginReceived();");
            tels.Add(telephone.MyNumber, telephone);
        }

        /// <summary>
        /// 電話をかける
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private int CallReceived(IList<object> command)
        {
            Console.WriteLine("Switcher.CallReceived();");

            var sender = (Agent)command[0];
            var comname = (string)command[1];
            var toNumber = (string)command[2];

            if (!tels.ContainsKey(toNumber))
            {
                sender.PostMessage(new List<object>() { this, "CallResult", -1, "Not exists." });
                return -1;
            }

            var receiver = tels[toNumber];
            receiver.PostMessage(new List<object>() { this, "CallRequest", 0, sender });

            return 0;
        }
    }
}
/*******************************************************************************
 * EOF
 ******************************************************************************/

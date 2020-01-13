using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelephoneSimulator.Src
{
    /// <summary>
    /// コマンドライン引数管理
    /// </summary>
    class CommandArgs
    {

        public string PhoneNumber { get; set; } = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="args"></param>
        public CommandArgs(string[] args = null)
        {
            if (args != null) ParseCommand(args);
        }

        /// <summary>
        /// コマンドライン引数の解析
        /// </summary>
        /// <param name="args"></param>
        public void ParseCommand(string[] args)
        {
            foreach (var arg in args)
            {
                // 電話番号のパターンマッチング
                if (Regex.IsMatch(arg, "([+]|[0-9])([-]|[0-9])*") && PhoneNumber == null)
                {
                    PhoneNumber = arg;
                }
            }

            // 最低要件チェック
            if (!IsRunable()) throw new Exception("Commandline argument is wrong!");
        }

        /// <summary>
        /// <para>コマンドライン引数が最低限のプログラム実行要件を満たすか判定</para>
        /// <para>true:合格、false:不合格</para>
        /// </summary>
        /// <returns></returns>
        private bool IsRunable()
        {
            bool runable = true;

            runable &= (PhoneNumber != null);

            return runable;
        }
    }
}

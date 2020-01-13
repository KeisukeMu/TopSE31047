/*******************************************************************************
 * アプリケーション統合管理
 * ---------------------------------------------------------------------------
 * TODO:
 * ○ 画面表示用の情報出力部分は出力先の実体を隠蔽する
 * ○ 各電話機の設定をファイルから読み込めるようにする
 * ○ 0 から始まる番号は登録時に +81 に変換しとく？
 * 
 ******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TelephoneSimulator.Src.Switcher;
using TelephoneSimulator.Src.Telephone;

namespace TelephoneSimulator.Src
{
    /// <summary>
    /// アプリケーション統合管理
    /// </summary>
    class Application
    {
        private CommandArgs cargs = null;
        private SwitcherMain switcher = null;
        private TelephoneMain myphone = null;
        private IList<TelephoneMain> tels = null;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="args"></param>
        public Application(string[] args)
        {
            cargs = new CommandArgs(args);

            switcher = new SwitcherMain();
            myphone = new TelephoneMain("090-0000-0000", switcher);
            myphone.MyName = "Mine";

            tels = CreateTels();
        }

        /// <summary>
        /// 自分以外の電話の生成
        /// </summary>
        /// <returns></returns>
        private IList<TelephoneMain> CreateTels()
        {
            var _tels = new List<TelephoneMain>();

            var tel = new TelephoneMain("090-1111-1111", switcher)
            {
                MyName = "Mob1"
            };
            _tels.Add(tel);

            tel = new TelephoneMain("090-2222-2222", switcher)
            {
                MyName = "Mob2"
            };
            _tels.Add(tel);

            return _tels;
        }

        /// <summary>
        /// メイン
        /// </summary>
        /// <returns></returns>
        public int RunApplication()
        {
            //TestAgent();
            
            var taskSwitcher = switcher.RunAsync();
            var taskMyTel = myphone.RunAsync();

            IList<Task> tasks = new List<Task>();
            foreach(var tel in tels)
            {
                tasks.Add(tel.RunAsync());
            }

            // 電話の起動後、即座にコール開始
            myphone.PostMessage(new List<object>() { this, "Call", cargs.PhoneNumber });

            Task.WaitAll(taskSwitcher, taskMyTel);
            
            return 0;
        }

        /// <summary>
        /// Agent クラス初期テスト用
        /// メッセージパッシングのテスト
        /// </summary>
        private void TestAgent()
        {
            var task2 = myphone.RunAsync();

            var command1 = new List<object>() { this, "Call" };
            var command2 = new List<object>() { this, "Shutdown" };
            for (int i = 0; i < 10000; i++)
                myphone.PostMessage(command1);
            myphone.PostMessage(command2);

            Task.WaitAll(task2);
        }
    }
}
/*******************************************************************************
 * EOF
 ******************************************************************************/

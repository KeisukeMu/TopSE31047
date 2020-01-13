/*******************************************************************************
 * 疑似ハードウェアをエミュレートするための共通機能
 * ---------------------------------------------------------------------------
 * TODO:
 * ○ 画面表示用の情報出力部分は出力先の実体を隠蔽する
 * ○ コマンドオブジェクトは List をやめてインタフェースか抽象クラスにする
 * 
 ******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TelephoneSimulator.Src.Common
{
    /// <summary>
    /// 疑似ハードウェアをエミュレートするための共通機能
    /// </summary>
    abstract class Agent
    {
        public Task RunningTask { get; protected set; } = null;

        private Queue<IList<object>> msgQueue = null;
        protected IDictionary<string, Func<IList<object>,int>> msgDispatcher = null;
        protected bool shutdown = false;
        private ManualResetEvent resetEvent = null;

        public Agent()
        {
            msgQueue = new Queue<IList<object>>();
            //msgDispatcher = CreateMessageDispathcer();
            resetEvent = new ManualResetEvent(true);
        }

        /// <summary>
        /// エージェント起動
        /// </summary>
        /// <returns></returns>
        public Task RunAsync()
        {
            msgDispatcher = CreateMessageDispathcer();
            PreProcess();
            RunningTask = Task.Run(RunInternal);
            PostProcess();
            return RunningTask;
        }

        /// <summary>
        /// エージェント起動（内部）
        /// </summary>
        /// <returns></returns>
        private int RunInternal()
        {
            do
            {
                // 休止状態の場合はスレッド一時停止
                resetEvent.WaitOne();

                // メッセージキューの消化
                while (msgQueue.Count > 0)
                {
                    IList<object> msg = null;
                    lock (msgQueue) { msg = msgQueue.Dequeue(); }
                    var func = msgDispatcher[msg[1].ToString()];
                    var result = func(msg);
                }

                // キュー消化が終わったら休止状態とする
                resetEvent.Reset();
 
            } while (!shutdown);
            return 0;
        }

        /// <summary>
        /// メッセージのポスト
        /// </summary>
        /// <param name="msg"></param>
        public void PostMessage(IList<object> msg)
        {
            // メッセージをキューに登録してディスパッチャーを再開
            lock (msgQueue) { msgQueue.Enqueue(msg); }
            resetEvent.Set();
        }

        /// <summary>
        /// メッセージディスパッチャーを生成する（子クラスにて定義）
        /// </summary>
        /// <returns></returns>
        protected virtual IDictionary<string, Func<IList<object>, int>> CreateMessageDispathcer()
        {
            var disp = new Dictionary<string, Func<IList<object>, int>>();

            disp.Add("Shutdown", Shutdown);

            return disp;
        }

        /// <summary>
        /// メッセージループ開始前の処理
        /// </summary>
        /// <returns></returns>
        protected virtual int PreProcess()
        {
            // 必要に応じて子クラスで実装
            return 0;
        }

        /// <summary>
        /// シャットダウン前の処理
        /// </summary>
        /// <returns></returns>
        protected virtual int PostProcess()
        {
            // 必要に応じて子クラスで実装
            return 0;
        }

        /// <summary>
        /// シャットダウンする
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private int Shutdown(IList<object> command)
        {
            Console.WriteLine("Agent.Shutdown();");
            shutdown = true;
            return 0;
        }
    }
}
/*******************************************************************************
 * EOF
 ******************************************************************************/

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


using static getLancersInfoApp.Common;
using static getLancersInfoApp.MessageManagement;

namespace getLancersInfoApp
{
    public partial class Form1 : Form
    {

        static int PROC_STATUS = PROC_STANDBY;
        //static List<ItemInfo> itemInfo = new List<ItemInfo>(); //CSV読み込みデータ格納リスト

        //シングルトン
        static System.OperatingSystem os = System.Environment.OSVersion;
        static System.Text.Encoding enc = System.Text.Encoding.GetEncoding("utf-8");

        
        public Form1()
        {
            InitializeComponent();
            textBox_search.Text = "スクレイピング";
            conponentExecControl(PROC_STANDBY);
        }

        /// <summary>
        /// 処理途中はボタンおよびラベルを無効化する
        /// </summary>
        private void conponentExecControl(int procStatus)
        {
            PROC_STATUS = procStatus;
            if (PROC_STATUS == PROC_STANDBY)
            {
                label_status.Text = "--スタンバイ--";
                label_status.ForeColor = System.Drawing.Color.Black;
                //buttonAndTextBoxControl(true);
            }

            if (PROC_STATUS == PROC_START)
            {
                label_status.Text = "--処理を実行しています--";
                label_status.ForeColor = System.Drawing.Color.Black;
                //buttonAndTextBoxControl(false);
            }
            else if (PROC_STATUS == PROC_END)
            {
                label_status.Text = "--処理が完了しました--";
                label_status.ForeColor = System.Drawing.Color.Blue;
                //buttonAndTextBoxControl(true);
            }
            else if (PROC_STATUS == PROC_ERROR)
            {
                label_status.Text = "--処理の途中にエラーが発生しました--\n";
                label_status.ForeColor = System.Drawing.Color.Red;
                //buttonAndTextBoxControl(true);
            }
        }


        /// <summary>
        /// 実行ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_exec_Click(object sender, EventArgs e)
        {
            getLancersInfoMain();

        }
        
        /// <summary>
        /// ランサーズデータ取得メイン処理
        /// </summary>
        private void getLancersInfoMain()
        {
            conponentExecControl(PROC_START);

            //try
            //{
            //    string url = @"https://www.lancers.jp/work/search?keyword=" + textBox_search.Text;
            //    string htmlData = getHtml(url);
            //   // var chrome = new ChromeDriver(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            //   // chrome.Url = url;
            //   // Console.ReadKey();
            //   // chrome.Quit();
            //    Debug.WriteLine(htmlData);
            //
            //
            //
            //}
            //catch(Exception ex)
            //{
            //    //メッセージボックスを表示する
            //    MessageBox.Show("データが正常に取得できませんでした。",
            //        "エラー",
            //        MessageBoxButtons.OK,
            //        MessageBoxIcon.Error);
            //    conponentExecControl(PROC_ERROR);
            //    return;
            //
            //}

            try
            {
                // プロンプトを出さないようにする
                using (var driverService = ChromeDriverService.CreateDefaultService())
                {
                    driverService.HideCommandPromptWindow = true;
                    string url = @"https://www.lancers.jp/work/search?keyword=" + textBox_search.Text;
            
                    // 起動オプションの設定
                    var options = new ChromeOptions();
                    // ヘッドレス(画面なし)
                    options.AddArgument("--headless");
            
                    // ドライバ起動
                    using (var driver = new ChromeDriver(driverService, options))
                    {
                        // URLへ移動
                        driver.Url = url;
                        driver.Navigate();
            
                        // HTMLを取得する
                        var source = driver.PageSource;
                        string replaceHtmlData = getReplaceHtmlData(source);

                        string pattern = "c-media__title-inner.>(.*?)</span>";
                        MatchCollection match = extractingDataWithRegexMulti(replaceHtmlData, pattern);
                        foreach (Match m in match)
                        {
                            string itemHtmlData = m.Groups[1].ToString();

                            if (itemHtmlData.Contains("c-media__job-tags") && itemHtmlData != "")
                            {
                                //pattern = @"</ul>(.*?)アプリ";
                                pattern = @"</ul>(.*?)$";
                                itemHtmlData = extractingDataWithRegexSingle(itemHtmlData, pattern);
                                itemHtmlData = getReplaceHtmlData(itemHtmlData);

                            }
                            Debug.WriteLine(itemHtmlData);
                            //itemUrlList.Add(itemUrl);
                            //Debug.WriteLine(itemUrl);
                        }


                        var test = driver.FindElementsByClassName("c-media__title-inner");
                        //var test = driver.FindElement(By.ClassName("c-media__title-inner"));

                        
                        foreach (var data in test)
                        {
                            //string strTitle =  replaceHtmlData(data.Text);
                            
                          Trace.WriteLine(data.Text);

                        }
                        
                        

                        Trace.WriteLine("------");
                        Trace.WriteLine(source);
            
                        // タイトルを取得する
                        var title = driver.Title;
                        Trace.WriteLine("------");
                        Trace.WriteLine(title);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception: " + ex.Message + Environment.NewLine + ex.StackTrace);
                conponentExecControl(PROC_ERROR);

            }
            conponentExecControl(PROC_END);
        }
    }
}

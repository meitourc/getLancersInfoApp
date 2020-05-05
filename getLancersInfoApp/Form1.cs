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
                    string url = @"https://www.lancers.jp/work/search?keyword=" + textBox_search.Text + "&sort=started";

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

                        //Trace.WriteLine(source);

                        string replaceHtmlData = getReplaceHtmlData(source);

                        //Trace.WriteLine(replaceHtmlData);


                        string pattern = "<divclass=.c-media__content.>(.*?)<divclass=.c-media__work-follow.>";
                        MatchCollection match = extractingDataWithRegexMulti(replaceHtmlData, pattern);

                        int count  = 1;
                        foreach(Match m in match)
                        {
                            //itemNo（取得した順番）
                            string strItemNo = count.ToString();

                            //タイトル
                            pattern = "<spanclass=.c-media__title-inner.>(.*?)</span>";
                            String strItemTitle = extractingDataWithRegexSingle(m.ToString(), pattern);
                            if (strItemTitle.Contains("c-media__job-tags"))
                            {
                                //pattern = @"</ul>(.*?)アプリ";
                                pattern = @"</ul>(.*?)$";
                                strItemTitle = extractingDataWithRegexSingle(strItemTitle, pattern);
                                strItemTitle = getReplaceHtmlData(strItemTitle);
                            }
                            Debug.WriteLine(strItemTitle);

                            //URL
                            pattern = "";
                            string strItemUrl = extractingDataWithRegexSingle(m.ToString(), pattern);

                            //カテゴリ
                            pattern = "";
                            string strItemCategory = extractingDataWithRegexSingle(m.ToString(), pattern);
                            //形態
                            pattern = "";
                            string strItemWorkType = extractingDataWithRegexSingle(m.ToString(), pattern);
                            //募集期間
                            pattern = "";
                            string strItemApplicationPeriod = extractingDataWithRegexSingle(m.ToString(), pattern);
                            //取引期間
                            pattern = "";
                            string strItemTransactionPeriod = extractingDataWithRegexSingle(m.ToString(), pattern);
                            //価格
                            pattern = "";
                            string strItemPrice = extractingDataWithRegexSingle(m.ToString(), pattern);
                            //提案数
                            pattern = "";
                            string strItemProposalNum = extractingDataWithRegexSingle(m.ToString(), pattern);
                            //提案者
                            pattern = "";
                            string strItemProposer = extractingDataWithRegexSingle(m.ToString(), pattern);
                            //発注数
                            pattern = "";
                            string strItemOrderNum = extractingDataWithRegexSingle(m.ToString(), pattern);
                            //評価
                            pattern = "";
                            string strItemEvaluation = extractingDataWithRegexSingle(m.ToString(), pattern);
                            //説明
                            pattern = "";
                            string strItemDescription = extractingDataWithRegexSingle(m.ToString(), pattern);

                            count++;

                        }

                        //foreach (Match m in match)
                        //{
                        //    string itemHtmlData = m.Groups[1].ToString();
                        //
                        //    if (itemHtmlData.Contains("c-media__job-tags") && itemHtmlData != "")
                        //    {
                        //        //pattern = @"</ul>(.*?)アプリ";
                        //        pattern = @"</ul>(.*?)$";
                        //        itemHtmlData = extractingDataWithRegexSingle(itemHtmlData, pattern);
                        //        itemHtmlData = getReplaceHtmlData(itemHtmlData);
                        //
                        //    }
                        //    Debug.WriteLine(itemHtmlData);
                        //    //itemUrlList.Add(itemUrl);
                        //    //Debug.WriteLine(itemUrl);
                        //}


                        //var test = driver.FindElementsByClassName("c-media__content__right");
                        //var test = driver.FindElementsByClassName("c-media__title-inner");
                        var test = driver.FindElementsByClassName("c-media__job-time__remaining");

                        
                        //var test = driver.FindElement(By.ClassName("c-media-list__item c-media "));

                        //var test = driver.FindElement(By.ClassName("c-media__title-inner"));



                        foreach (var data in test)
                        {
                          string strTitle =  getReplaceHtmlData(data.Text );
                          Trace.WriteLine(data.Text);

                        }
                        
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

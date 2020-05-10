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
        static List<LancersItem> lancersItemList = new List<LancersItem>();


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
            File.Delete(@".\lancersItem.csv");
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
                    //options.AddArgument("--headless");

                    // ドライバ起動
                    using (var driver = new ChromeDriver(driverService, options))
                    {
                        // URLへ移動
                        driver.Url = url;
                        driver.Navigate();




                        // HTMLを取得する
                        string source = driver.PageSource;
                        //driver.FindElement(By.ClassName("pager__item pager__item--next")).Click();
                        //driver.FindElementByXPath("/html/body/div[2]/div[2]/main/section/section/nav/div/span/a"

                        for (int i = 0; i < 3; i++)
                        {
                            if (i > 0)
                            {
                                IWebElement element = driver.FindElement(By.LinkText("次へ"));
                                if (element.Displayed)
                                {
                                    element.Click();
                                }
                            }
                            source = driver.PageSource;
                            scrapingLancersData(source);
                            csvWrite_lancersItem();
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

        private void scrapingLancersData(string source)
        {
            string replaceHtmlData = getReplaceHtmlData(source);
            lancersItemList = new List<LancersItem>();

            string pattern = "<divclass=.c-media__content.>(.*?)<divclass=.c-media__work-follow.>";
            MatchCollection match = extractingDataWithRegexMulti(replaceHtmlData, pattern);
            int count = 0;
            try
            {
                foreach (Match m in match)
                {
                    LancersItem lancersItemData = new LancersItem();

                    if (lancersItemList.Count == 0)
                    {
                        lancersItemData.itemNo = "No";
                        lancersItemData.itemTitle = "タイトル";
                        lancersItemData.itemUrl = "URL";
                        lancersItemData.itemCategory = "カテゴリー";
                        lancersItemData.itemWorkType = "案件種別";
                        lancersItemData.itemTransactionPeriod = "取引条件";
                        lancersItemData.itemProposalNum = "提案数";
                        lancersItemData.itemProposer = "提案者";
                        lancersItemData.itemOrderNum = "発注数";
                        lancersItemData.itemEvaluation = "評価";
                        lancersItemData.itemDescription = "説明";
                    }
                    else
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

                        //URL
                        pattern = "<aclass=.c-media__title.href=.(.*?).>";
                        string strItemUrl = "https://www.lancers.jp/work" + extractingDataWithRegexSingle(m.ToString(), pattern);
                        //Debug.WriteLine(strItemUrl);

                        //カテゴリ
                        pattern = ".p-search-job__division-link.*?>(.*?)<";
                        string strItemCategory = extractingDataWithRegexSingle(m.ToString(), pattern);

                        pattern = "<liclass=.p-search-job__division.>(.*?)<";
                        match = extractingDataWithRegexMulti(m.ToString(), pattern);
                        strItemCategory += "/" + match[1].Groups[1];


                        //形態
                        pattern = "<spanclass=.c-badge__text.>(.*?)<";
                        string strItemWorkType = extractingDataWithRegexSingle(m.ToString(), pattern);

                        //Debug.WriteLine(strItemWorkType);
                        ////募集期間
                        //pattern = "";
                        //string strItemApplicationPeriod = extractingDataWithRegexSingle(m.ToString(), pattern);
                        //
                        //
                        ////取引期間
                        //pattern = "";
                        //string strItemTransactionPeriod = extractingDataWithRegexSingle(m.ToString(), pattern);

                        //条件
                        pattern = "class=.c-media__job-numberc-media__job-number--budgetFrom.>(.*?)<";
                        string strItemTransactionConditions = extractingDataWithRegexSingle(m.ToString(), pattern);

                        pattern = "class=.c-media__job-unitc-media__job-number--budgetFrom.>(.*?)<";
                        strItemTransactionConditions += extractingDataWithRegexSingle(m.ToString(), pattern);

                        pattern = "class=.c-media__job-unit.>(.*?)<";
                        strItemTransactionConditions += extractingDataWithRegexSingle(m.ToString(), pattern);

                        pattern = "class=.c-media__job-number.>(.*?)<";
                        strItemTransactionConditions += extractingDataWithRegexSingle(m.ToString(), pattern);

                        pattern = "class=.c-media__job-unit.>(.*?)<";
                        match = extractingDataWithRegexMulti(m.ToString(), pattern);
                        strItemTransactionConditions += match[1].Groups[1];

                        //提案数
                        pattern = "class=.c-media__job-number.>(.*?)<";
                        match = extractingDataWithRegexMulti(m.ToString(), pattern);
                        string strItemProposalNum = match[1].Groups[1].ToString();

                        //提案者
                        pattern = "class=.c-avatar__image-wrapper.>.*alt=.(.*?).title";
                        string strItemProposer = extractingDataWithRegexSingle(m.ToString(), pattern);
                        Debug.WriteLine(strItemProposer);

                        //発注数
                        pattern = "発注<strong>(.*?)</strong>";
                        string strItemOrderNum = extractingDataWithRegexSingle(m.ToString(), pattern);
                        if(strItemOrderNum == "")
                        {
                            strItemOrderNum = "-";
                        }

                        //評価
                        pattern = "評価<strong>(.*?)</strong>";
                        string strItemEvaluation = extractingDataWithRegexSingle(m.ToString(), pattern);
                        if (strItemEvaluation == "")
                        {
                            strItemEvaluation = "-";
                        }

                        //説明
                        pattern = "class=.c-media__description.>(.*?)</div>";
                        string strItemDescription = extractingDataWithRegexSingle(m.ToString(), pattern);

                        if (strItemDescription.Contains("c-media__job-tag-lists"))
                        {
                            //pattern = @"</ul>(.*?)アプリ";
                            pattern = @"<ulclass=.c-media__job-tag-lists.>(.*?)class=.c-media__description.";
                            strItemDescription = extractingDataWithRegexSingle(strItemTitle, pattern);
                            strItemDescription = getReplaceHtmlData(strItemTitle);
                        }

                        //Listに格納
                        lancersItemData.itemNo = strItemNo;
                        lancersItemData.itemTitle = strItemTitle;
                        lancersItemData.itemUrl = strItemUrl;
                        lancersItemData.itemCategory = strItemCategory;
                        lancersItemData.itemWorkType = strItemWorkType;
                        lancersItemData.itemTransactionPeriod = strItemTransactionConditions;
                        lancersItemData.itemProposalNum = strItemProposalNum;
                        lancersItemData.itemProposer = strItemProposer;
                        lancersItemData.itemOrderNum = strItemOrderNum;
                        lancersItemData.itemEvaluation = strItemEvaluation;
                        lancersItemData.itemDescription = strItemDescription;
                    }

                    lancersItemList.Add(lancersItemData);

                    count++;
                }
            }
            catch (Exception ex)
            {
                //メッセージボックスを表示する
                MessageBox.Show("データが正常に取得できませんでした。",
                "エラー",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                conponentExecControl(PROC_ERROR);
                return;

            }
        }
        /// <summary>
        /// CSV書き込み_サジェスト
        /// </summary>
        private void csvWrite_lancersItem()
        {
            string output_file_path = @".\lancersItem.csv";
            string strData = ""; //1行分のデータ

            try
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(output_file_path,
                                                                       true,
                                                                       System.Text.Encoding.Default);
                //ヘッダ書き込み
                foreach (var data in lancersItemList)
                {
                    strData =
                    data.itemNo + DELIMITER +
                    DOUBLE_QUOTATION + data.itemTitle               + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemUrl                 + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemCategory            + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemWorkType            + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemTransactionPeriod   + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemProposalNum         + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemProposer            + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemOrderNum            + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemEvaluation          + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemDescription         + DOUBLE_QUOTATION;

                    sw.WriteLine(strData);
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                //メッセージボックスを表示する
                MessageBox.Show("CSVファイルの書き込みに失敗しました。",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                conponentExecControl(PROC_ERROR);
                return;
            }
            string fullOutputPath = System.IO.Path.GetFullPath(output_file_path);
            label_output.Text = "出力先：" + fullOutputPath;

        }
    }
}

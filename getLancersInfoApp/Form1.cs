using Codeplex.Data;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
        string db_file = "Test.db";
        static string WEBHOOK_URL = "https://hooks.slack.com/services/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";
        //static List<ItemInfo> itemInfo = new List<ItemInfo>(); //CSV読み込みデータ格納リスト

        //シングルトン
        static System.OperatingSystem os = System.Environment.OSVersion;
        static System.Text.Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
        static List<LancersItem> lancersItemList = new List<LancersItem>();

        //static int count = 0;


        public Form1()
        {
            InitializeComponent();
            textBox_search.Text = "スクレイピング";
            conponentExecControl(PROC_STANDBY);
        }

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                フォームを表示ToolStripMenuItem.Enabled = false;
            }
            else
            {
                フォームを表示ToolStripMenuItem.Enabled = true;
            }
        }

        private void button_createDb_Click(object sender, EventArgs e)
        {
            deleteDB();
            createDB();
            connectDB();
            insertDataToDB();
            getDataFromDB();
        }

        /// <summary>
        /// 終了ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_end_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        /// <summary>
        /// フォームを表示
        /// </summary>
        private void showMainForm()
        {
            this.Show();
            this.ShowInTaskbar = true;
            フォームを表示ToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// フォームを表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_showForm_Click(object sender, EventArgs e)
        {
            showMainForm();
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
        /// フォームの☓ボタンを押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            putInTray();
        }

        /// <summary>
        /// トレイに入れる
        /// </summary>
        private void putInTray()
        {
            this.Hide();
            this.ShowInTaskbar = false;
            フォームを表示ToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// 表示_タスクトレイ右クリックメニュー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void フォームを表示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showMainForm();

        }

        /// <summary>
        /// 終了_タスクトレイ右クリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Application.Exit();
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
        /// 新規に取得した情報をSlackに通知
        /// </summary>
        private void slackNotification()
        {
            var wc = new WebClient();
            var data = DynamicJson.Serialize(new
            {
                text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                icon_emoji = ":ghost:", //アイコンを動的に変更する
                username = "投稿テスト用Bot"  //名前を動的に変更する
            });

            wc.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");
            wc.Encoding = Encoding.UTF8;

            wc.UploadString(WEBHOOK_URL, data);

        }




        /// <summary>
        /// ランサーズデータ取得メイン処理
        /// </summary>
        private void getLancersInfoMain()
        {
            conponentExecControl(PROC_START);

            //ファイル書き込み準備処理
            File.Delete(@".\lancersItem.csv");

            LancersItem lancersItemData = new LancersItem();
            //lancersItemData.itemGetDate = "取得日時";
            //lancersItemData.itemTitle = "タイトル";
            //lancersItemData.itemUrl = "URL";
            //lancersItemData.itemCategory = "カテゴリー";
            //lancersItemData.itemWorkType = "案件種別";
            //lancersItemData.itemTransactionPeriod = "取引条件";
            //lancersItemData.itemProposalNum = "提案数";
            //lancersItemData.itemProposer = "提案者";
            //lancersItemData.itemOrderNum = "発注数";
            //lancersItemData.itemEvaluation = "評価";
            //lancersItemData.itemDescription = "説明";
            //lancersItemList.Add(lancersItemData);

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

                        for (int i = 0; i < 1; i++)
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

            try
            {
                foreach (Match m in match)
                {
                    LancersItem lancersItemData = new LancersItem();

                    //itemNo（取得した順番）
                    DateTime dt = DateTime.Now;
                    string strItemGetDate = dt.ToString();

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
                    if (strItemOrderNum == "")
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
                    lancersItemData.itemGetDate = strItemGetDate;
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

                    lancersItemList.Add(lancersItemData);

                    //count++;
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
                    data.itemGetDate + DELIMITER +
                    DOUBLE_QUOTATION + data.itemTitle + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemUrl + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemCategory + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemWorkType + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemTransactionPeriod + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemProposalNum + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemProposer + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemOrderNum + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemEvaluation + DOUBLE_QUOTATION + DELIMITER +
                    DOUBLE_QUOTATION + data.itemDescription + DOUBLE_QUOTATION;

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





        //参考：http://rubbish.mods.jp/blog/2016/08/22/visual-studio%E3%81%A7c%E3%81%8B%E3%82%89sqlite%E3%82%92%E4%BD%BF%E3%81%86%E6%96%B9%E6%B3%95/
        // データベース作成
        private void createDB()
        {
            // コネクションを開いてテーブル作成して閉じる  
            using (var conn = new SQLiteConnection("Data Source=" + db_file))
            {
                conn.Open();
                using (SQLiteCommand command = conn.CreateCommand())
                {

                    command.CommandText = "create table Sample(" +
                        "Id INTEGER  PRIMARY KEY AUTOINCREMENT," +
                        "itemGetDate DATE" +
                        " itemTitle TEXT," +
                        " itemUrl TEXT," +
                        " itemCategory TEXT," +
                        "itemWorkType TEXT," +
                        "itemApplicationPeriod  TEXT," +
                        "itemTransactionPeriod TEXT," +
                        "itemPrice  TEXT," +
                        "itemProposalNum  TEXT," +
                        "itemProposer  TEXT," +
                        "itemOrderNum  TEXT," +
                        "itemEvaluation  TEXT," +
                        "itemDescription  TEXT)";
                    //command.CommandText = "create table Sample(Id INTEGER  PRIMARY KEY AUTOINCREMENT, Name TEXT, Age INTEGER)";


                    command.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        // データベース削除
        private void deleteDB()
        {
            // コネクションを開いてテーブル作成して閉じる  
            using (var conn = new SQLiteConnection("Data Source=" + db_file))
            {
                conn.Open();
                using (SQLiteCommand command = conn.CreateCommand())
                {

                    command.CommandText = "drop table Sample";
                    command.ExecuteNonQuery();
                }

                conn.Close();
            }
        }


        // データベース接続
        private void connectDB()
        {

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + db_file))
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("Connection Success", "Connection Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Connection Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        // データの追加

        //private void insertDataToDB()
        //{
        //    using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + db_file))
        //    {
        //        conn.Open();
        //        using (SQLiteTransaction trans = conn.BeginTransaction())
        //        {
        //            SQLiteCommand cmd = conn.CreateCommand();

        //            // インサート
        //            cmd.CommandText = "INSERT INTO Sample (Name, Age) VALUES (@Name, @Age)";


        //            // パラメータセット
        //            cmd.Parameters.Add("Name", System.Data.DbType.String);
        //            cmd.Parameters.Add("Age", System.Data.DbType.Int64);

        //            // データ追加
        //            cmd.Parameters["Name"].Value = "佐藤";
        //            cmd.Parameters["Age"].Value = 32;
        //            cmd.ExecuteNonQuery();

        //            cmd.Parameters["Name"].Value = "斉藤";
        //            cmd.Parameters["Age"].Value = 24;
        //            cmd.ExecuteNonQuery();

        //            // コミット
        //            trans.Commit();
        //        }
        //    }
        //}


        private void insertDataToDB()
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + db_file))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    SQLiteCommand cmd = conn.CreateCommand();

                    // インサート
                    cmd.CommandText = "INSERT INTO Sample (" +
                        "itemGetDate," +
                        "itemTitle," +
                        "itemUrl," +
                        "itemCategory," +
                        "itemWorkType," +
                        "itemApplicationPeriod," +
                        "itemTransactionPeriod," +
                        "itemPrice," +
                        "itemPrice," +
                        "itemProposalNum," +
                        "itemProposer," +
                        "itemOrderNum," +
                        "itemEvaluation," +
                        "itemDescription) " +
                        "VALUES (@itemTitle, @itemUrl," +
                        "@itemCategory," +
                        "@itemWorkType," +
                        "@itemApplicationPeriod," +
                        "@itemTransactionPeriod," +
                        "@itemPrice,@itemPrice," +
                        "@itemProposalNum," +
                        "@itemProposer," +
                        "@itemOrderNum," +
                        "@itemEvaluation," +
                        "@itemDescription)";


                    // パラメータセット
                    
                    cmd.Parameters.Add("itemGetDate", System.Data.DbType.Date);
                    cmd.Parameters.Add("itemTitle", System.Data.DbType.String);
                    cmd.Parameters.Add("itemUrl", System.Data.DbType.String);
                    cmd.Parameters.Add("itemCategory", System.Data.DbType.String);
                    cmd.Parameters.Add("itemWorkType", System.Data.DbType.String);
                    cmd.Parameters.Add("itemApplicationPeriod", System.Data.DbType.String);
                    cmd.Parameters.Add("itemTransactionPeriod", System.Data.DbType.String);
                    cmd.Parameters.Add("itemPrice", System.Data.DbType.String);
                    cmd.Parameters.Add("itemProposalNum", System.Data.DbType.String);
                    cmd.Parameters.Add("itemProposer", System.Data.DbType.String);
                    cmd.Parameters.Add("itemOrderNum", System.Data.DbType.String);
                    cmd.Parameters.Add("itemEvaluation", System.Data.DbType.String);
                    cmd.Parameters.Add("itemDescription", System.Data.DbType.String);


                    //データ追加
                    foreach (var data in lancersItemList)
                    {
                        cmd.Parameters["itemGetDate"].Value = data.itemGetDate;
                        cmd.Parameters["itemTitle"].Value = data.itemTitle;
                        cmd.Parameters["itemUrl"].Value = data.itemUrl;
                        cmd.Parameters["itemCategory"].Value = data.itemCategory;
                        cmd.Parameters["itemWorkType"].Value = data.itemWorkType;
                        cmd.Parameters["itemApplicationPeriod"].Value = data.itemTransactionPeriod;
                        cmd.Parameters["itemTransactionPeriod"].Value = data.itemProposalNum;
                        cmd.Parameters["itemPrice"].Value = data.itemProposer;
                        cmd.Parameters["itemProposalNum"].Value = data.itemOrderNum;
                        cmd.Parameters["itemProposer"].Value = data.itemEvaluation;
                        cmd.Parameters["itemTitle"].Value = data.itemDescription;
                    }

                    //cmd.Parameters["itemTitle"].Value = "test_title";
                    //cmd.Parameters["itemUrl"].Value = "test_url";
                    //cmd.ExecuteNonQuery();
                    //
                    //cmd.Parameters["itemTitle"].Value = "test_title2";
                    //cmd.Parameters["itemUrl"].Value = "test_url2";
                    //cmd.ExecuteNonQuery();

                    // コミット
                    trans.Commit();
                }
            }
        }

        // データの取得
        private void getDataFromDB()
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + db_file))
            {
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Sample";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    string message = "Id,Name,Age\n";

                    while (reader.Read())
                    {
                        message += reader["Id"].ToString() + "," + reader["itemTitle"].ToString() + "," + reader["itemUrl"].ToString() + "\n";
                    }

                    MessageBox.Show(message);
                }
                conn.Close();
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
namespace getLancersInfoApp
{
    class Common
    {

        public static string DELIMITER = ","; //CSV読み書き用区切り文字
        public static string DOUBLE_QUOTATION = "\""; //ダブルクォーテーション
        public static string LINE_FEED_CODE = convertLineFeedCode(); //改行コード

        /// <summary>
        /// OSごとに改行文字列を設定
        /// </summary>
        public static string convertLineFeedCode()
        {
            System.OperatingSystem os = System.Environment.OSVersion;
            PlatformID pid = os.Platform;
            Debug.WriteLine("os:" + pid);
            string strLineFeedCode = "\r\n";
            if (pid == PlatformID.Win32NT)
            {
                strLineFeedCode = "\r\n";
            }
            else if (pid == PlatformID.Unix)
            {
                strLineFeedCode = "\n";

            }
            else if (pid == PlatformID.MacOSX)
            {
                strLineFeedCode = "\r";
            }
            else
            {
                strLineFeedCode = "\r\n";
            }
            return strLineFeedCode;
        }

        ///// <summary>
        ///// 引数urlにアクセスした際に取得できるHTMLを返す
        ///// </summary>
        ///// <param name="url">URL</param>
        ///// <returns>取得したHTML</returns>
        public static string getHtml(string url)
        {
            string html = "";
            try
            {
                //指定したURLに対してrequestを投げてresponseを取得
                Console.WriteLine(url);

                var req = (HttpWebRequest)WebRequest.Create(url);
                //req.UserAgent = "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like ecko) //Chrome / 70.0.3538.77 Safari / 537.36";
                req.UserAgent = "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 81.0.4044.122 Safari / 537.36"; //確認くん
                                                                                                                                                             //req.Timeout = 10000000;

                using (var res = (HttpWebResponse)req.GetResponse())
                using (var resSt = res.GetResponseStream())
                using (var sr = new StreamReader(resSt, Encoding.UTF8))
                {
                    //HTMLを取得する。
                    html = sr.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex + "Errorが発生しました。");
                //using (var sr = new StreamReader(ex.Response.GetResponseStream(), Encoding.GetEncoding("shift_jis")))
                using (var sr = new StreamReader(ex.Response.GetResponseStream(), Encoding.ASCII))
                    html = sr.ReadToEnd();
                Console.WriteLine(html);

            }


            //WebClient wc = new WebClient();
            //try
            //{
            //    wc.Encoding = Encoding.UTF8;
            //    html = wc.DownloadString(url);
            //}
            //catch (WebException exc)
            //{
            //}

            return html;
        }

        private StreamWriter stream = null;

        /// <summary>
        /// 共通化用CSVWrite（作成途中）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        //public void csvWrite<T>(List<T> row)
        public void csvWrite<T>(List<T> row)
        {

            string output_file_path = @".\rakkutenItemInfoResult.csv";
            try
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(output_file_path,
                                                   false,
                                                   System.Text.Encoding.Default);

                var sb = new StringBuilder();
                foreach (var cell in row)
                {



                    var value = cell.ToString();
                    //if (value.Contains(this.NewLine)||
                    if (
                     value.Contains(",") ||
                     value.Contains("\""))
                    {
                        value = value.Replace("\"", "\"\"");
                        sb.Append("\"");
                        sb.Append(value);
                        sb.Append("\"");
                    }
                    else
                    {
                        sb.Append(value);
                    }
                    sb.Append(",");
                    sb.Remove(sb.Length - 1, 1);

                    sw.WriteLine(sb.ToString());
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + "Errorが発生しました。");
            }

        }

        /// <summary>
        /// 現在のストリームで利用される改行文字列を取得または設定します。
        /// </summary>
        public string NewLine
        {
            get
            {
                return this.stream.NewLine;
            }

            set
            {
                this.stream.NewLine = value;
            }
        }

        /// <summary>
        /// curlでGet処理
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task getHtmlWithCurlAsync(string url)
        {
            var handler = new HttpClientHandler();
            // If you are using .NET Core 3.0+ you can replace `~DecompressionMethods.None` to `DecompressionMethods.All`
            handler.AutomaticDecompression = ~DecompressionMethods.None;
            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), url))
                //using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://tech-lab.sios.jp/archives/15711"))
                {
                    request.Headers.TryAddWithoutValidation("authority", "item.rakuten.co.jp");
                    request.Headers.TryAddWithoutValidation("cache-control", "max-age=0");
                    request.Headers.TryAddWithoutValidation("upgrade-insecure-requests", "1");
                    request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.113 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                    request.Headers.TryAddWithoutValidation("sec-fetch-site", "none");
                    request.Headers.TryAddWithoutValidation("sec-fetch-mode", "navigate");
                    request.Headers.TryAddWithoutValidation("sec-fetch-user", "?1");
                    request.Headers.TryAddWithoutValidation("sec-fetch-dest", "document");
                    request.Headers.TryAddWithoutValidation("accept-language", "ja,en-US;q=0.9,en;q=0.8");
                    request.Headers.TryAddWithoutValidation("cookie", "Rp=e5a72126b2e28e8c383326ac715d32defd58e197b0a8eed; _ra=1563639942360|975f43bf-b48f-4327-a251-180987873b05; cto_lwid=097f9234-8e87-4fe0-8418-67c6a451b531; _fbp=fb.2.1563639945492.502029503; Rt=d0b5348654575fe031515948ce68db10; __pp_uid=PYs5RP7ExyZK8b9MpGG2qPC4ez1LeCpE; __gads=ID=d27f0c137fd88c61:T=1563639973:S=ALNI_MZRxOCpNoQ74u1Q_zQccu2MITYkeg; fmSessionV2=1cdb7fdc-0fb4-1e94-beb4-ce7d70f77fbf; _ga=GA1.3.229895832.1568509209; tg_af_histid=h686973745f6964r3336333633353634333736343338363536353332333033303330333533323631326536313339363236353335333033373632; _mkto_trk=id:673-CVK-590&token:_mch-rakuten.co.jp-1572287844283-80962; utag_main=v_id:016c519db4c90010c3430e51c88503079004a07100bd0$_sn:3$_ss:0$_st:1575886818123$ses_id:1575884637992%3Bexp-session$_pn:4%3Bexp-session; _ebtd=2.7arq1479eb.1562850391; rmStore=tmid:; stc113632=env:1586017626%7C20200505162706%7C20200404165706%7C1%7C1029826:20210404162706|uid:1564372287961.1676365398.1475806.113632.767004688.:20210404162706|srchist:1029827%3A1584416669%3A20200417034429%7C1029826%3A1586017626%3A20200505162706:20210404162706|nsc:1:20210120063746|tsa:0:20200404165706; Rz=A0pfwe5mCJhJ-BX4kozcltqmqTM63MOFfr73PUEuaEkjkhY76gw2SXdx_-kVsjaknOqpYUexd9MTapGU0fW7gjYQkjZRbu67TYlrR7vHIXY8_KQyvJDWum32LWrWPPDn8kgvUvR-62iP; s_pers=%20s_xsent%3D1912%7C1733558280342%3B%20s_fid%3D6AF37A5D2B9DA3EC-3CA9D76E0748FB4D%7C1639326768227%3B%20s_mrcr%3D5200400000000000%257C4000000000000%257C4000000000000%257C4000000000000%7C1745069342165%3B; Rb=1obf697f425; Ry=1&1b239f734651eb631b89eafca2555a9257cf89f70ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEF&20200423234752; Rq=5ea1aa98; Rg=1142cb%2603e82aaf793b7587f5268c9ae4a4be1263b7966e7748b7afc2fbdb85816f23a3e1d79ed1594105e84e58c95a298a85a54aef6ad417803fd84ac1dc580fba7abc9dbcead360c0b7dcf2951550af8d1aa6de57acbb03dcd8c55690d20d5c840f701e1895099c8afe1eaeeca1a0998fc5c58990cd014c60f77988b5da72faa96eae65ea976f59d713ead55231dc032f0536%262020-04-24+23%3A34%3A38; ak_bmsc=7F859D40183395EB5B2F30D3CBFEBB83B81CE52C5509000057DDA65E55130B7C~plgLoM1xlyUu2j6GApacXcKm0ipO1R9YfAoZUlRfL3+sW9s1Qr3zxkDqhHEYMn7nDycFEbSgiQutDVoonwNxsAxsOghd6jzYMeBPgROLV/bPo6gimP/kO9vbJ91xMzbsMN76i6ErvSCWzVBZckIjW7ol3JBLGdzCKqrEtEy/NuBiO0tynofmzRpclk+M5ENNc5F2liyu9pZdJFKH6o0kNVbGP7u16DsxurGJlW5VlDbCLdAcbzZZb1tB85jWvtQ5Tc; rat_v=7ce02c2079e9f2eb678a9aabf35ea6dd5accfe6; bm_sv=21508E07587E7B84BC32B467A75FC409~H4TAPKvOOgiPO+k3dZNdK0rkVwpQ/XCX/Rlt8BuABGg1bycGkO1+yrh/FGdMwq4X+VRdaBr9tAip5OXIKNjUq7zHuz7WiYIx0a4mxy4SGk6BBLhOJlM11rnl8viZnu2OCKsNdus/KA+dgyPyaKuH6MBSXRzTKMy+yYguGN7ayQA=; Re=14.5.2.0.0.560052.1:13.4.2.0.0.552407.1:13.3.3.0.0.211582.1:12.13.5.27.0.101768.1:22.4.2.0.0.210254.1:27.12.3.0.0.406336.3:14.1.0.0.0.560202.2:24.21.0.0.0.101254.3:22.4.3.0.0.210271.1:11.5.8.3.0.212277.1:24.3.10.2.0.208776.1:10.23.2.1.0.508925.1:22.4.15.12.6.112136.3:14.7.4.0.0.564277.2:31.5.7.0.0.101888.1:21.10.3.3.0.300851.1:24.3.10.1.0.208775.22:11.4.3.5.0.566900.1:31.7.12.0.0.201528.4:10.15.5.1.6.207966.3-19.12.4.0.0.562084.3:11.1.1.1.0.565105.4:34.3.2.7.0.111224.4:32.3.3.1.2.302763.2:18.8.3.0.0.303017.2:27.12.3.0.0.406336.3:14.1.0.0.0.560202.2:24.21.0.0.0.101254.3:22.4.3.0.0.210271.1:11.5.8.3.0.212277.1:24.3.10.2.0.208776.1:10.23.2.1.0.508925.1:22.4.15.12.6.112136.3:14.7.4.0.0.564277.2:31.5.7.0.0.101888.1:21.10.3.3.0.300851.1:24.3.10.1.0.208775.22:11.4.3.5.0.566900.1:31.7.12.0.0.201528.4:10.15.5.1.6.207966.3");
                    var response = await httpClient.SendAsync(request);
                    var response2 = await httpClient.SendAsync(request);
                    Console.WriteLine(response);
                }
            }
        }

        /// <summary>
        /// 正規表現でデータを抽出（複数）
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MatchCollection extractingDataWithRegexMulti(string html, string pattern)
        {
            MatchCollection matche = Regex.Matches(html, pattern);
            return matche;
        }

        /// <summary>
        /// 正規表現でデータを抽出（単数）
        /// </summary>
        /// <param name="html"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string extractingDataWithRegexSingle(string html, string pattern)
        {
            Regex regex = new Regex(pattern);
            Match match = regex.Match(html);
            //var test = match.Groups[0].ToString();
            //var test2 = match.Groups[1].ToString();
            //var test3 = match.Groups[2].ToString();

            return match.Groups[1].ToString();
        }


        /// <summary>
        /// htmlから空行やタブ等を取り除きます。
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string getReplaceHtmlData(string html)
        {
            //@空白改行をリプレイスする
            //html = html.Replace("\r", "").Replace("\n", "");
            string beforeReplacePattern = "\\s";
            string afterReplacePattern = "";
            Regex regex = new Regex(beforeReplacePattern);
            html = regex.Replace(html, afterReplacePattern);
            //replace済みhtml出力（デバッグ甩）
            //Console.WriteLine("----------------------------------------------------------------------------------------\n");
            //Console.WriteLine("html:" +  html + "\n");
            //Console.WriteLine("----------------------------------------------------------------------------------------\n");
            return html;
        }


    }
}

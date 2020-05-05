using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace getLancersInfoApp
{
    class LancersItem
    {
        public string itemNo { get; set; } //itemNo（取得した順番）
        public string itemTitle { get; set; } //タイトル
        public string itemUrl { get; set; } //URL
        public string itemCategory { get; set; } //カテゴリ
        public string itemWorkType { get; set; } //形態
        public string itemApplicationPeriod { get; set; } //募集期間
        public string itemTransactionPeriod{ get; set; } //取引期間
        public string itemPrice { get; set; } //価格
        public string itemProposalNum { get; set; } //提案数
        public string itemProposer { get; set; } //提案者
        public string itemOrderNum { get; set; } //発注数
        public string itemEvaluation { get; set; } //評価
        public string itemDescription { get; set; } //説明

    }
}

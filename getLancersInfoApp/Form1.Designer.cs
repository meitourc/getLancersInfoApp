namespace getLancersInfoApp
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox_search = new System.Windows.Forms.TextBox();
            this.button_exec = new System.Windows.Forms.Button();
            this.label_status = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox_search
            // 
            this.textBox_search.Location = new System.Drawing.Point(112, 99);
            this.textBox_search.Name = "textBox_search";
            this.textBox_search.Size = new System.Drawing.Size(245, 31);
            this.textBox_search.TabIndex = 0;
            // 
            // button_exec
            // 
            this.button_exec.Location = new System.Drawing.Point(686, 99);
            this.button_exec.Name = "button_exec";
            this.button_exec.Size = new System.Drawing.Size(305, 195);
            this.button_exec.TabIndex = 1;
            this.button_exec.Text = "実行";
            this.button_exec.UseVisualStyleBackColor = true;
            this.button_exec.Click += new System.EventHandler(this.button_exec_Click);
            // 
            // label_status
            // 
            this.label_status.AutoSize = true;
            this.label_status.Location = new System.Drawing.Point(255, 253);
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(58, 24);
            this.label_status.TabIndex = 2;
            this.label_status.Text = "状態";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(122, 253);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "ステータス：";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1236, 645);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label_status);
            this.Controls.Add(this.button_exec);
            this.Controls.Add(this.textBox_search);
            this.Name = "Form1";
            this.Text = "ランサーズ情報取得";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_search;
        private System.Windows.Forms.Button button_exec;
        private System.Windows.Forms.Label label_status;
        private System.Windows.Forms.Label label1;
    }
}


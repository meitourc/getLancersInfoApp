﻿namespace getLancersInfoApp
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.textBox_search = new System.Windows.Forms.TextBox();
            this.button_exec = new System.Windows.Forms.Button();
            this.label_status = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label_output = new System.Windows.Forms.Label();
            this.button_createDb = new System.Windows.Forms.Button();
            this.button_end = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.フォームを表示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.終了ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1.SuspendLayout();
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
            this.button_exec.Location = new System.Drawing.Point(552, 99);
            this.button_exec.Name = "button_exec";
            this.button_exec.Size = new System.Drawing.Size(406, 195);
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
            // label_output
            // 
            this.label_output.AutoSize = true;
            this.label_output.Location = new System.Drawing.Point(142, 414);
            this.label_output.Name = "label_output";
            this.label_output.Size = new System.Drawing.Size(94, 24);
            this.label_output.TabIndex = 4;
            this.label_output.Text = "出力先：";
            // 
            // button_createDb
            // 
            this.button_createDb.Location = new System.Drawing.Point(1104, 99);
            this.button_createDb.Name = "button_createDb";
            this.button_createDb.Size = new System.Drawing.Size(293, 195);
            this.button_createDb.TabIndex = 6;
            this.button_createDb.Text = "データベース作成";
            this.button_createDb.UseVisualStyleBackColor = true;
            this.button_createDb.Click += new System.EventHandler(this.button_createDb_Click);
            // 
            // button_end
            // 
            this.button_end.Location = new System.Drawing.Point(1049, 529);
            this.button_end.Name = "button_end";
            this.button_end.Size = new System.Drawing.Size(338, 84);
            this.button_end.TabIndex = 7;
            this.button_end.Text = "終了";
            this.button_end.UseVisualStyleBackColor = true;
            this.button_end.Click += new System.EventHandler(this.button_end_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.フォームを表示ToolStripMenuItem,
            this.終了ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(223, 76);
            // 
            // フォームを表示ToolStripMenuItem
            // 
            this.フォームを表示ToolStripMenuItem.Name = "フォームを表示ToolStripMenuItem";
            this.フォームを表示ToolStripMenuItem.Size = new System.Drawing.Size(300, 36);
            this.フォームを表示ToolStripMenuItem.Text = "フォームを表示";
            this.フォームを表示ToolStripMenuItem.Click += new System.EventHandler(this.フォームを表示ToolStripMenuItem_Click);
            // 
            // 終了ToolStripMenuItem
            // 
            this.終了ToolStripMenuItem.Name = "終了ToolStripMenuItem";
            this.終了ToolStripMenuItem.Size = new System.Drawing.Size(300, 36);
            this.終了ToolStripMenuItem.Text = "終了";
            this.終了ToolStripMenuItem.Click += new System.EventHandler(this.終了ToolStripMenuItem_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "ランサーズ案件取得";
            this.notifyIcon1.Visible = true;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1493, 865);
            this.Controls.Add(this.button_end);
            this.Controls.Add(this.button_createDb);
            this.Controls.Add(this.label_output);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label_status);
            this.Controls.Add(this.button_exec);
            this.Controls.Add(this.textBox_search);
            this.Name = "Form1";
            this.Text = "ランサーズ案件取得";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_search;
        private System.Windows.Forms.Button button_exec;
        private System.Windows.Forms.Label label_status;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_output;
        private System.Windows.Forms.Button button_createDb;
        private System.Windows.Forms.Button button_end;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem フォームを表示ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 終了ToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Timer timer1;
    }
}


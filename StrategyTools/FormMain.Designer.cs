namespace StrategyTools
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.buttonStartStrategy = new System.Windows.Forms.Button();
            this.dataGridViewStrategy = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPagePosition = new System.Windows.Forms.TabPage();
            this.dataGridViewPosition = new System.Windows.Forms.DataGridView();
            this.tabPageOrder = new System.Windows.Forms.TabPage();
            this.buttonExport = new System.Windows.Forms.Button();
            this.dataGridViewOrder = new System.Windows.Forms.DataGridView();
            this.buttonPageBefore = new System.Windows.Forms.Button();
            this.labelPage = new System.Windows.Forms.Label();
            this.buttonPageAfter = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStrategy)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPagePosition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPosition)).BeginInit();
            this.tabPageOrder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOrder)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.buttonStartStrategy);
            this.splitContainerMain.Panel1.Controls.Add(this.dataGridViewStrategy);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.tabControl1);
            this.splitContainerMain.Size = new System.Drawing.Size(884, 561);
            this.splitContainerMain.SplitterDistance = 225;
            this.splitContainerMain.SplitterWidth = 5;
            this.splitContainerMain.TabIndex = 0;
            // 
            // buttonStartStrategy
            // 
            this.buttonStartStrategy.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.buttonStartStrategy.ForeColor = System.Drawing.Color.Crimson;
            this.buttonStartStrategy.Location = new System.Drawing.Point(74, 12);
            this.buttonStartStrategy.Name = "buttonStartStrategy";
            this.buttonStartStrategy.Size = new System.Drawing.Size(93, 45);
            this.buttonStartStrategy.TabIndex = 1;
            this.buttonStartStrategy.Text = "启动策略";
            this.buttonStartStrategy.UseVisualStyleBackColor = false;
            // 
            // dataGridViewStrategy
            // 
            this.dataGridViewStrategy.AllowUserToAddRows = false;
            this.dataGridViewStrategy.AllowUserToDeleteRows = false;
            this.dataGridViewStrategy.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewStrategy.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewStrategy.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewStrategy.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridViewStrategy.Location = new System.Drawing.Point(0, 77);
            this.dataGridViewStrategy.MultiSelect = false;
            this.dataGridViewStrategy.Name = "dataGridViewStrategy";
            this.dataGridViewStrategy.ReadOnly = true;
            this.dataGridViewStrategy.RowTemplate.Height = 23;
            this.dataGridViewStrategy.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewStrategy.Size = new System.Drawing.Size(225, 484);
            this.dataGridViewStrategy.TabIndex = 0;
            this.dataGridViewStrategy.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewStrategy_CellClick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPagePosition);
            this.tabControl1.Controls.Add(this.tabPageOrder);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(654, 561);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPagePosition
            // 
            this.tabPagePosition.Controls.Add(this.dataGridViewPosition);
            this.tabPagePosition.Location = new System.Drawing.Point(4, 26);
            this.tabPagePosition.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPagePosition.Name = "tabPagePosition";
            this.tabPagePosition.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPagePosition.Size = new System.Drawing.Size(646, 531);
            this.tabPagePosition.TabIndex = 0;
            this.tabPagePosition.Text = "持仓";
            this.tabPagePosition.UseVisualStyleBackColor = true;
            // 
            // dataGridViewPosition
            // 
            this.dataGridViewPosition.AllowUserToAddRows = false;
            this.dataGridViewPosition.AllowUserToDeleteRows = false;
            this.dataGridViewPosition.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewPosition.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewPosition.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPosition.Location = new System.Drawing.Point(3, 4);
            this.dataGridViewPosition.Name = "dataGridViewPosition";
            this.dataGridViewPosition.ReadOnly = true;
            this.dataGridViewPosition.RowTemplate.Height = 23;
            this.dataGridViewPosition.Size = new System.Drawing.Size(640, 523);
            this.dataGridViewPosition.TabIndex = 0;
            // 
            // tabPageOrder
            // 
            this.tabPageOrder.Controls.Add(this.buttonPageAfter);
            this.tabPageOrder.Controls.Add(this.labelPage);
            this.tabPageOrder.Controls.Add(this.buttonPageBefore);
            this.tabPageOrder.Controls.Add(this.buttonExport);
            this.tabPageOrder.Controls.Add(this.dataGridViewOrder);
            this.tabPageOrder.Location = new System.Drawing.Point(4, 26);
            this.tabPageOrder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageOrder.Name = "tabPageOrder";
            this.tabPageOrder.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageOrder.Size = new System.Drawing.Size(646, 531);
            this.tabPageOrder.TabIndex = 1;
            this.tabPageOrder.Text = "订单";
            this.tabPageOrder.UseVisualStyleBackColor = true;
            // 
            // buttonExport
            // 
            this.buttonExport.Location = new System.Drawing.Point(285, 8);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(75, 37);
            this.buttonExport.TabIndex = 1;
            this.buttonExport.Text = "导出数据";
            this.buttonExport.UseVisualStyleBackColor = true;
            // 
            // dataGridViewOrder
            // 
            this.dataGridViewOrder.AllowUserToAddRows = false;
            this.dataGridViewOrder.AllowUserToDeleteRows = false;
            this.dataGridViewOrder.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewOrder.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewOrder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOrder.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridViewOrder.Location = new System.Drawing.Point(3, 51);
            this.dataGridViewOrder.Name = "dataGridViewOrder";
            this.dataGridViewOrder.ReadOnly = true;
            this.dataGridViewOrder.RowTemplate.Height = 23;
            this.dataGridViewOrder.Size = new System.Drawing.Size(640, 476);
            this.dataGridViewOrder.TabIndex = 0;
            this.dataGridViewOrder.VirtualMode = true;
            // 
            // buttonPageBefore
            // 
            this.buttonPageBefore.Location = new System.Drawing.Point(6, 8);
            this.buttonPageBefore.Name = "buttonPageBefore";
            this.buttonPageBefore.Size = new System.Drawing.Size(60, 37);
            this.buttonPageBefore.TabIndex = 2;
            this.buttonPageBefore.Text = "上一页";
            this.buttonPageBefore.UseVisualStyleBackColor = true;
            // 
            // labelPage
            // 
            this.labelPage.AutoSize = true;
            this.labelPage.Location = new System.Drawing.Point(87, 18);
            this.labelPage.Name = "labelPage";
            this.labelPage.Size = new System.Drawing.Size(82, 17);
            this.labelPage.TabIndex = 4;
            this.labelPage.Text = "共0页，第0页";
            // 
            // buttonPageAfter
            // 
            this.buttonPageAfter.Location = new System.Drawing.Point(206, 7);
            this.buttonPageAfter.Name = "buttonPageAfter";
            this.buttonPageAfter.Size = new System.Drawing.Size(60, 37);
            this.buttonPageAfter.TabIndex = 5;
            this.buttonPageAfter.Text = "下一页";
            this.buttonPageAfter.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.splitContainerMain);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "StrategyTools";
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStrategy)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPagePosition.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPosition)).EndInit();
            this.tabPageOrder.ResumeLayout(false);
            this.tabPageOrder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOrder)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPagePosition;
        private System.Windows.Forms.TabPage tabPageOrder;
        private System.Windows.Forms.Button buttonStartStrategy;
        private System.Windows.Forms.DataGridView dataGridViewStrategy;
        private System.Windows.Forms.DataGridView dataGridViewPosition;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.DataGridView dataGridViewOrder;
        private System.Windows.Forms.Button buttonPageAfter;
        private System.Windows.Forms.Label labelPage;
        private System.Windows.Forms.Button buttonPageBefore;
    }
}


namespace MinFin.Report.WinForm7
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		 System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		 void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.Vw_TaxLiqReport_UnifiedBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.findemoDataSet = new MinFin.Report.WinForm7.findemoDataSet();
			this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
			this.Vw_TaxLiqReport_UnifiedTableAdapter = new MinFin.Report.WinForm7.findemoDataSetTableAdapters.Vw_TaxLiqReport_UnifiedTableAdapter();
			this.button1 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.Vw_TaxLiqReport_UnifiedBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.findemoDataSet)).BeginInit();
			this.SuspendLayout();
			// 
			// Vw_TaxLiqReport_UnifiedBindingSource
			// 
			this.Vw_TaxLiqReport_UnifiedBindingSource.DataMember = "Vw_TaxLiqReport_Unified";
			this.Vw_TaxLiqReport_UnifiedBindingSource.DataSource = this.findemoDataSet;
			// 
			// findemoDataSet
			// 
			this.findemoDataSet.DataSetName = "findemoDataSet";
			this.findemoDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// reportViewer1
			// 
			this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
			reportDataSource1.Name = "DataSet1";
			reportDataSource1.Value = this.Vw_TaxLiqReport_UnifiedBindingSource;
			this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
			this.reportViewer1.LocalReport.ReportEmbeddedResource = "MinFin.Report.WinForm7.Report3_Unified.rdlc";
			this.reportViewer1.Location = new System.Drawing.Point(0, 0);
			this.reportViewer1.Name = "reportViewer1";
			this.reportViewer1.Size = new System.Drawing.Size(784, 1024);
			this.reportViewer1.TabIndex = 0;
			// 
			// Vw_TaxLiqReport_UnifiedTableAdapter
			// 
			this.Vw_TaxLiqReport_UnifiedTableAdapter.ClearBeforeFill = true;
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point(919, 13);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "Exit";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button1;
			this.ClientSize = new System.Drawing.Size(784, 1024);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.reportViewer1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Form1";
			this.Text = "MinFin - Report3_Unified";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.Vw_TaxLiqReport_UnifiedBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.findemoDataSet)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		 Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
		 System.Windows.Forms.BindingSource Vw_TaxLiqReport_UnifiedBindingSource;
		 findemoDataSet findemoDataSet;
		 findemoDataSetTableAdapters.Vw_TaxLiqReport_UnifiedTableAdapter Vw_TaxLiqReport_UnifiedTableAdapter;
		 System.Windows.Forms.Button button1;
	}
}
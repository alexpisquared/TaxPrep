using System;
using System.Collections.Generic;
using Db.FinDemo.DbModel;

namespace HistoricalChartSet
{
  partial class WinFormChartUsrCtrl
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
     void InitializeComponent()
    {
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
      ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
      this.SuspendLayout();
      // 
      // chart1
      // 
      this.chart1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
      chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.WhiteSmoke;
      chartArea1.AxisX2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
      chartArea1.AxisX2.MajorGrid.LineColor = System.Drawing.Color.WhiteSmoke;
      chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.WhiteSmoke;
      chartArea1.AxisY2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
      chartArea1.AxisY2.MajorGrid.LineColor = System.Drawing.Color.WhiteSmoke;
      chartArea1.BorderColor = System.Drawing.Color.Red;
      chartArea1.Name = "ChartArea1";
      this.chart1.ChartAreas.Add(chartArea1);
      this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
      legend1.Name = "Legend1";
      this.chart1.Legends.Add(legend1);
      this.chart1.Location = new System.Drawing.Point(0, 0);
      this.chart1.Name = "chart1";
      series1.ChartArea = "ChartArea1";
      series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StepLine;
      series1.Legend = "Legend1";
      series1.MarkerSize = 15;
      series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
      series1.Name = "Series1";
      series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
      this.chart1.Series.Add(series1);
      this.chart1.Size = new System.Drawing.Size(499, 400);
      this.chart1.TabIndex = 0;
      this.chart1.Text = "chart1 text";
      this.chart1.GetToolTipText += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs>(this.chart1_GetToolTipText);
      this.chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseDown);
      this.chart1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseMove);
      // 
      // WinFormChartUsrCtrl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
      this.Controls.Add(this.chart1);
      this.Name = "WinFormChartUsrCtrl";
      this.Size = new System.Drawing.Size(499, 400);
      ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

     System.Windows.Forms.DataVisualization.Charting.Chart chart1;

  }
}

﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmKlimaatatlas
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmKlimaatatlas))
        btnExecute = New Button()
        prProgress = New ProgressBar()
        lblProgress = New Label()
        btnConfigFile = New Button()
        txtConfigFile = New TextBox()
        lblConfigFile = New Label()
        dlgOpenFile = New OpenFileDialog()
        MenuStrip1 = New MenuStrip()
        AboutToolStripMenuItem = New ToolStripMenuItem()
        dlgSaveFile = New SaveFileDialog()
        TabControl1 = New TabControl()
        tabSettings = New TabPage()
        tabMaatlatten = New TabPage()
        Label2 = New Label()
        cmbRekenregels = New ComboBox()
        MenuStrip1.SuspendLayout()
        TabControl1.SuspendLayout()
        tabSettings.SuspendLayout()
        tabMaatlatten.SuspendLayout()
        SuspendLayout()
        ' 
        ' btnExecute
        ' 
        btnExecute.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnExecute.Location = New Point(1146, 640)
        btnExecute.Margin = New Padding(3, 4, 3, 4)
        btnExecute.Name = "btnExecute"
        btnExecute.Size = New Size(125, 64)
        btnExecute.TabIndex = 0
        btnExecute.Text = "Uitvoeren"
        btnExecute.UseVisualStyleBackColor = True
        ' 
        ' prProgress
        ' 
        prProgress.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        prProgress.Location = New Point(14, 673)
        prProgress.Margin = New Padding(3, 4, 3, 4)
        prProgress.Name = "prProgress"
        prProgress.Size = New Size(1113, 31)
        prProgress.TabIndex = 1
        ' 
        ' lblProgress
        ' 
        lblProgress.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        lblProgress.AutoSize = True
        lblProgress.Location = New Point(14, 640)
        lblProgress.Name = "lblProgress"
        lblProgress.Size = New Size(65, 20)
        lblProgress.TabIndex = 2
        lblProgress.Text = "Progress"
        ' 
        ' btnConfigFile
        ' 
        btnConfigFile.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnConfigFile.Location = New Point(1213, 14)
        btnConfigFile.Margin = New Padding(3, 4, 3, 4)
        btnConfigFile.Name = "btnConfigFile"
        btnConfigFile.Size = New Size(27, 32)
        btnConfigFile.TabIndex = 5
        btnConfigFile.Text = ".."
        btnConfigFile.UseVisualStyleBackColor = True
        ' 
        ' txtConfigFile
        ' 
        txtConfigFile.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtConfigFile.Location = New Point(122, 15)
        txtConfigFile.Margin = New Padding(3, 4, 3, 4)
        txtConfigFile.Name = "txtConfigFile"
        txtConfigFile.Size = New Size(1081, 27)
        txtConfigFile.TabIndex = 4
        ' 
        ' lblConfigFile
        ' 
        lblConfigFile.AutoSize = True
        lblConfigFile.Location = New Point(6, 19)
        lblConfigFile.Name = "lblConfigFile"
        lblConfigFile.Size = New Size(81, 20)
        lblConfigFile.TabIndex = 3
        lblConfigFile.Text = "Config file:"
        ' 
        ' dlgOpenFile
        ' 
        dlgOpenFile.FileName = "dlgOpenFile"
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.ImageScalingSize = New Size(24, 24)
        MenuStrip1.Items.AddRange(New ToolStripItem() {AboutToolStripMenuItem})
        MenuStrip1.Location = New Point(0, 0)
        MenuStrip1.Name = "MenuStrip1"
        MenuStrip1.Padding = New Padding(5, 1, 0, 1)
        MenuStrip1.Size = New Size(1285, 26)
        MenuStrip1.TabIndex = 4
        MenuStrip1.Text = "MenuStrip1"
        ' 
        ' AboutToolStripMenuItem
        ' 
        AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        AboutToolStripMenuItem.Size = New Size(49, 24)
        AboutToolStripMenuItem.Text = "Info"
        ' 
        ' TabControl1
        ' 
        TabControl1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        TabControl1.Controls.Add(tabSettings)
        TabControl1.Controls.Add(tabMaatlatten)
        TabControl1.Location = New Point(14, 36)
        TabControl1.Margin = New Padding(3, 4, 3, 4)
        TabControl1.Name = "TabControl1"
        TabControl1.SelectedIndex = 0
        TabControl1.Size = New Size(1257, 596)
        TabControl1.TabIndex = 6
        ' 
        ' tabSettings
        ' 
        tabSettings.Controls.Add(btnConfigFile)
        tabSettings.Controls.Add(lblConfigFile)
        tabSettings.Controls.Add(txtConfigFile)
        tabSettings.Location = New Point(4, 29)
        tabSettings.Margin = New Padding(3, 4, 3, 4)
        tabSettings.Name = "tabSettings"
        tabSettings.Padding = New Padding(3, 4, 3, 4)
        tabSettings.Size = New Size(1249, 563)
        tabSettings.TabIndex = 0
        tabSettings.Text = "Instellingen"
        tabSettings.UseVisualStyleBackColor = True
        ' 
        ' tabMaatlatten
        ' 
        tabMaatlatten.Controls.Add(Label2)
        tabMaatlatten.Controls.Add(cmbRekenregels)
        tabMaatlatten.Location = New Point(4, 29)
        tabMaatlatten.Margin = New Padding(3, 4, 3, 4)
        tabMaatlatten.Name = "tabMaatlatten"
        tabMaatlatten.Padding = New Padding(3, 4, 3, 4)
        tabMaatlatten.Size = New Size(1249, 563)
        tabMaatlatten.TabIndex = 1
        tabMaatlatten.Text = "Maatlatten"
        tabMaatlatten.UseVisualStyleBackColor = True
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(19, 20)
        Label2.Name = "Label2"
        Label2.Size = New Size(86, 20)
        Label2.TabIndex = 7
        Label2.Text = "Rekenregel:"
        ' 
        ' cmbRekenregels
        ' 
        cmbRekenregels.FormattingEnabled = True
        cmbRekenregels.Location = New Point(166, 13)
        cmbRekenregels.Margin = New Padding(3, 4, 3, 4)
        cmbRekenregels.Name = "cmbRekenregels"
        cmbRekenregels.Size = New Size(342, 28)
        cmbRekenregels.TabIndex = 0
        ' 
        ' frmKlimaatatlas
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1285, 720)
        Controls.Add(TabControl1)
        Controls.Add(lblProgress)
        Controls.Add(prProgress)
        Controls.Add(btnExecute)
        Controls.Add(MenuStrip1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MainMenuStrip = MenuStrip1
        Margin = New Padding(3, 4, 3, 4)
        Name = "frmKlimaatatlas"
        Text = "Klimaatatlas"
        WindowState = FormWindowState.Maximized
        MenuStrip1.ResumeLayout(False)
        MenuStrip1.PerformLayout()
        TabControl1.ResumeLayout(False)
        tabSettings.ResumeLayout(False)
        tabSettings.PerformLayout()
        tabMaatlatten.ResumeLayout(False)
        tabMaatlatten.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents btnExecute As Button
    Friend WithEvents prProgress As ProgressBar
    Friend WithEvents lblProgress As Label
    Friend WithEvents btnConfigFile As Button
    Friend WithEvents txtConfigFile As TextBox
    Friend WithEvents lblConfigFile As Label
    Friend WithEvents dlgOpenFile As OpenFileDialog
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents dlgSaveFile As SaveFileDialog
    Friend WithEvents Flowcharts As GroupBox
    Friend WithEvents pnlFlowchart As Panel
    Friend WithEvents cmbIndicators As ComboBox
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents tabSettings As TabPage
    Friend WithEvents tabMaatlatten As TabPage
    Friend WithEvents Label2 As Label
    Friend WithEvents cmbRekenregels As ComboBox
    Friend WithEvents AboutToolStripMenuItem As ToolStripMenuItem
End Class

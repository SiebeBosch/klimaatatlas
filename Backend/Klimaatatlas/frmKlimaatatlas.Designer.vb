<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
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
        Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(frmKlimaatatlas))
        btnExecute = New Button()
        prProgress = New ProgressBar()
        lblProgress = New Label()
        btnConfigFile = New Button()
        txtConfigFile = New TextBox()
        lblConfigFile = New Label()
        btnDatabase = New Button()
        lblDatabase = New Label()
        txtDatabase = New TextBox()
        dlgOpenFile = New OpenFileDialog()
        MenuStrip1 = New MenuStrip()
        ToolsToolStripMenuItem = New ToolStripMenuItem()
        GISToolStripMenuItem = New ToolStripMenuItem()
        SpatialJoinToolStripMenuItem = New ToolStripMenuItem()
        dlgSaveFile = New SaveFileDialog()
        TabControl1 = New TabControl()
        tabSettings = New TabPage()
        tabMaatlatten = New TabPage()
        Label2 = New Label()
        cmbRekenregels = New ComboBox()
        tabRekenregels = New TabPage()
        MenuStrip1.SuspendLayout()
        TabControl1.SuspendLayout()
        tabSettings.SuspendLayout()
        tabMaatlatten.SuspendLayout()
        SuspendLayout()
        ' 
        ' btnExecute
        ' 
        btnExecute.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnExecute.Location = New Point(1003, 480)
        btnExecute.Name = "btnExecute"
        btnExecute.Size = New Size(109, 48)
        btnExecute.TabIndex = 0
        btnExecute.Text = "Execute"
        btnExecute.UseVisualStyleBackColor = True
        ' 
        ' prProgress
        ' 
        prProgress.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        prProgress.Location = New Point(12, 505)
        prProgress.Name = "prProgress"
        prProgress.Size = New Size(974, 23)
        prProgress.TabIndex = 1
        ' 
        ' lblProgress
        ' 
        lblProgress.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        lblProgress.AutoSize = True
        lblProgress.Location = New Point(12, 480)
        lblProgress.Name = "lblProgress"
        lblProgress.Size = New Size(52, 15)
        lblProgress.TabIndex = 2
        lblProgress.Text = "Progress"
        ' 
        ' btnConfigFile
        ' 
        btnConfigFile.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnConfigFile.Location = New Point(1062, 41)
        btnConfigFile.Name = "btnConfigFile"
        btnConfigFile.Size = New Size(24, 24)
        btnConfigFile.TabIndex = 5
        btnConfigFile.Text = ".."
        btnConfigFile.UseVisualStyleBackColor = True
        ' 
        ' txtConfigFile
        ' 
        txtConfigFile.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtConfigFile.Location = New Point(89, 42)
        txtConfigFile.Name = "txtConfigFile"
        txtConfigFile.Size = New Size(966, 23)
        txtConfigFile.TabIndex = 4
        ' 
        ' lblConfigFile
        ' 
        lblConfigFile.AutoSize = True
        lblConfigFile.Location = New Point(6, 45)
        lblConfigFile.Name = "lblConfigFile"
        lblConfigFile.Size = New Size(65, 15)
        lblConfigFile.TabIndex = 3
        lblConfigFile.Text = "Config file:"
        ' 
        ' btnDatabase
        ' 
        btnDatabase.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnDatabase.Location = New Point(1062, 11)
        btnDatabase.Name = "btnDatabase"
        btnDatabase.Size = New Size(24, 24)
        btnDatabase.TabIndex = 2
        btnDatabase.Text = ".."
        btnDatabase.UseVisualStyleBackColor = True
        ' 
        ' lblDatabase
        ' 
        lblDatabase.AutoSize = True
        lblDatabase.Location = New Point(6, 15)
        lblDatabase.Name = "lblDatabase"
        lblDatabase.Size = New Size(77, 15)
        lblDatabase.TabIndex = 1
        lblDatabase.Text = "Database file:"
        ' 
        ' txtDatabase
        ' 
        txtDatabase.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtDatabase.Location = New Point(89, 12)
        txtDatabase.Name = "txtDatabase"
        txtDatabase.Size = New Size(966, 23)
        txtDatabase.TabIndex = 0
        ' 
        ' dlgOpenFile
        ' 
        dlgOpenFile.FileName = "dlgOpenFile"
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.ImageScalingSize = New Size(24, 24)
        MenuStrip1.Items.AddRange(New ToolStripItem() {ToolsToolStripMenuItem})
        MenuStrip1.Location = New Point(0, 0)
        MenuStrip1.Name = "MenuStrip1"
        MenuStrip1.Padding = New Padding(4, 1, 0, 1)
        MenuStrip1.Size = New Size(1124, 24)
        MenuStrip1.TabIndex = 4
        MenuStrip1.Text = "MenuStrip1"
        ' 
        ' ToolsToolStripMenuItem
        ' 
        ToolsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {GISToolStripMenuItem})
        ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        ToolsToolStripMenuItem.Size = New Size(46, 22)
        ToolsToolStripMenuItem.Text = "Tools"
        ' 
        ' GISToolStripMenuItem
        ' 
        GISToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {SpatialJoinToolStripMenuItem})
        GISToolStripMenuItem.Name = "GISToolStripMenuItem"
        GISToolStripMenuItem.Size = New Size(91, 22)
        GISToolStripMenuItem.Text = "GIS"
        ' 
        ' SpatialJoinToolStripMenuItem
        ' 
        SpatialJoinToolStripMenuItem.Name = "SpatialJoinToolStripMenuItem"
        SpatialJoinToolStripMenuItem.Size = New Size(133, 22)
        SpatialJoinToolStripMenuItem.Text = "Spatial Join"
        ' 
        ' TabControl1
        ' 
        TabControl1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        TabControl1.Controls.Add(tabSettings)
        TabControl1.Controls.Add(tabMaatlatten)
        TabControl1.Controls.Add(tabRekenregels)
        TabControl1.Location = New Point(12, 27)
        TabControl1.Name = "TabControl1"
        TabControl1.SelectedIndex = 0
        TabControl1.Size = New Size(1100, 447)
        TabControl1.TabIndex = 6
        ' 
        ' tabSettings
        ' 
        tabSettings.Controls.Add(txtDatabase)
        tabSettings.Controls.Add(lblDatabase)
        tabSettings.Controls.Add(btnDatabase)
        tabSettings.Controls.Add(btnConfigFile)
        tabSettings.Controls.Add(lblConfigFile)
        tabSettings.Controls.Add(txtConfigFile)
        tabSettings.Location = New Point(4, 24)
        tabSettings.Name = "tabSettings"
        tabSettings.Padding = New Padding(3)
        tabSettings.Size = New Size(1092, 419)
        tabSettings.TabIndex = 0
        tabSettings.Text = "Instellingen"
        tabSettings.UseVisualStyleBackColor = True
        ' 
        ' tabMaatlatten
        ' 
        tabMaatlatten.Controls.Add(Label2)
        tabMaatlatten.Controls.Add(cmbRekenregels)
        tabMaatlatten.Location = New Point(4, 24)
        tabMaatlatten.Name = "tabMaatlatten"
        tabMaatlatten.Padding = New Padding(3)
        tabMaatlatten.Size = New Size(1092, 419)
        tabMaatlatten.TabIndex = 1
        tabMaatlatten.Text = "Maatlatten"
        tabMaatlatten.UseVisualStyleBackColor = True
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(17, 15)
        Label2.Name = "Label2"
        Label2.Size = New Size(68, 15)
        Label2.TabIndex = 7
        Label2.Text = "Rekenregel:"
        ' 
        ' cmbRekenregels
        ' 
        cmbRekenregels.FormattingEnabled = True
        cmbRekenregels.Location = New Point(145, 10)
        cmbRekenregels.Name = "cmbRekenregels"
        cmbRekenregels.Size = New Size(300, 23)
        cmbRekenregels.TabIndex = 0
        ' 
        ' tabRekenregels
        ' 
        tabRekenregels.Location = New Point(4, 24)
        tabRekenregels.Name = "tabRekenregels"
        tabRekenregels.Size = New Size(1092, 419)
        tabRekenregels.TabIndex = 2
        tabRekenregels.Text = "Rekenregels"
        tabRekenregels.UseVisualStyleBackColor = True
        ' 
        ' frmKlimaatatlas
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1124, 540)
        Controls.Add(TabControl1)
        Controls.Add(lblProgress)
        Controls.Add(prProgress)
        Controls.Add(btnExecute)
        Controls.Add(MenuStrip1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MainMenuStrip = MenuStrip1
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
    Friend WithEvents btnDatabase As Button
    Friend WithEvents lblDatabase As Label
    Friend WithEvents txtDatabase As TextBox
    Friend WithEvents dlgOpenFile As OpenFileDialog
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents ToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GISToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SpatialJoinToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents dlgSaveFile As SaveFileDialog
    Friend WithEvents Flowcharts As GroupBox
    Friend WithEvents pnlFlowchart As Panel
    Friend WithEvents cmbIndicators As ComboBox
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents tabSettings As TabPage
    Friend WithEvents tabRekenregels As TabPage
    Friend WithEvents tabMaatlatten As TabPage
    Friend WithEvents Label2 As Label
    Friend WithEvents cmbRekenregels As ComboBox
End Class

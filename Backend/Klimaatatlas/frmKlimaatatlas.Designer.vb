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
        GroupBox1 = New GroupBox()
        Label1 = New Label()
        btnResultsFile = New Button()
        txtResultsFile = New TextBox()
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
        Flowcharts = New GroupBox()
        cmbIndicators = New ComboBox()
        Label2 = New Label()
        pnlFlowchart = New Panel()
        GroupBox1.SuspendLayout()
        MenuStrip1.SuspendLayout()
        Flowcharts.SuspendLayout()
        SuspendLayout()
        ' 
        ' btnExecute
        ' 
        btnExecute.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnExecute.Location = New Point(1433, 800)
        btnExecute.Margin = New Padding(4, 5, 4, 5)
        btnExecute.Name = "btnExecute"
        btnExecute.Size = New Size(156, 80)
        btnExecute.TabIndex = 0
        btnExecute.Text = "Execute"
        btnExecute.UseVisualStyleBackColor = True
        ' 
        ' prProgress
        ' 
        prProgress.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        prProgress.Location = New Point(17, 842)
        prProgress.Margin = New Padding(4, 5, 4, 5)
        prProgress.Name = "prProgress"
        prProgress.Size = New Size(1391, 38)
        prProgress.TabIndex = 1
        ' 
        ' lblProgress
        ' 
        lblProgress.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        lblProgress.AutoSize = True
        lblProgress.Location = New Point(17, 800)
        lblProgress.Margin = New Padding(4, 0, 4, 0)
        lblProgress.Name = "lblProgress"
        lblProgress.Size = New Size(81, 25)
        lblProgress.TabIndex = 2
        lblProgress.Text = "Progress"
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        GroupBox1.Controls.Add(Label1)
        GroupBox1.Controls.Add(btnResultsFile)
        GroupBox1.Controls.Add(txtResultsFile)
        GroupBox1.Controls.Add(btnConfigFile)
        GroupBox1.Controls.Add(txtConfigFile)
        GroupBox1.Controls.Add(lblConfigFile)
        GroupBox1.Controls.Add(btnDatabase)
        GroupBox1.Controls.Add(lblDatabase)
        GroupBox1.Controls.Add(txtDatabase)
        GroupBox1.Location = New Point(17, 58)
        GroupBox1.Margin = New Padding(4, 5, 4, 5)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Padding = New Padding(4, 5, 4, 5)
        GroupBox1.Size = New Size(1571, 220)
        GroupBox1.TabIndex = 3
        GroupBox1.TabStop = False
        GroupBox1.Text = "Settings"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(16, 152)
        Label1.Margin = New Padding(4, 0, 4, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(99, 25)
        Label1.TabIndex = 8
        Label1.Text = "Results file:"
        ' 
        ' btnResultsFile
        ' 
        btnResultsFile.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnResultsFile.Location = New Point(1526, 143)
        btnResultsFile.Margin = New Padding(4, 5, 4, 5)
        btnResultsFile.Name = "btnResultsFile"
        btnResultsFile.Size = New Size(34, 40)
        btnResultsFile.TabIndex = 7
        btnResultsFile.Text = ".."
        btnResultsFile.UseVisualStyleBackColor = True
        ' 
        ' txtResultsFile
        ' 
        txtResultsFile.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtResultsFile.Location = New Point(134, 148)
        txtResultsFile.Margin = New Padding(4, 5, 4, 5)
        txtResultsFile.Name = "txtResultsFile"
        txtResultsFile.Size = New Size(1384, 31)
        txtResultsFile.TabIndex = 6
        ' 
        ' btnConfigFile
        ' 
        btnConfigFile.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnConfigFile.Location = New Point(1529, 95)
        btnConfigFile.Margin = New Padding(4, 5, 4, 5)
        btnConfigFile.Name = "btnConfigFile"
        btnConfigFile.Size = New Size(34, 40)
        btnConfigFile.TabIndex = 5
        btnConfigFile.Text = ".."
        btnConfigFile.UseVisualStyleBackColor = True
        ' 
        ' txtConfigFile
        ' 
        txtConfigFile.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtConfigFile.Location = New Point(134, 95)
        txtConfigFile.Margin = New Padding(4, 5, 4, 5)
        txtConfigFile.Name = "txtConfigFile"
        txtConfigFile.Size = New Size(1384, 31)
        txtConfigFile.TabIndex = 4
        ' 
        ' lblConfigFile
        ' 
        lblConfigFile.AutoSize = True
        lblConfigFile.Location = New Point(16, 100)
        lblConfigFile.Margin = New Padding(4, 0, 4, 0)
        lblConfigFile.Name = "lblConfigFile"
        lblConfigFile.Size = New Size(97, 25)
        lblConfigFile.TabIndex = 3
        lblConfigFile.Text = "Config file:"
        ' 
        ' btnDatabase
        ' 
        btnDatabase.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnDatabase.Location = New Point(1529, 45)
        btnDatabase.Margin = New Padding(4, 5, 4, 5)
        btnDatabase.Name = "btnDatabase"
        btnDatabase.Size = New Size(34, 40)
        btnDatabase.TabIndex = 2
        btnDatabase.Text = ".."
        btnDatabase.UseVisualStyleBackColor = True
        ' 
        ' lblDatabase
        ' 
        lblDatabase.AutoSize = True
        lblDatabase.Location = New Point(16, 50)
        lblDatabase.Margin = New Padding(4, 0, 4, 0)
        lblDatabase.Name = "lblDatabase"
        lblDatabase.Size = New Size(118, 25)
        lblDatabase.TabIndex = 1
        lblDatabase.Text = "Database file:"
        ' 
        ' txtDatabase
        ' 
        txtDatabase.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtDatabase.Location = New Point(134, 45)
        txtDatabase.Margin = New Padding(4, 5, 4, 5)
        txtDatabase.Name = "txtDatabase"
        txtDatabase.Size = New Size(1384, 31)
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
        MenuStrip1.Size = New Size(1606, 33)
        MenuStrip1.TabIndex = 4
        MenuStrip1.Text = "MenuStrip1"
        ' 
        ' ToolsToolStripMenuItem
        ' 
        ToolsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {GISToolStripMenuItem})
        ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        ToolsToolStripMenuItem.Size = New Size(69, 29)
        ToolsToolStripMenuItem.Text = "Tools"
        ' 
        ' GISToolStripMenuItem
        ' 
        GISToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {SpatialJoinToolStripMenuItem})
        GISToolStripMenuItem.Name = "GISToolStripMenuItem"
        GISToolStripMenuItem.Size = New Size(141, 34)
        GISToolStripMenuItem.Text = "GIS"
        ' 
        ' SpatialJoinToolStripMenuItem
        ' 
        SpatialJoinToolStripMenuItem.Name = "SpatialJoinToolStripMenuItem"
        SpatialJoinToolStripMenuItem.Size = New Size(203, 34)
        SpatialJoinToolStripMenuItem.Text = "Spatial Join"
        ' 
        ' Flowcharts
        ' 
        Flowcharts.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Flowcharts.Controls.Add(cmbIndicators)
        Flowcharts.Controls.Add(Label2)
        Flowcharts.Controls.Add(pnlFlowchart)
        Flowcharts.Location = New Point(17, 288)
        Flowcharts.Margin = New Padding(4, 5, 4, 5)
        Flowcharts.Name = "Flowcharts"
        Flowcharts.Padding = New Padding(4, 5, 4, 5)
        Flowcharts.Size = New Size(1571, 502)
        Flowcharts.TabIndex = 5
        Flowcharts.TabStop = False
        Flowcharts.Text = "Flowchart"
        ' 
        ' cmbIndicators
        ' 
        cmbIndicators.AutoCompleteCustomSource.AddRange(New String() {"Kroos"})
        cmbIndicators.FormattingEnabled = True
        cmbIndicators.Items.AddRange(New Object() {"Kroos"})
        cmbIndicators.Location = New Point(134, 50)
        cmbIndicators.Margin = New Padding(4, 5, 4, 5)
        cmbIndicators.Name = "cmbIndicators"
        cmbIndicators.Size = New Size(181, 33)
        cmbIndicators.TabIndex = 10
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(16, 55)
        Label2.Margin = New Padding(4, 0, 4, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(86, 25)
        Label2.TabIndex = 9
        Label2.Text = "Indicator:"
        ' 
        ' pnlFlowchart
        ' 
        pnlFlowchart.Location = New Point(326, 37)
        pnlFlowchart.Margin = New Padding(4, 5, 4, 5)
        pnlFlowchart.Name = "pnlFlowchart"
        pnlFlowchart.Size = New Size(1234, 455)
        pnlFlowchart.TabIndex = 0
        ' 
        ' frmKlimaatatlas
        ' 
        AutoScaleDimensions = New SizeF(10F, 25F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1606, 900)
        Controls.Add(Flowcharts)
        Controls.Add(GroupBox1)
        Controls.Add(lblProgress)
        Controls.Add(prProgress)
        Controls.Add(btnExecute)
        Controls.Add(MenuStrip1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MainMenuStrip = MenuStrip1
        Margin = New Padding(4, 5, 4, 5)
        Name = "frmKlimaatatlas"
        Text = "Klimaatatlas"
        WindowState = FormWindowState.Maximized
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        MenuStrip1.ResumeLayout(False)
        MenuStrip1.PerformLayout()
        Flowcharts.ResumeLayout(False)
        Flowcharts.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents btnExecute As Button
    Friend WithEvents prProgress As ProgressBar
    Friend WithEvents lblProgress As Label
    Friend WithEvents GroupBox1 As GroupBox
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
    Friend WithEvents Label1 As Label
    Friend WithEvents btnResultsFile As Button
    Friend WithEvents txtResultsFile As TextBox
    Friend WithEvents dlgSaveFile As SaveFileDialog
    Friend WithEvents Flowcharts As GroupBox
    Friend WithEvents Label2 As Label
    Friend WithEvents pnlFlowchart As Panel
    Friend WithEvents cmbIndicators As ComboBox
End Class

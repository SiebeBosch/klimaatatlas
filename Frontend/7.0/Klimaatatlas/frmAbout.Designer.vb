<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAbout
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAbout))
        lblVersion = New Label()
        Label1 = New Label()
        Label2 = New Label()
        Label3 = New Label()
        Label4 = New Label()
        lblLink = New LinkLabel()
        SuspendLayout()
        ' 
        ' lblVersion
        ' 
        lblVersion.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblVersion.AutoSize = True
        lblVersion.Location = New Point(215, 69)
        lblVersion.Name = "lblVersion"
        lblVersion.Size = New Size(57, 20)
        lblVersion.TabIndex = 0
        lblVersion.Text = "Version"
        lblVersion.TextAlign = ContentAlignment.TopCenter
        ' 
        ' Label1
        ' 
        Label1.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Label1.AutoSize = True
        Label1.Location = New Point(198, 31)
        Label1.Name = "Label1"
        Label1.Size = New Size(91, 20)
        Label1.TabIndex = 1
        Label1.Text = "Klimaatatlas"
        Label1.TextAlign = ContentAlignment.TopCenter
        ' 
        ' Label2
        ' 
        Label2.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Label2.AutoSize = True
        Label2.Location = New Point(93, 117)
        Label2.Name = "Label2"
        Label2.Size = New Size(301, 20)
        Label2.TabIndex = 2
        Label2.Text = "Vervaardigd door Siebe Bosch Hydroconsult"
        Label2.TextAlign = ContentAlignment.TopCenter
        ' 
        ' Label3
        ' 
        Label3.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Label3.AutoSize = True
        Label3.Location = New Point(71, 144)
        Label3.Name = "Label3"
        Label3.Size = New Size(344, 20)
        Label3.TabIndex = 3
        Label3.Text = "In opdracht van Hoogheemraadschap van Rijnland"
        Label3.TextAlign = ContentAlignment.TopCenter
        ' 
        ' Label4
        ' 
        Label4.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Label4.AutoSize = True
        Label4.Location = New Point(125, 191)
        Label4.Name = "Label4"
        Label4.Size = New Size(237, 20)
        Label4.TabIndex = 4
        Label4.Text = "Opensource onder GPL-3.0 licentie"
        Label4.TextAlign = ContentAlignment.TopCenter
        ' 
        ' lblLink
        ' 
        lblLink.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblLink.AutoSize = True
        lblLink.Location = New Point(207, 222)
        lblLink.Name = "lblLink"
        lblLink.Size = New Size(73, 20)
        lblLink.TabIndex = 5
        lblLink.TabStop = True
        lblLink.Text = "broncode"
        lblLink.TextAlign = ContentAlignment.TopCenter
        ' 
        ' frmAbout
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(522, 264)
        Controls.Add(lblLink)
        Controls.Add(Label4)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(lblVersion)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(3, 4, 3, 4)
        Name = "frmAbout"
        Text = "Info"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents lblVersion As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents lblLink As LinkLabel
End Class

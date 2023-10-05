Imports System.IO
Imports Newtonsoft.Json.Linq
Imports System.Windows.Forms

Public Class frmKlimaatatlas
    Public Klimaatatlas As clsKlimaatatlas
    Public Flowchart As clsFlowchart
    Private Sub frmKlimaatatlas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Klimaatatlas = New clsKlimaatatlas()
        Klimaatatlas.SetProgressBar(prProgress, lblProgress)

        txtDatabase.Text = My.Settings.Database
        txtConfigFile.Text = My.Settings.Configfile
        txtResultsFile.Text = My.Settings.Resultsfile

        Initialize()

        If System.IO.File.Exists(txtDatabase.Text) AndAlso System.IO.File.Exists(txtConfigFile.Text) Then
            ReadConfiguration()
        End If

        ' Assuming you have a TabControl named TabControl1 with at least one TabPage
        AddHandler TabControl1.TabPages(1).Paint, AddressOf Benchmarks_Paint

    End Sub
    Private Sub Benchmarks_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint

        ' Initialize position
        Dim vPos As Integer = 50
        Dim labelWidth As Integer = 100  ' Set your desired label width

        'for now, pick the first rule to render
        If Klimaatatlas.Rules.ContainsKey(cmbRekenregels.Text.Trim.ToUpper) Then
            Dim Rule As clsRule = Klimaatatlas.Rules.Item(cmbRekenregels.Text.Trim.ToUpper)
            For Each Benchmark As clsBenchmark In Rule.Benchmarks.Values
                ' Create and configure Label
                Dim label As New Label()
                label.Text = Benchmark.Name  ' Assuming Name is a property of clsBenchmark
                label.Location = New Point(20, vPos)
                label.Width = labelWidth
                label.TextAlign = ContentAlignment.MiddleLeft


                ' Add the label to the TabPage's Controls collection
                TabControl1.TabPages(1).Controls.Add(label)

                ' Set position for color scale rectangle (next to the label)
                Dim rect As New Rectangle(40 + labelWidth + 5, vPos, 300, 20) ' Adjust the starting X position and width as necessary
                Benchmark.colorScale.DrawColorScale(e, rect)  ' Assuming DrawColorScale is a method that draws the color scale

                ' Increment position for next item
                vPos += 50  ' Adjust as necessary for proper spacing
            Next
        End If

    End Sub



    Private Sub DrawingPanel_Paint(sender As Object, e As PaintEventArgs) Handles pnlFlowchart.Paint
        If Flowchart IsNot Nothing Then
            Flowchart.DrawFlowchart(e.Graphics)
        End If
    End Sub

    Private Sub Initialize()
        'set the database connection, read the configuration file and start processing each rule
        Klimaatatlas.SetProgressBar(prProgress, lblProgress)
        Klimaatatlas.SetDatabaseConnection(txtDatabase.Text)
        Klimaatatlas.UpgradeDatabase()
    End Sub

    Private Sub ReadConfiguration()
        Klimaatatlas.ReadConfigurationFile(txtConfigFile.Text)
        Klimaatatlas.PopulateRules(cmbRekenregels)
        Klimaatatlas.PopulateScenarios()

    End Sub

    Private Sub btnExecute_Click(sender As Object, e As EventArgs) Handles btnExecute.Click
        'store our paths for the next time
        My.Settings.Database = txtDatabase.Text
        My.Settings.Configfile = txtConfigFile.Text
        My.Settings.Resultsfile = txtResultsFile.Text
        My.Settings.Save()

        Call ReadConfiguration()

        Klimaatatlas.readFeaturesDataset()
        'Klimaatatlas.SetAndInitializeRatingFields()         'add two fields to our dataset for storing the final rating and result_text

        Klimaatatlas.ProcessRules()

        'write the results to a new shapefile
        If System.IO.File.Exists(txtResultsFile.Text) Then Klimaatatlas.Generalfunctions.DeleteShapeFile(txtResultsFile.Text)
        Klimaatatlas.ExportResultsToShapefile(txtResultsFile.Text)

    End Sub

    Private Sub btnDatabase_Click(sender As Object, e As EventArgs) Handles btnDatabase.Click
        dlgOpenFile.Filter = "SQLite|*.db"
        Dim res As DialogResult = dlgOpenFile.ShowDialog
        If res = DialogResult.OK Then
            txtDatabase.Text = dlgOpenFile.FileName
            Klimaatatlas.SetDatabaseConnection(dlgOpenFile.FileName) ' As New clsKlimaatatlas(jsonPath, connectionString, configContent)
            Klimaatatlas.UpgradeDatabase()
        End If
    End Sub

    Private Sub btnConfigFile_Click(sender As Object, e As EventArgs) Handles btnConfigFile.Click
        dlgOpenFile.Filter = "JSON|*.json"
        Dim res As DialogResult = dlgOpenFile.ShowDialog
        If res = DialogResult.OK Then
            txtConfigFile.Text = dlgOpenFile.FileName

            'remember this path for the next time we open our application
            My.Settings.Configfile = txtConfigFile.Text
            My.Settings.Save()

            'read the configuration
            ReadConfiguration()
        End If
    End Sub

    Private Sub btnResultsFile_Click(sender As Object, e As EventArgs) Handles btnResultsFile.Click
        dlgSaveFile.Filter = "ESRI Shapefile|*.shp"
        Dim res As DialogResult = dlgSaveFile.ShowDialog
        If res = DialogResult.OK Then
            txtResultsFile.Text = dlgSaveFile.FileName
        End If
    End Sub


    Private Sub cmbIndicators_SelectedValueChanged(sender As Object, e As EventArgs) Handles cmbIndicators.SelectedValueChanged
        'as a first test I would like to create a simple flowchart containing just two nodes and one connection
        Flowchart = New clsFlowchart()

        pnlFlowchart.Invalidate()
        pnlFlowchart.Update()


    End Sub

    Private Sub cmbIndicators_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbIndicators.SelectedIndexChanged

    End Sub

    Private Sub cmbRekenregels_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbRekenregels.SelectedIndexChanged
        ' Trigger Repaint
        Me.Refresh()
        ' Or you could invalidate and update the specific control where benchmarks are drawn
        ' YourControl.Invalidate()
        ' YourControl.Update()

    End Sub

End Class

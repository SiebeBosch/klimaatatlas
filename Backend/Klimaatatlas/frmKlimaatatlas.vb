Imports System.IO
Imports Newtonsoft.Json.Linq

Public Class frmKlimaatatlas
    Public Klimaatatlas As clsKlimaatatlas
    Private Sub frmKlimaatatlas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Klimaatatlas = New clsKlimaatatlas()
        Klimaatatlas.SetProgressBar(prProgress, lblProgress)

        txtDatabase.Text = My.Settings.Database
        txtConfigFile.Text = My.Settings.Configfile

        If System.IO.File.Exists(txtDatabase.Text) Then
            Klimaatatlas.SetDatabaseConnection(txtDatabase.Text)
            Klimaatatlas.UpgradeDatabase()
        End If
    End Sub

    Private Sub btnExecute_Click(sender As Object, e As EventArgs) Handles btnExecute.Click
        'store our paths for the next time
        My.Settings.Database = txtDatabase.Text
        My.Settings.Configfile = txtConfigFile.Text
        My.Settings.Save()

        'set the database connection, read the configuration file and start processing each rule
        Klimaatatlas.setProgressbar(prProgress, lblProgress)
        Klimaatatlas.SetDatabaseConnection(txtDatabase.Text)
        Klimaatatlas.ReadConfiguration(txtConfigFile.Text)
        Klimaatatlas.UpgradeDatabase()

        Klimaatatlas.readFeaturesDataset()

        Klimaatatlas.PopulateDatasets()
        Klimaatatlas.populateClassifications()
        Klimaatatlas.PopulateRules()

        Klimaatatlas.ProcessRules()

        'write the results to a new shapefile
        Klimaatatlas.ExportResultsToShapefile("c:\temp\results.shp")

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
        End If
    End Sub
End Class

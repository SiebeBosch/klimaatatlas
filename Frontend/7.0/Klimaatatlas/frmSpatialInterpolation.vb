Public Class frmSpatialInterpolation

    Private Klimaatatlas As clsKlimaatatlas

    Public Sub New(ByRef myKlimaatAtlas As clsKlimaatatlas)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Klimaatatlas = myKlimaatAtlas

    End Sub


    Private Sub frmSpatialInterpolation_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'load all settings from memory
        txtDatabase.Text = My.Settings.Database
        If System.IO.File.Exists(txtDatabase.Text) Then
            PrepareDatabase(txtDatabase.Text)
            PopulateDatabaseTableCombobox()
            If cmbDatabaseTable.Items.Contains(My.Settings.DatabaseTable) Then
                cmbDatabaseTable.SelectedItem = My.Settings.DatabaseTable
            End If
        End If

        txtGeopackage.Text = My.Settings.Geopackage
        If System.IO.File.Exists(txtGeopackage.Text) Then
            Dim gpkg As New clsGeoPackage(Klimaatatlas, txtGeopackage.Text)
            gpkg.PopulateComboboxWithLayerNames(cmbGeopackageLayer)
            If cmbGeopackageLayer.Items.Contains(My.Settings.GeopackageLayer) Then
                cmbGeopackageLayer.SelectedItem = My.Settings.GeopackageLayer
            End If
        End If

        chkInterpolationInsidePolygons.Checked = My.Settings.InterpolationInsidePolygons
        If chkInterpolationInsidePolygons.Checked Then
            txtPolygonSf.Text = My.Settings.PolygonSF
        End If

    End Sub

    Private Sub btnGeopackage_Click(sender As Object, e As EventArgs) Handles btnGeopackage.Click
        dlgOpenFile.Filter = "Geopackage|*.gpkg"
        Dim res As DialogResult = dlgOpenFile.ShowDialog
        If res = DialogResult.OK Then
            txtGeopackage.Text = dlgOpenFile.FileName

            'populate the combobox with the available tables from the geopackage
            Dim gpkg As New clsGeoPackage(Klimaatatlas, txtGeopackage.Text)
            gpkg.PopulateComboboxWithLayerNames(cmbGeopackageLayer)

        End If


    End Sub

    Public Function PrepareDatabase(path As String)
        Klimaatatlas.SetDatabaseConnection(path) ' As New clsKlimaatatlas(jsonPath, connectionString, configContent)
        Klimaatatlas.UpgradeDatabase()
    End Function

    Public Function PopulateDatabaseTableCombobox()
        'populate the combobox with the available tables from the database
        Dim tables As List(Of String) = Klimaatatlas.GetTablesFromDatabase()
        cmbDatabaseTable.Items.Clear()
        For Each table As String In tables
            cmbDatabaseTable.Items.Add(table)
        Next
    End Function

    Private Sub btnDatabase_Click(sender As Object, e As EventArgs) Handles btnDatabase.Click
        dlgOpenFile.Filter = "SQLite|*.db"
        Dim res As DialogResult = dlgOpenFile.ShowDialog
        If res = DialogResult.OK Then
            txtDatabase.Text = dlgOpenFile.FileName
            PrepareDatabase(txtDatabase.Text)
            PopulateDatabaseTableCombobox()
        End If
    End Sub


    Private Sub chkInterpolationInsidePolygons_Click(sender As Object, e As EventArgs) Handles chkInterpolationInsidePolygons.Click
        dlgOpenFile.Filter = "ESRI Shapefile|*.shp"
        If chkInterpolationInsidePolygons.Checked Then
            Dim res As DialogResult = dlgOpenFile.ShowDialog
            If res = DialogResult.OK Then
                txtPolygonSf.Text = dlgOpenFile.FileName
            End If
        End If
    End Sub

    Private Sub btnExecute_Click(sender As Object, e As EventArgs) Handles btnExecute.Click

        'store all settings for the next time
        My.Settings.Database = txtDatabase.Text
        My.Settings.DatabaseTable = cmbDatabaseTable.Text

        My.Settings.Geopackage = txtGeopackage.Text
        My.Settings.GeopackageLayer = cmbGeopackageLayer.SelectedItem

        My.Settings.InterpolationInsidePolygons = chkInterpolationInsidePolygons.Checked
        My.Settings.PolygonSF = txtPolygonSf.Text

        My.Settings.Save()

        dlgFolder.Description = "Select a temporary work dir for the operation."
        dlgFolder.ShowDialog()
        Dim tempFolder As String = dlgFolder.SelectedPath

        'first open the connection to the database
        Klimaatatlas.SetDatabaseConnection(txtDatabase.Text)

        'read the table from the database
        Dim query As String = "SELECT DISTINCT LOCATIONID, X, Y FROM " & cmbDatabaseTable.SelectedItem.ToString
        Dim dt As New DataTable
        Klimaatatlas.Generalfunctions.SQLiteQuery(Klimaatatlas.SQLiteCon, query, dt)

        'then read the geopackage
        Dim gpkg As New clsGeoPackage(Klimaatatlas, txtGeopackage.Text)
        Dim gpkgLayer As String = cmbGeopackageLayer.SelectedItem.ToString

        'convert the geopackage to a shapefile
        Dim Utils As New MapWinGIS.Utils
        Utils.OGR2OGR(txtGeopackage.Text, tempFolder, cmbGeopackageLayer.SelectedItem.ToString)


        'if required, read the polygon shapefile
        If chkInterpolationInsidePolygons.Checked Then
            'we must interpolate within polygons. first we will assign each point to a polygon
            Dim polygonSf As String = txtPolygonSf.Text
            Dim SF As New MapWinGIS.Shapefile
            SF.Open(polygonSf)
            SF.BeginPointInShapefile()

            'create a dictionary where each polygon gets its points assigned
            Dim Polygons As New Dictionary(Of Integer, List(Of clsIDXY)) 'key = polygon index, values = list of point indices
            For i = 0 To SF.NumShapes - 1
                Dim pointIDs As New List(Of clsIDXY)
                Polygons.Add(i, pointIDs)
            Next

            'loop over all the points and assign them to their polygon
            Dim polyidx As Integer
            For i = 0 To dt.Rows.Count - 1
                polyidx = SF.PointInShapefile(dt.Rows(i).Item("X"), dt.Rows(i).Item("Y"))
                If polyidx >= 0 Then
                    Polygons(polyidx).Add(New clsIDXY(dt.Rows(i).Item("LOCATIONID"), dt.Rows(i).Item("X"), dt.Rows(i).Item("Y")))
                End If
            Next

            SF.EndPointInShapefile()
            SF.Close()

            'now loop over all the polygons from the geopackage, find in which shapefile polygon they are located and interpolate the points that lie within that same polygon
            'since we're unable to read the features from our geopackage we have first converted it to a shapefile
            Dim SFGPKG As New MapWinGIS.Shapefile
            SFGPKG.Open(tempFolder & "\" & cmbGeopackageLayer.Text)
            For i = 0 To SF.NumShapes - 1
                'get the interior point of the polygon
                Dim interiorPoint As MapWinGIS.Point = SF.Shape(i).InteriorPoint

                'figure out inside wich polygon we are
                polyidx = SF.PointInShapefile(interiorPoint.x, interiorPoint.y)

                'now we can interpolate the points that lie within this polygon, using inverse distance weighting
                Dim points As List(Of clsIDXY) = Polygons(polyidx)


            Next




        Else
            'we must interpolate over the entire area
            MsgBox("Interpolatie zonder medeneming van polygonen is nog niet geïmplementeerd")
        End If





    End Sub

    Private Sub chkInterpolationInsidePolygons_CheckedChanged(sender As Object, e As EventArgs) Handles chkInterpolationInsidePolygons.CheckedChanged

    End Sub
End Class
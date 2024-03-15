Imports System.Data.Entity
Imports System.Data.SQLite
Imports System.Text
Imports System.Windows.Forms
Imports GeoLibrary.Model
Imports System
Imports GeoLibrary
Imports GeoLibrary.IO.Wkb
Imports GeoLibrary.IO.Wkt

Public Class clsGeoPackage

    Dim Path As String
    Friend con As SQLiteConnection
    Private Setup As clsKlimaatatlas

    Dim Dataset As clsDataset

    Public Enum enmGeopackageLayerTypeSelection
        both = 0
        geometry = 1
        non_geometry = 2
    End Enum

    Public Sub New(ByRef mySetup As clsKlimaatatlas)
        Setup = mySetup
    End Sub

    Public Sub New(ByRef mySetup As clsKlimaatatlas, myPath As String)
        Setup = mySetup
        Path = myPath

        'SqliteCon.ConnectionString = "Data Source=" & Database & ";Version=3;"
        con = New SQLiteConnection($"Data Source={Path};Version=3;")
    End Sub

    Public Sub PopulateComboboxWithLayerNames(ByRef myCombobox As ComboBox)
        Try
            If Not con.State = ConnectionState.Open Then con.Open()
            Using command As New SQLiteCommand("SELECT table_name FROM gpkg_geometry_columns;", con)
                Using reader As SQLiteDataReader = command.ExecuteReader()
                    myCombobox.Items.Clear()

                    While reader.Read()
                        Dim layerName As String = reader.GetString(0)
                        myCombobox.Items.Add(layerName)
                    End While
                End Using
            End Using

            con.Close()

        Catch ex As Exception
            MessageBox.Show("An error occurred while reading the GeoPackage file: " & ex.Message)
        End Try


    End Sub

    Public Function getGeometries(LayerName As String) As List(Of MapWinGIS.Shape)
        Dim geometries As New List(Of MapWinGIS.Shape)
        Try
            ' Open connection if not already open
            If Not con.State = ConnectionState.Open Then con.Open()

            ' SQL query to select the geometry column from the specified layer
            Dim commandText As String = $"SELECT geom FROM {LayerName};"

            Using command As New SQLiteCommand(commandText, con)
                Using reader As SQLiteDataReader = command.ExecuteReader()
                    While reader.Read()
                        ' Assuming geom is in the first column and is of type BLOB
                        Dim blob As Byte() = reader.GetValue(0)

                        'the blob contains more than just the geometry, it also contains the header and envelope
                        'we must pass only the geometry section to the parser
                        'in order to get the geometry we must skip the first 40 bytes of the blob
                        Dim headerlength As Integer = 40
                        Dim wkb As Byte() = New Byte(blob.Length - headerlength - 1) {}
                        Array.Copy(blob, headerlength, wkb, 0, blob.Length - headerlength - 1)

                        'display both
                        Debug.Print("blob is " & Setup.Generalfunctions.ByteArrayToHexString(blob))
                        Debug.Print("wkb is " & Setup.Generalfunctions.ByteArrayToHexString(wkb))


                        Dim geometry = WkbReader.Read(Setup.Generalfunctions.ByteArrayToHexString(wkb))
                        Dim wkt As String = geometry.ToWkt()

                        ' Now create and import the shape using the adjusted byte array
                        Dim myShape As New MapWinGIS.Shape
                        myShape.Create(MapWinGIS.ShpfileType.SHP_POLYGON)
                        myShape.ImportFromWKT(wkt)
                        geometries.Add(myShape)
                    End While
                End Using
            End Using

            con.Close()
        Catch ex As Exception
            MessageBox.Show("An error occurred while reading geometries: " & ex.Message)
        End Try
        Return geometries
    End Function


    Public Function getFieldTypeStr(LayerName As String, FieldName As String) As String
        Try
            If Not con.State = ConnectionState.Open Then con.Open()

            ' Query to get the field type from the selected table and field
            Dim commandText As String = $"PRAGMA table_info({LayerName});"

            Using command As New SQLiteCommand(commandText, con)
                Using reader As SQLiteDataReader = command.ExecuteReader()
                    While reader.Read()
                        ' The second column contains the field name, and the third contains the field type
                        Dim currentFieldName As String = reader.GetString(1)
                        If currentFieldName.Equals(FieldName, StringComparison.InvariantCultureIgnoreCase) Then
                            Dim fieldType As String = reader.GetString(2) ' Get the field type
                            Return fieldType
                        End If
                    End While
                End Using
            End Using

            con.Close()

            ' Return an empty string if the field was not found
            Return String.Empty
        Catch ex As Exception
            MessageBox.Show("An error occurred while reading the GeoPackage file: " & ex.Message)
            Return String.Empty
        End Try
    End Function

    Public Function ChangeFieldType(LayerName As String, FieldName As String, NewFieldType As String) As Boolean
        Try
            Me.Setup.Generalfunctions.UpdateProgressBar($"Changing field type for field {FieldName} in layer {LayerName} to {NewFieldType}", 0, 10, True)

            ' Open connection if necessary
            If Not con.State = ConnectionState.Open Then con.Open()

            ' Step 1: Add a temporary column with the new data type
            Dim addTempColumn As String = $"ALTER TABLE {LayerName} ADD COLUMN temp_column {NewFieldType};"
            Using command As New SQLiteCommand(addTempColumn, con)
                command.ExecuteNonQuery()
            End Using

            ' Step 2: Update the temporary column with the converted values
            Dim updateTempColumn As String = $"UPDATE {LayerName} SET temp_column = CAST({FieldName} AS {NewFieldType}) WHERE {FieldName} <> '' OR {FieldName} IS NOT NULL;"
            Using command As New SQLiteCommand(updateTempColumn, con)
                command.ExecuteNonQuery()
            End Using

            ' Step 3: Create a list of columns except the one to be changed
            Dim columns As New List(Of String)
            Dim commandText As String = $"PRAGMA table_info({LayerName});"
            Using command As New SQLiteCommand(commandText, con)
                Using reader As SQLiteDataReader = command.ExecuteReader()
                    While reader.Read()
                        Dim currentFieldName As String = reader.GetString(1)
                        Dim currentFieldType As String = reader.GetString(2)
                        If currentFieldName.Equals(FieldName, StringComparison.InvariantCultureIgnoreCase) Then
                            ' Replace the original field with the temporary one
                            columns.Add("temp_column AS " & FieldName)
                        ElseIf Not currentFieldName.Equals("temp_column", StringComparison.InvariantCultureIgnoreCase) Then
                            ' Keep all other fields
                            columns.Add(currentFieldName)
                        End If
                    End While
                End Using
            End Using

            ' Step 4: Create a new table with the new structure
            Dim tempTableName As String = "temp_" & LayerName
            Dim createTempTable As String = $"CREATE TABLE {tempTableName} AS SELECT {String.Join(", ", columns)} FROM {LayerName};"
            Using command As New SQLiteCommand(createTempTable, con)
                command.ExecuteNonQuery()
            End Using

            ' Step 5: Drop the original table
            Dim dropOriginalTable As String = $"DROP TABLE {LayerName};"
            Using command As New SQLiteCommand(dropOriginalTable, con)
                command.ExecuteNonQuery()
            End Using

            ' Step 6: Rename the temporary table to the original name
            Dim renameTempTable As String = $"ALTER TABLE {tempTableName} RENAME TO {LayerName};"
            Using command As New SQLiteCommand(renameTempTable, con)
                command.ExecuteNonQuery()
            End Using

            Me.Setup.Generalfunctions.UpdateProgressBar("", 10, 10, True)

            con.Close()

            Me.Setup.Generalfunctions.UpdateProgressBar($"Operation complete", 0, 10, True)

            Return True
        Catch ex As Exception
            MessageBox.Show("An error occurred while changing the field type: " & ex.Message)
            Return False
        End Try
    End Function


    Public Function PopulateComboboxWithLayerFieldNames(LayerName As String, ByRef myComboBox As ComboBox) As Boolean
        Try
            If Not con.State = ConnectionState.Open Then con.Open()

            ' Query to get all columns from the selected table using PRAGMA
            Dim commandText As String = $"PRAGMA table_info({LayerName});"

            Using command As New SQLiteCommand(commandText, con)
                Using reader As SQLiteDataReader = command.ExecuteReader()
                    myComboBox.Items.Clear()

                    While reader.Read()
                        ' The second column of the result set contains the field name
                        Dim fieldName As String = reader.GetString(1)
                        myComboBox.Items.Add(fieldName)
                    End While
                End Using
            End Using

            con.Close()
            Return True ' Successfully populated the ComboBox

        Catch ex As Exception
            MessageBox.Show("An error occurred while reading the GeoPackage file: " & ex.Message)
            Return False ' An error occurred
        End Try
    End Function

    Public Sub PopulateCheckedListBoxWithLayerNames(ByRef myCheckedListBox As CheckedListBox, LayerTypeSelection As enmGeopackageLayerTypeSelection)
        Try
            If Not con.State = ConnectionState.Open Then con.Open()

            Dim sqlQuery As String = ""

            Select Case LayerTypeSelection
                Case enmGeopackageLayerTypeSelection.geometry
                    sqlQuery = "SELECT table_name FROM gpkg_geometry_columns;"
                Case enmGeopackageLayerTypeSelection.non_geometry
                    sqlQuery = "SELECT table_name FROM gpkg_contents WHERE table_name NOT IN (SELECT table_name FROM gpkg_geometry_columns);"
                Case enmGeopackageLayerTypeSelection.both
                    sqlQuery = "SELECT table_name FROM gpkg_contents;"
            End Select

            Using command As New SQLiteCommand(sqlQuery, con)
                Using reader As SQLiteDataReader = command.ExecuteReader()
                    myCheckedListBox.Items.Clear()

                    While reader.Read()
                        Dim layerName As String = reader.GetString(0)
                        myCheckedListBox.Items.Add(layerName)
                    End While
                End Using
            End Using

            con.Close()

        Catch ex As Exception
            MessageBox.Show("An error occurred while reading the GeoPackage file: " & ex.Message)
        End Try
    End Sub


    Public Sub DeleteSelectedLayers(ByRef checkedListBox As CheckedListBox)
        Try
            If Not con.State = ConnectionState.Open Then con.Open()
            Dim i As Integer = 0
            Me.Setup.Generalfunctions.UpdateProgressBar("Removing layers from geopackage...", 0, 10, True)

            For Each layerName As String In checkedListBox.CheckedItems
                i += 1
                Me.Setup.Generalfunctions.UpdateProgressBar("", i, checkedListBox.CheckedItems.Count)
                ' Delete the layer from the geometry columns
                Using command As New SQLiteCommand($"DELETE FROM gpkg_geometry_columns WHERE table_name = @layerName;", con)
                    command.Parameters.AddWithValue("@layerName", layerName)
                    command.ExecuteNonQuery()
                End Using

                ' Drop the layer table
                Using command As New SQLiteCommand($"DROP TABLE IF EXISTS {layerName};", con)
                    command.ExecuteNonQuery()
                End Using
            Next

            'purge all deleted data
            VacuumDatabase(con)

            con.Close()
            Me.Setup.Generalfunctions.UpdateProgressBar("Operation complete.", 0, 10, True)

        Catch ex As Exception
            MessageBox.Show("An error occurred while deleting the layers: " & ex.Message)
        End Try
    End Sub

    Public Sub VacuumDatabase(ByVal SQLiteCon As SQLiteConnection)
        Try
            If SQLiteCon.State = ConnectionState.Closed Then
                SQLiteCon.Open()
            End If
            Using cmd As New SQLiteCommand("VACUUM;", SQLiteCon)
                cmd.ExecuteNonQuery()
            End Using

        Catch ex As Exception
            ' Handle any exceptions that occur during the vacuum process
        Finally
            If SQLiteCon.State = ConnectionState.Open Then
                SQLiteCon.Close()
            End If
        End Try
    End Sub

    Public Function ParseBlobToPolygonWkt(blob As Byte()) As String
        Try
            'this function parses the blob to a WKT string
            'notice that for now we only support polygons
            Dim position As Integer = 0

            ' Read the magic number 'GP'
            Dim magicNumberBytes() As Byte = {blob(position), blob(position + 1)}
            Dim magicNumber As String = BitConverter.ToString(magicNumberBytes)
            position += 2 ' Move past the 2 bytes of the magic number

            ' Read the version
            Dim version As Byte = blob(position)
            Dim versionHex As String = version.ToString("X2")
            position += 1 ' Move past the version byte

            ' Read the flags
            Dim flags As Byte = blob(position)
            Dim flagsHex As String = flags.ToString("X2")

            position += 1 ' Move past the flags byte

            ' Example: To check the endianess (most significant bit of flags)
            'If you Then 're working with binary data from a GeoPackage on a little-endian system
            '(which is common in personal computers using Intel or AMD processors),
            'understanding that the data is in little-endian format means you can directly
            'read the values from the file into memory without needing to reorder the bytes.
            'However, if you were to process this data on a big-endian system,
            'you would need to reverse the byte order of multi-byte fields to interpret the data correctly.
            Dim isLittleEndian As Boolean = (flags And &H1) = &H1

            'the next four bytes represent the SRID
            Dim sridBytes() As Byte = {blob(position), blob(position + 1), blob(position + 2), blob(position + 3)}
            Dim srid As Integer = BitConverter.ToInt32(sridBytes, 0)
            position += 4

            ' Read the envelope
            Dim envelope As New Envelope
            envelope.MinX = BitConverter.ToDouble(blob, position)
            position += 8
            envelope.MaxX = BitConverter.ToDouble(blob, position)
            position += 8
            envelope.MinY = BitConverter.ToDouble(blob, position)
            position += 8
            envelope.MaxY = BitConverter.ToDouble(blob, position)
            position += 8

            'immediately after the envelope, the endianness of the geometry is stored
            'this is the same as the endianness of the flags
            Dim isGeometryLittleEndian As Boolean = isLittleEndian
            position += 1

            ' Read the geometry type. This is a 4-byte unsigned integer
            Dim geometryType As Integer = BitConverter.ToInt32(blob, position)
            position += 4

            'read the number of linear rings
            Dim numRings As Integer = BitConverter.ToInt32(blob, position)
            position += 4

            'read the number of outer ring coordinates
            Dim numOuterRingCoordinates As Integer = BitConverter.ToInt32(blob, position)
            position += 4











            ' Skip header and envelope (assuming minimal envelope and little-endian format)
            position += 48 ' 8 bytes for header, 32 for envelope, 8 for type and ring count

            ' Read number of rings
            numRings = BitConverter.ToInt32(blob, position)
            position += 4

            Dim wkt As New StringBuilder("POLYGON(")

            For i As Integer = 0 To numRings - 1
                ' Read number of points in this ring
                Dim numPoints As Integer = BitConverter.ToInt32(blob, position)
                position += 4

                wkt.Append("(")
                For j As Integer = 0 To numPoints - 1
                    ' Read each point
                    Dim x As Double = BitConverter.ToDouble(blob, position)
                    position += 8
                    Dim y As Double = BitConverter.ToDouble(blob, position)
                    position += 8

                    wkt.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "{0} {1}", x, y)

                    If j < numPoints - 1 Then
                        wkt.Append(", ")
                    End If
                Next
                wkt.Append(")")

                If i < numRings - 1 Then
                    wkt.Append(", ")
                End If
            Next

            wkt.Append(")")

            Return wkt.ToString()
        Catch ex As Exception
            Me.Setup.Log.AddError("Error parsing geopackage geometry: " & ex.Message)
            Return String.Empty
        End Try

    End Function


    Public Sub DeleteUnselectedLayers(ByRef checkedListBox As CheckedListBox)
        Try
            If Not con.State = ConnectionState.Open Then con.Open()

            ' Determine the total number of layers
            Dim nLayers As Integer
            Using countCommand As New SQLiteCommand("SELECT COUNT(*) FROM gpkg_geometry_columns;", con)
                nLayers = Convert.ToInt32(countCommand.ExecuteScalar())
            End Using

            Dim i As Integer = 0
            Me.Setup.Generalfunctions.UpdateProgressBar("Processing layer selection...", 0, 10, True)

            ' Create a list of all selected layer names
            Dim selectedLayers As New List(Of String)
            For Each layerName As String In checkedListBox.CheckedItems
                selectedLayers.Add(layerName)
            Next

            ' Create a list of unselected layers
            Dim unselectedLayers As New List(Of String)
            'since we're deleting all unselected layers (hence exporting selected ones) we must iterate through ALL contents, not just the geometry kind 
            Using command As New SQLiteCommand("SELECT table_name FROM gpkg_contents;", con)
                Using reader As SQLiteDataReader = command.ExecuteReader()
                    While reader.Read()
                        Dim layerName As String = reader.GetString(0)
                        i += 1
                        Me.Setup.Generalfunctions.UpdateProgressBar("", i, nLayers, True)

                        ' Skip the layer if it's selected
                        If selectedLayers.Contains(layerName) Then Continue While

                        unselectedLayers.Add(layerName)
                    End While
                End Using
            End Using

            ' Delete unselected layers
            For Each layerName In unselectedLayers
                ' Delete from the geometry columns
                Using deleteCommand As New SQLiteCommand($"DELETE FROM gpkg_contents WHERE table_name = @layerName;", con)
                    deleteCommand.Parameters.AddWithValue("@layerName", layerName)
                    deleteCommand.ExecuteNonQuery()
                End Using

                ' Drop the unselected layer table
                Using dropCommand As New SQLiteCommand($"DROP TABLE IF EXISTS {layerName};", con)
                    dropCommand.ExecuteNonQuery()
                End Using
            Next

            'purge all deleted data
            VacuumDatabase(con)

            con.Close()
            Me.Setup.Generalfunctions.UpdateProgressBar("Operation complete.", 0, 10, True)
        Catch ex As Exception
            MessageBox.Show("An error occurred while deleting the unselected layers: " & ex.Message)
        End Try
    End Sub




End Class

Public Class Envelope
    Public Property MinX As Double
    Public Property MaxX As Double
    Public Property MinY As Double
    Public Property MaxY As Double

End Class


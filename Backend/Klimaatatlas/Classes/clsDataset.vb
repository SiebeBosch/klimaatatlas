Imports Klimaatatlas.clsGeneralFunctions
Imports MapWinGIS
Imports Newtonsoft.Json.Linq
Imports System.Data.SQLite
Imports System.Globalization
Imports System.IO
Public Class clsDataset

    Private Setup As clsKlimaatatlas
    Friend Comment As String
    Friend readingCompleted As Boolean = False
    Friend ID As String
    Friend dataType As enmDataType
    Friend storageType As enmStorageType
    Friend tablename As String
    Friend path As String
    Friend Fields As Dictionary(Of String, clsSQLiteField)            'key = the name of the field in our source dataset, to uppercase
    Friend Features As Dictionary(Of Integer, clsSpatialFeature)      'key = index number of our feature
    Friend Values As Object(,)                                        'first dimension = field idx, second dimension = feature idx

    Friend SF As MapWinGIS.Shapefile

    Public Sub New(ByRef myKlimaatatlas As clsKlimaatatlas)
        Setup = myKlimaatatlas
        Fields = New Dictionary(Of String, clsSQLiteField)
        Features = New Dictionary(Of Integer, clsSpatialFeature)
    End Sub

    Public Function getValue(FieldIdx As Integer, FeatureDatasetfeatureidx As Integer, JoinMethod As enmJoinMethod) As Object
        Try
            'IMPORTANT: the FeatureDatasetFeatureIdx is the index number from the FeatureDataset, which is NOT necessarily THIS dataset.
            'We might need a spatial join or a lookup in order to get the corresponding data from THIS dataset for the requested feature
            Dim Polygon As MapWinGIS.Shape
            Dim ShapeIdx As Integer

            If ID = Setup.featuresDataset.ID Then
                'no need for special lookup functions such as spatial joins or lookup tables
                'our current datset is also the feature dataset. We can simply lookup our requested value
                Return Values(FieldIdx, FeatureDatasetfeatureidx)
            Else
                Select Case JoinMethod
                    Case enmJoinMethod.feature_centerpoint_in_polygon
                        'we'll need to perform a point-in-polygon operation of our feature's centerpoint in this dataset
                        'if all's well this dataset is a polygon shapefile and it has been opened with the option 'BeginPointInShapefile acivated
                        Polygon = New MapWinGIS.Shape
                        Polygon.ImportFromWKT(Setup.featuresDataset.Features.Item(FeatureDatasetfeatureidx).WKT)
                        ShapeIdx = SF.PointInShapefile(Polygon.Center.x, Polygon.Center.y)

                        If ShapeIdx >= 0 Then
                            Return SF.CellValue(Fields.Values(FieldIdx).sourceFieldIdx, ShapeIdx)
                        Else
                            Return Nothing
                        End If
                    Case Else
                        Throw New Exception($"JoinMethod {JoinMethod.ToString} not supported.")
                End Select
            End If


        Catch ex As Exception
            Me.Setup.Log.AddError("Error in function getValue of class clsDataset: " & ex.Message)
            Return Nothing
        End Try



    End Function

    Public Function GetField(fieldname As String) As clsSQLiteField
        If Fields.ContainsKey(fieldname.Trim.ToUpper) Then
            Return Fields.Item(fieldname.Trim.ToUpper)
        Else
            Return Nothing
        End If
    End Function

    Public Function GetAddField(FieldName As String, FieldType As enmFieldType, DataType As enmSQLiteDataType) As clsSQLiteField

        If Not Fields.ContainsKey(FieldName.Trim.ToUpper) Then
            'create a new field
            Dim myField As New clsSQLiteField(FieldName, FieldType, DataType, Fields.Count)
            Fields.Add(FieldName.Trim.ToUpper, myField)
            'redim the values array in order to make place for values in this field
            ReDimPreserve(Values, Fields.Count)
        End If

        Return Fields.Item(FieldName.Trim.ToUpper)

    End Function

    Public Sub ReDimPreserve(ByRef arr As Object(,), newSize1 As Integer)
        ' Save the old array size
        Dim oldSize1 As Integer = arr.GetUpperBound(0) + 1
        Dim oldSize2 As Integer = arr.GetUpperBound(1) + 1

        ' Create a new array with the new size
        Dim newArr(newSize1 - 1, oldSize2 - 1) As Object

        ' Copy the old values into the new array
        For i As Integer = 0 To Math.Min(oldSize1, newSize1) - 1
            For j As Integer = 0 To oldSize2 - 1
                newArr(i, j) = arr(i, j)
            Next
        Next

        ' Update the original array reference
        arr = newArr
    End Sub

    Public Function OpenAndPrepareShapefile() As Boolean
        Try
            Select Case storageType
                Case enmStorageType.shapefile
                    SF = New MapWinGIS.Shapefile
                    SF.Open(path)
                    SF.BeginPointInShapefile()

                    'first walk through each field of our source dataset, check if it is one of the required ones and
                    'if so, assign the source's field index number to it so we know where to find the data
                    For i = 0 To SF.NumFields - 1
                        Dim FieldName As String = SF.Field(i).Name
                        If Fields.ContainsKey(FieldName.Trim.ToUpper) Then
                            Fields.Item(FieldName.Trim.ToUpper).sourceFieldIdx = i
                        End If
                    Next

                    Return True
                Case Else
                    Throw New Exception("Dataset must be of type shapefile.")
            End Select
            Throw New Exception("")
        Catch ex As Exception
            Setup.Log.AddError("Error in function OpenShapefileAndBeginPointInShapefile: " & ex.Message)
            Return False
        End Try
    End Function

    Public Function CloseShapefileAndEndPointInShapefile() As Boolean
        Try
            SF.EndPointInShapefile()
            SF.Close()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function


    Public Function readToDictionary() As Boolean
        Try
            Dim i As Integer, j As Integer
            Select Case storageType
                Case enmStorageType.geopackage

                    Features = New Dictionary(Of Integer, clsSpatialFeature) 'key = feature index

                    'Dim Tables As List(Of String) = GetGeoPackageTables(path)

                    ' Features = New Dictionary(Of Integer, clsSpatialFeature) 'key = feature index
                    Dim connectionString As String = $"Data Source={path};"
                    Using conn As New SQLite.SQLiteConnection(connectionString)
                        conn.Open()

                        ' Creating the SQL command to select all rows from the specified table
                        Dim cmd As New SQLiteCommand($"SELECT * FROM {tablename};", conn)

                        ' Execute the command and fill a DataTable with the query results
                        Using reader As SQLiteDataReader = cmd.ExecuteReader()
                            Dim dt As New DataTable
                            dt.Load(reader)

                            'find the geometry column
                            Dim geomColIdx As Integer = -1
                            For i = 0 To dt.Columns.Count - 1
                                If dt.Columns(i).ColumnName.Trim.ToLower = "geom" OrElse dt.Columns(i).ColumnName.Trim.ToLower = "geometry" Then
                                    geomColIdx = i
                                    Exit For
                                End If
                            Next

                            If geomColIdx >= 0 Then

                                'Transfer the data from our DataTable to our internal data structure
                                ReDim Values(Fields.Count - 1, dt.Rows.Count - 1)
                                For i = 0 To dt.Rows.Count - 1

                                    'write the geometry to the features dictionary
                                    Dim myFeature As New clsSpatialFeature(enmDataType.polygons)
                                    Dim geomBytes As Byte() = DirectCast(dt.Rows(i)(geomColIdx), Byte())
                                    myFeature.WKB = geomBytes
                                    Features.Add(Features.Count, myFeature)

                                    'write the attribute values to the values list
                                    For j = 0 To Fields.Count - 1
                                        Values(Fields.Values(j).fieldIdx, i) = dt.Rows(i)(j)
                                    Next
                                Next

                            End If

                        End Using

                        conn.Close()
                    End Using



                Case enmStorageType.shapefile
                    Features = New Dictionary(Of Integer, clsSpatialFeature) 'key = feature index

                    'read the shapefile
                    SF = New MapWinGIS.Shapefile
                    SF.Open(path)

                    'first walk through each field of our source dataset, check if it is one of the required ones and
                    'if so, assign the source's field index number to it so we know where to find the data
                    For i = 0 To SF.NumFields - 1
                        Dim FieldName As String = SF.Field(i).Name
                        If Fields.ContainsKey(FieldName.Trim.ToUpper) Then
                            Fields.Item(FieldName.Trim.ToUpper).sourceFieldIdx = i
                        End If
                    Next

                    'check if for each field the sourcefieldidx is assigned
                    For Each Field As clsSQLiteField In Fields.Values
                        If Field.sourceFieldIdx < 0 Then Throw New Exception("Required field is not yet associated with the source dataset: " & Field.FieldName)
                    Next

                    'then walk through each feature of the source dataset and add it
                    Setup.Generalfunctions.UpdateProgressBar(Setup.ProgressBar, Setup.ProgressLabel, "Reading features...", 0, 10, True)

                    For j = 0 To SF.NumShapes - 1
                        Setup.Generalfunctions.UpdateProgressBar(Setup.ProgressBar, Setup.ProgressLabel, "", j, SF.NumShapes)
                        Dim myFeature As New clsSpatialFeature(clsGeneralFunctions.enmDataType.polygons, SF.Shape(j).ExportToWKT)
                        Features.Add(j, myFeature)
                    Next

                    'then walk through each feature of the source dataset and add it
                    Setup.Generalfunctions.UpdateProgressBar(Setup.ProgressBar, Setup.ProgressLabel, "Reading feature values...", 0, 10, True)

                    'and finally populate the table with the data contents
                    ReDim Values(Fields.Count - 1, SF.NumShapes - 1)
                    For j = 0 To Features.Count - 1
                        Setup.Generalfunctions.UpdateProgressBar(Setup.ProgressBar, Setup.ProgressLabel, "", j, SF.NumShapes)
                        For i = 0 To Fields.Count - 1
                            Values(i, j) = SF.CellValue(Fields.Values(i).sourceFieldIdx, j)
                        Next
                    Next

                    SF.Close()

                Case enmStorageType.sqlite

                    'Features = New Dictionary(Of Integer, clsSpatialFeature) 'key = feature index                  'features not necessary since we have the featureidx and can look up the feature from the featuresdataset
                    Dim connectionString As String = String.Format("Data Source={0};Version=3;", path)
                    Setup.SQLiteCon = New SQLite.SQLiteConnection(connectionString)
                    Setup.SQLiteCon.Open()

                    Dim query As String = "SELECT " & Fields.Values(0).FieldName
                    For i = 1 To Fields.Count - 1
                        query &= ", " & Fields.Values(i).FieldName
                    Next
                    query &= " FROM " & tablename & ";"

                    'read all data from the SQLite table
                    Dim dt As New DataTable
                    Setup.Generalfunctions.SQLiteQuery(Setup.SQLiteCon, query, dt, True)

                    'transfer the data from our datatable to our internal data structure
                    ReDim Values(Fields.Count - 1, dt.Rows.Count - 1)
                    For i = 0 To dt.Rows.Count - 1
                        For j = 0 To Fields.Count - 1
                            Values(Fields.Values(j).fieldIdx, i) = dt.Rows(i)(j)
                        Next
                    Next

                    Setup.SQLiteCon.Close()
                Case Else
                    Throw New Exception("Error: storage type of dataset not yet supported for reading to internal dictionary: " & ID)
            End Select

            readingCompleted = True
            Return True

        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function GetGeoPackageTables(ByVal geoPackagePath As String) As List(Of String)
        Dim tables As New List(Of String)

        Try
            Dim connectionString As String = $"Data Source={geoPackagePath};"
            Using conn As New SQLiteConnection(connectionString)
                conn.Open()

                ' Query to get all tables in the GeoPackage
                Using cmd As New SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table';", conn)
                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            tables.Add(reader("name").ToString())
                        End While
                    End Using
                End Using

                conn.Close()
            End Using
        Catch ex As Exception
            ' Handle exceptions like file not found or no permission
            Console.WriteLine($"An error occurred: {ex.Message}")
        End Try

        Return tables
    End Function

    Public Function getFeatureIndexList() As List(Of Integer)
        Try
            Dim IdxList As New List(Of Integer)
            Dim IdxField As clsSQLiteField = getFeatureIndexField()
            If IdxField Is Nothing Then Throw New Exception($"no feature index nmbers field found in dataset {ID}")
            For i = 0 To Features.Count - 1
                IdxList.Add(Values(IdxField.fieldIdx, i))
            Next
            Return IdxList
        Catch ex As Exception
            Me.Setup.Log.AddError("Error in function getFeatureIndexList of class clsDataset: " & ex.Message)
        End Try
    End Function

    Public Function getFeatureIndexField() As clsSQLiteField
        For Each myField As clsSQLiteField In Fields.Values
            If myField.FieldType = enmFieldType.featureidx Then Return myField
        Next
        Return Nothing
    End Function

    Public Function getFieldByType(myType As enmFieldType) As clsSQLiteField
        For Each myField As clsSQLiteField In Fields.Values
            If myField.FieldType = myType Then Return myField
        Next
        Return Nothing
    End Function

End Class

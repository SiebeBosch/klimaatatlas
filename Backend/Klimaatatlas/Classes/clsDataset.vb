Imports Klimaatatlas.clsGeneralFunctions
Imports MapWinGIS
Imports Newtonsoft.Json.Linq
Imports System.IO
Public Class clsDataset

    Private Setup As clsKlimaatatlas
    Friend Comment As String
    Friend ID As String
    Friend dataType As enmDataType
    Friend storageType As enmStorageType
    Friend path As String
    Friend Fields As Dictionary(Of String, clsSQLiteField)            'key = the name of the field in our source dataset, to uppercase
    Friend Features As Dictionary(Of Integer, clsSpatialFeature)      'key = index number of our feature
    Friend Values As Object(,)                                        'first dimension = field idx, second dimension = feature idx

    Public Sub New(ByRef myKlimaatatlas As clsKlimaatatlas)
        Setup = myKlimaatatlas
        Fields = New Dictionary(Of String, clsSQLiteField)
        Features = New Dictionary(Of Integer, clsSpatialFeature)
    End Sub

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


    Public Function readToDictionary() As Boolean
        Try
            Dim i As Integer, j As Integer
            Select Case storageType
                Case enmStorageType.shapefile
                    Features = New Dictionary(Of Integer, clsSpatialFeature) 'key = feature index

                    'read the shapefile
                    Dim sf As New MapWinGIS.Shapefile
                    sf.Open(path)

                    'first walk through each field of our source dataset, check if it is one of the required ones and
                    'if so, assign the source's field index number to it so we know where to find the data
                    For i = 0 To sf.NumFields - 1
                        Dim FieldName As String = sf.Field(i).Name
                        If Fields.ContainsKey(FieldName.Trim.ToUpper) Then
                            Fields.Item(FieldName.Trim.ToUpper).sourceFieldIdx = i
                        End If
                    Next

                    'check if for each field the sourcefieldidx is assigned
                    For Each Field As clsSQLiteField In Fields.Values
                        If Field.sourceFieldIdx < 0 Then Throw New Exception("Required field is not yet associated with the source dataset: " & Field.FieldName)
                    Next

                    'then walk through each feature of the source dataset and add it
                    Setup.Generalfunctions.UpdateProgressBar(Setup.myProgressBar, Setup.myProgressLabel, "Reading features...", 0, 10, True)

                    For j = 0 To sf.NumShapes - 1
                        Setup.Generalfunctions.UpdateProgressBar(Setup.myProgressBar, Setup.myProgressLabel, "", j, sf.NumShapes)
                        Dim myFeature As New clsSpatialFeature(clsGeneralFunctions.enmDataType.polygons, sf.Shape(j).ExportToWKT)
                        Features.Add(j, myFeature)
                    Next

                    'then walk through each feature of the source dataset and add it
                    Setup.Generalfunctions.UpdateProgressBar(Setup.myProgressBar, Setup.myProgressLabel, "Reading feature values...", 0, 10, True)

                    'and finally populate the table with the data contents
                    ReDim Values(Fields.Count - 1, sf.NumShapes - 1)
                    For j = 0 To Features.Count - 1
                        Setup.Generalfunctions.UpdateProgressBar(Setup.myProgressBar, Setup.myProgressLabel, "", j, sf.NumShapes)
                        For i = 0 To Fields.Count - 1
                            Values(i, j) = sf.CellValue(Fields.Values(i).sourceFieldIdx, j)
                        Next
                    Next

                    sf.Close()

                Case Else
                    Throw New Exception("Error: storage type of dataset not yet supported for reading to internal dictionary: " & ID)
            End Select
        Catch ex As Exception

        End Try
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

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

End Class

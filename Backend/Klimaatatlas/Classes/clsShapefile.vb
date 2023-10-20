Imports System.Drawing
Imports System.IO
Imports MapWinGIS

Public Class clsShapeFile

    Implements MapWinGIS.ICallback

    Public Path As String
    Public sf As New MapWinGIS.Shapefile
    Public Fields As New Dictionary(Of String, String)

    Private Setup As clsKlimaatatlas

    Public Sub New(ByRef mySetup As clsKlimaatatlas, Optional ByVal ImplementCallback As Boolean = False)

        Setup = mySetup
        'initalize the dictionary of fields with a fictional field named "NONE" and field index -1 so that the calling function can retrieve something it can handle 
        If ImplementCallback Then sf.GlobalCallback = Me

    End Sub

    Public Function CreateNew(ByVal mypath As String) As Boolean
        Path = mypath

        If System.IO.File.Exists(mypath) Then
            Me.Setup.GeneralFunctions.DeleteShapeFile(mypath)
        End If

        If Not sf.CreateNew(Path, MapWinGIS.ShpfileType.SHP_POLYLINE) Then Return False
        Return True
    End Function


    Public Function CreateField(ByVal FieldName As String, FieldType As MapWinGIS.FieldType, Precision As Integer, Length As Integer, ByRef FieldIdx As Integer) As Boolean
        Try
            FieldIdx = sf.EditAddField(FieldName, FieldType, Precision, Length)
            If FieldIdx >= 0 Then
                Return True
            Else
                Return False
            End If
            Return True
        Catch ex As Exception
            Me.Setup.Log.AddError("Error in function CreateField of class clsShapefile: " & ex.Message)
            Return False
        End Try
    End Function

    Public Function GetCreateField(ByVal FieldName As String, FIeldType As MapWinGIS.FieldType, Precision As Integer, Length As Integer, ByRef FieldIdx As Integer) As Boolean
        Try
            For i = 0 To sf.NumFields - 1
                If sf.Field(i).Name.Trim.ToUpper = FieldName Then
                    FieldIdx = i
                    Return True
                End If
            Next

            'if we end up here, a new field must be created
            Return CreateField(FieldName, FIeldType, Precision, Length, FieldIdx)

            Return True
        Catch ex As Exception
            Me.Setup.Log.AddError("Error in function GetCreateField of class clsShapefile: " & ex.Message)
            Return False
        End Try

    End Function

    Public Function WriteToGeoJSONForWeb(ByRef jsWriter As StreamWriter, ScenarioName As String, Optional ByVal nTimeStamps As Integer = 0) As Boolean
        Try
            Dim ShapeNum As Integer

            jsWriter.WriteLine("   {")
            jsWriter.WriteLine("        ""type"": ""FeatureCollection"",")
            jsWriter.WriteLine("        ""name"": """ & ScenarioName & """,")
            jsWriter.WriteLine("        ""timesteps"": " & nTimeStamps & ",")
            jsWriter.WriteLine("        ""crs"": { ""type"": ""name"", ""properties"": { ""name"": ""urn:ogc:def:crs:OGC:1.3:CRS84""} },")

            jsWriter.WriteLine("        ""extent"": {")
            jsWriter.WriteLine("            ""minLat"":" & sf.Extents.yMin & ",")
            jsWriter.WriteLine("            ""minLng"":" & sf.Extents.xMin & ",")
            jsWriter.WriteLine("            ""maxLat"":" & sf.Extents.yMax & ",")
            jsWriter.WriteLine("            ""maxLng"":" & sf.Extents.xMax)
            jsWriter.WriteLine("        },")

            jsWriter.WriteLine("        ""features"": [") 'open the array containing the features

            For i = 0 To sf.NumShapes - 1
                Dim myShape As MapWinGIS.Shape = sf.Shape(i)
                ShapeNum = i

                If myShape.NumParts > 1 Then
                    jsWriter.Write("            { ""type"": ""Feature"", ""geometry"": { ""type"": ""MultiPolygon"", ""coordinates"": [[")
                Else
                    jsWriter.Write("            { ""type"": ""Feature"", ""geometry"": { ""type"": ""Polygon"", ""coordinates"": [") ' Removed one square bracket here
                End If

                For p = 0 To myShape.NumParts - 1
                    If p > 0 Then jsWriter.Write(", ")

                    jsWriter.Write("[")

                    Dim startIdx As Integer = myShape.Part(p)
                    Dim endIdx As Integer
                    If p < myShape.NumParts - 1 Then
                        endIdx = myShape.Part(p + 1) - 1
                    Else
                        endIdx = myShape.numPoints - 1
                    End If

                    For j = startIdx To endIdx
                        jsWriter.Write("[" & myShape.Point(j).x & "," & myShape.Point(j).y & "]")
                        If j < endIdx Then jsWriter.Write(", ")
                    Next

                    jsWriter.Write("]")
                Next

                If myShape.NumParts > 1 Then jsWriter.Write("]]") Else jsWriter.Write("]") ' Removed one square bracket here

                'also write the cell index number + its centerpoint
                jsWriter.Write("}, ""properties"": { ""i"": " & ShapeNum & ",""idx"": " & i & ",""lat"": " & myShape.Center.y & ", ""lon"": " & myShape.Center.x & "}}")

                If i < sf.NumShapes - 1 Then jsWriter.Write(",") 'more features to be added
                jsWriter.WriteLine()
            Next

            jsWriter.WriteLine("            ]")
            jsWriter.WriteLine("        }")

            Return True
        Catch ex As Exception
            Me.Setup.Log.AddError("Error in function WriteToGeoJSONForWeb of class clsShapefile: " & ex.Message)
            Return False
        End Try
    End Function



    Public Function ExportAsTilemapByTemplate(TemplateDir As String, ExportDir As String, ShapeIdxFieldIdx As Integer, ValuesFieldIdx As Integer, timestep As Integer, ntimesteps As Integer, Parameter As String, Gradient As clsColorGradient) As Boolean
        Try
            'this function uses a directory with template files (GeoTIFF containing shape index numbers!) in order to 
            'create a new tilemap (PNG) for values from a given values field in the shapefile
            Dim Zoomlevel As Integer
            Dim myTile As New MapWinGIS.Grid
            Dim Write As Boolean = False
            Dim DirPath As String
            Dim PngPath As String
            Dim iCol As Integer

            sf.Open(Path)

            'make sure our export dir exists
            If Not Directory.Exists(ExportDir) Then Directory.CreateDirectory(ExportDir)

            'the great thing about using a template is that we have no need for coordinates any longer. 
            'simply walk through the directory structure of our template tilemap
            Dim ZoomDirs As String() = Directory.GetDirectories(TemplateDir, "*", SearchOption.TopDirectoryOnly)
            For Each ZoomDir As String In ZoomDirs

                'figure out our current zoomlevel
                Dim Sections As String() = Strings.Split(ZoomDir, "\")
                Zoomlevel = Convert.ToInt16(Sections(UBound(Sections)))
                Me.Setup.Generalfunctions.UpdateProgressBar("Writing " & Parameter.ToString & " tiles for timestep " & timestep & " of " & ntimesteps & " at zoomlevel " & Zoomlevel & "...", 0, 10, True)

                'make sure our export dir exists
                DirPath = Strings.Replace(ZoomDir, TemplateDir, ExportDir, ,, CompareMethod.Text)
                If Not Directory.Exists(DirPath) Then Directory.CreateDirectory(DirPath)

                Dim ColDirs As String() = Directory.GetDirectories(ZoomDir, "*", SearchOption.TopDirectoryOnly)

                iCol = 0
                Me.Setup.Generalfunctions.UpdateProgressBar("", 0, 10, True)
                For Each ColDir As String In ColDirs
                    iCol += 1
                    Me.Setup.Generalfunctions.UpdateProgressBar("", iCol, ColDirs.Count, True)

                    'make sure our export dir exists
                    DirPath = Strings.Replace(ColDir, TemplateDir, ExportDir, ,, CompareMethod.Text)
                    If Not Directory.Exists(DirPath) Then Directory.CreateDirectory(DirPath)

                    Dim TiffFiles As String() = Directory.GetFiles(ColDir, "*.tif", SearchOption.TopDirectoryOnly)
                    For Each TiffFile In TiffFiles

                        'create a new bitmap for our tile and read the corresponding GeoTIFF into memory
                        Dim bmp As New Bitmap(256, 256)
                        myTile.Open(TiffFile, MapWinGIS.GridDataType.LongDataType, True)
                        Write = False

                        'walk throug all rows and columns and convert our tile's values to RGBA colors
                        For r = 0 To 255
                            For c = 0 To 255
                                If myTile.Value(c, r) <> -999 Then
                                    If Not IsDBNull(sf.CellValue(ValuesFieldIdx, myTile.Value(c, r))) Then
                                        bmp.SetPixel(c, r, Gradient.InterpolateRGBAColorForValue(sf.CellValue(ValuesFieldIdx, myTile.Value(c, r))))
                                        Write = True
                                    End If
                                End If
                            Next
                        Next

                        myTile.Close()
                        PngPath = Strings.Replace(Strings.Replace(TiffFile, TemplateDir, ExportDir,,, CompareMethod.Text), ".tif", ".png", CompareMethod.Text)
                        If Write Then bmp.Save(PngPath)

                    Next
                Next
            Next

            sf.Close()
            Return True
        Catch ex As Exception
            Me.Setup.Log.AddError("Error in function ExportAsTilemap of class clsraster: " & ex.Message)
            Return False
        End Try
    End Function

    Public Function WriteTile(TilePath As String, TifPath As String, ValueFieldName As String, ByRef ColorGradient As clsColorGradient, tilelatll As Double, tilelonll As Double, tilelatur As Double, tilelonur As Double) As Boolean
        'this function writes a standard tile according to the Leaflet conventions: 256 x 256 pixels
        'first it rasterizes the specified section of our shapefile to a GeoTIFF containing the required polygon index numbers
        'it then writes a bitmap (png) with the same extent and resolution, containing the colors corresponding to the values from our shapefile
        Try
            Dim xmin As Double, ymin As Double, xmax As Double, ymax As Double
            Dim Utils As New MapWinGIS.Utils
            Dim options As String
            Dim bmp As New Bitmap(256, 256)

            Dim Write As Boolean = False

            'write our tile from the shapefile's value field & the color gradient
            Me.Setup.GeneralFunctions.WGS842RD(tilelatll, tilelonll, xmin, ymin)
            Me.Setup.GeneralFunctions.WGS842RD(tilelatur, tilelonur, xmax, ymax)

            'write our shape index numbers to a GeoTIFF
            options = "-of GTiff -ot Int16 -a_nodata -999 -te " & xmin & " " & ymin & " " & xmax & " " & ymax & " -ts 256 256 -a " & ValueFieldName               'set the rasterize options
            Utils.GDALRasterize(Path, TifPath, options, Utils.GlobalCallback)   'rasterize part of our shapefile with the given resolution and extent

            'now read our GeoTIFF into memory
            Dim myTile As New MapWinGIS.Grid
            myTile.Open(TifPath, MapWinGIS.GridDataType.LongDataType, True)

            'walk throug all rows and columns and convert our tile's values to RGBA colors
            For r = 0 To 255
                For c = 0 To 255
                    If myTile.Value(c, r) <> -999 Then
                        bmp.SetPixel(c, r, ColorGradient.InterpolateRGBAColorForValue(myTile.Value(c, r)))
                        Write = True
                    End If
                Next
            Next

            myTile.Close()

            'and finally remove the TIF since we don't need it any more
            System.IO.File.Delete(TifPath)

            If Write Then bmp.Save(TilePath)
            Return True

        Catch ex As Exception
            Me.Setup.Log.AddError("Error in function WriteTile of class clsShapeFile: " & ex.Message)
            Return False
        End Try

    End Function


    Public Sub Progress(KeyOfSender As String, Percent As Integer, Message As String) Implements ICallback.Progress
        Me.Setup.GeneralFunctions.UpdateProgressBar(Message, Percent, 100, True)
    End Sub

    Public Sub [Error](KeyOfSender As String, ErrorMsg As String) Implements ICallback.Error
        Select Case ErrorMsg
            Case Is = "Table: Index Out of Bounds"
                'door negatieve veldindex in shapefile. Dit gebruiken we actief als feature, dus niet als foutmelding afhandelen.
            Case Else
                Me.Setup.Log.AddError("Error returned from MapWinGIS Callback function: " & ErrorMsg)
                Me.Setup.Log.AddError(ErrorMsg)
        End Select
    End Sub


    Public Function DeleteFieldByName(FieldName As String) As Boolean
        Try
            'remove all unnecessary fields
            Dim FieldIdx As Integer = sf.FieldIndexByName(FieldName)
            If FieldIdx >= 0 Then sf.EditDeleteField(FieldIdx)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function getShapeValueByCoord(ByVal X As Double, ByVal Y As Double, ByVal FieldIdx As Integer) As Object
        'let op: voordat deze functie aangeroepen kan worden, moet sf.BeginPointInShapefile zijn geactiveerd. Na afloop sf.EndPointInShapefile!!!!
        'deze actie kost echter elke keer 2 ms, dus doen we het overkoepelend.
        Dim ShapeIdx As Integer = GetShapeIdxByCoord(X, Y)
        Return sf.CellValue(FieldIdx, ShapeIdx)
    End Function

    Public Function GetShapeIdxByValue(FieldIdx As Integer, FieldValue As String) As Integer
        Try
            For i = 0 To sf.NumShapes - 1
                If sf.CellValue(FieldIdx, i).ToString.Trim.ToUpper = FieldValue.Trim.ToUpper Then
                    Return i
                End If
            Next
            Return -1
        Catch ex As Exception
            Me.Setup.Log.AddError("Error in function GetShapeIdxByValue of class clsShapeFile.")
            Return -1
        End Try
    End Function

    Public Function GetShapeIdxByCoord(ByVal X As Double, ByVal Y As Double) As Integer
        Dim Idx As Integer
        Try
            'sf.BeginPointInShapefile() 'note: this statement has to be in the beginning but it is not practical to do it every time
            Idx = sf.PointInShapefile(X, Y)
            'sf.EndPointInShapefile()
            If Idx >= 0 Then
                Return Idx
            Else
                Return -1
            End If
        Catch ex As Exception
            Me.Setup.Log.AddError("Error in function GetRecordIdxByCoord of class clsGeoDatasource: " & ex.Message)
            Return -1
        Finally
        End Try
    End Function
    Public Function ValuesUnique(FieldIdx As Integer) As Boolean
        Try
            Dim ValuesList As New List(Of String)
            For i = 0 To sf.NumShapes - 1
                If ValuesList.Contains(sf.CellValue(FieldIdx, i)) Then
                    Return False
                Else
                    ValuesList.Add(sf.CellValue(FieldIdx, i))
                End If
            Next
            Return True
        Catch ex As Exception
            Me.Setup.Log.AddError("Error in function ValuesUnique of class clsShapefile: " & ex.Message)
            Return False
        End Try
    End Function



    Public Function GetAddField(Name As String, Type As MapWinGIS.FieldType, Precision As Integer, Width As Integer) As Integer
        'gets the shapefield index for a given fieldname. If not present it will create one
        If sf.FieldIndexByName(Name) < 0 Then
            'does not yet exist so create
            Return sf.EditAddField(Name, Type, Precision, Width)
        Else
            Return sf.FieldIndexByName(Name)
        End If
    End Function


    Friend Function getUnderlyingShapeIdx(ByVal X As Double, ByVal Y As Double, ByRef ShapeIdx As Long) As Boolean
        'geeft voor een gegeven XY-coordinaat het indexnummer van de onderliggende shape terug
        Dim myUtils As New MapWinGIS.Utils
        Dim myPoint As New MapWinGIS.Point
        Try
            myPoint.x = X
            myPoint.y = Y
            Dim i As Long
            For i = 0 To sf.NumShapes - 1
                If sf.Shape(i).Extents.xMin <= X AndAlso sf.Shape(i).Extents.xMax >= X AndAlso sf.Shape(i).Extents.yMin <= Y AndAlso sf.Shape(i).Extents.yMax >= Y Then
                    If myUtils.PointInPolygon(sf.Shape(i), myPoint) Then
                        ShapeIdx = i
                        Return True
                    End If
                End If
            Next
        Catch ex As Exception
            Return False
        Finally
            myUtils = Nothing
            myPoint = Nothing
        End Try
    End Function


    Public Function GetShapeIdxByCoord(ByVal X As Double, ByVal Y As Double, Optional CloseWhenDone As Boolean = False, Optional ByVal OpenFile As Boolean = True, Optional ByVal SetBeginPointInShapefile As Boolean = True) As Long
        'siebe: v1.71 we removed the methods 'beginpointinshapefile and endpointinshapefile since it consumes too much time to execute for each object separately
        'instead you can now set this when opening the shapefile itself. See the Open and Close functions
        Dim Idx As Long
        Try
            'LET OP: als deze functie vaak wordt aangeroepen is het verstandig om de file al geopend te hebben én om daarbij de optie BeginPointInShapefile al te hebben geactiveerd
            If OpenFile Then
                If Not sf.Open(SetBeginPointInShapefile) Then Throw New Exception("Could Not open shapefile " & sf.Filename)
            End If
            Idx = sf.PointInShapefile(X, Y)
            If Idx >= 0 Then
                Return Idx
            Else
                Me.Setup.Log.AddWarning("No shape found underlying coordinate " & X & "," & Y & ".")
                Return -1
            End If
            If CloseWhenDone Then Close()
        Catch ex As Exception
            Me.Setup.Log.AddError(ex.Message)
            Return -1
        Finally
        End Try
    End Function

    Public Function Open() As Boolean
        'v1.890: more detailed error handling when reading shapefiles
        Try
            If sf.Open(Path) Then
                Return True
            Else
                Throw New Exception("Error reading shapefile. Error code: " & sf.ErrorMsg(sf.LastErrorCode))
            End If
        Catch ex As Exception
            Me.Setup.Log.AddError(ex.Message)
            Me.Setup.Log.AddError("Error reading shapefile " & Path)
            Return False
        End Try
    End Function

    Public Sub Close()
        sf.Close()
    End Sub

    Public Function GetFieldIdx(ByVal Name As String) As Integer
        Dim i As Long
        For i = 0 To sf.NumFields - 1
            If sf.Field(i).Name.Trim.ToUpper = Name.Trim.ToUpper Then Return i
        Next
        Return -1
    End Function

    Public Function getUniqueValuesFromField(ByVal fieldName As String, ByRef Values As List(Of String)) As Boolean
        'this function populates a list with all unique values present in a given field of the underlying shapefile.
        Try
            Dim FieldIdx As Long = GetFieldIdx(fieldName), i As Long
            If FieldIdx < 0 Then Throw New Exception("Error: fieldname " & fieldName & " does not occur in shapefile " & Path)

            For i = 0 To sf.NumShapes - 1
                If Not Values.Contains(sf.CellValue(FieldIdx, i)) Then Values.Add(sf.CellValue(FieldIdx, i))
            Next

            Return True
        Catch ex As Exception
            Me.Setup.Log.AddError(ex.Message)
            Return False
        End Try
    End Function

End Class


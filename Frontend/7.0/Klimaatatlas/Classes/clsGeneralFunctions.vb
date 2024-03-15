Imports System.Data.Entity.ModelConfiguration.Conventions
Imports System.Data.SQLite
Imports System.IO
Imports System.Windows

Public Class clsGeneralFunctions

    Private Setup As clsKlimaatatlas
    Dim Log As clsLog


    Public Sub New(ByRef mySetup As clsKlimaatatlas)
        Setup = mySetup
        Log = New clsLog()
    End Sub

    Public Enum enm2DParameter
        depth = 0
        waterlevel = 1
        velocity = 2
        u_velocity = 3
        v_velocity = 4
        maxvelocity = 5
        max_uvelocity = 6
        max_vvelocity = 7
        maxdepth = 8
        maxwaterlevel = 9
        t_flood = 10     'time to inundation
        cellidx = 11     'the index number of a 2D cell or face
        t_max = 12
        t_20cm = 13
        t_50cm = 14
    End Enum


    Public Enum enmSQLiteDataType
        SQLITETEXT  'note: dates can be implemented as text in YYYY-MM-DD HH:MM:SS.SSSS format
        SQLITEINT
        SQLITEREAL
        SQLITENULL
        SQLITEBLOB
    End Enum


    Public Enum enmFlowChartNodeEdge
        top = 0
        right = 1
        bottom = 2
        left = 3
    End Enum

    Public Enum enmRatingMethod
        constant = 1
        classification = 2
        lookup_table = 3
    End Enum

    Public Enum enmJoinMethod
        none = 0                                'no join method required in case our dataset is the same as the feature dataset
        match_featureidx = 1                    'both datasets have a corresponding featureidx
        feature_centerpoint_in_polygon = 2      'the centerpoints from the feature dataset are used to sample the dataset via the point-in-polyton method
    End Enum

    Public Enum enmTransformationFunction
        bod2kleizandveen = 0                    'refers to the function bod2kleizandveen which translates bodemkaartNL values to klei/zand/veen classes

    End Enum
    Public Enum enmFieldType
        featureidx = 0
        id = 1
        origin = 2
        scenario = 3
        parameter_name = 4
        percentile = 5
        datavalue = 6
        depth = 7
        deadend = 8
        comment = 9
        datetime = 10
        watertype = 11
        soiltype = 12
        verdict = 13
        width = 14
        temperature = 15
    End Enum



    Public Enum enmStorageType
        geopackage = 0
        shapefile = 1
        sqlite = 2
        excel = 3
    End Enum

    Public Enum enmDataType
        points = 1
        polylines = 2
        polygons = 3
        percentiles = 4
        timeseries = 5
    End Enum


    Public Sub DeleteShapeFile(ByVal ShpPath As String)
        Dim myPath As String = ShpPath
        If System.IO.File.Exists(myPath) Then System.IO.File.Delete(myPath)
        myPath = Replace(ShpPath, ".shp", ".shx", , , CompareMethod.Text)
        If System.IO.File.Exists(myPath) Then System.IO.File.Delete(myPath)
        myPath = Replace(ShpPath, ".shp", ".dbf", , , CompareMethod.Text)
        If System.IO.File.Exists(myPath) Then System.IO.File.Delete(myPath)
        myPath = Replace(ShpPath, ".shp", ".prj", , , CompareMethod.Text)
        If System.IO.File.Exists(myPath) Then System.IO.File.Delete(myPath)
        myPath = Replace(ShpPath, ".shp", ".mwd", , , CompareMethod.Text)
        If System.IO.File.Exists(myPath) Then System.IO.File.Delete(myPath)
        myPath = Replace(ShpPath, ".shp", ".mwx", , , CompareMethod.Text)
        If System.IO.File.Exists(myPath) Then System.IO.File.Delete(myPath)
    End Sub


    Public Function CreateSqlConnection(ByVal path As String) As SQLiteConnection
        ' Replace single backslashes with double backslashes
        path = path.Replace("\", "\\")

        ' Create a connection string for SQLite
        Dim connectionString As String = $"Data Source={path};Version=3;"

        ' Create a new SQLiteConnection object
        Dim connection As New SQLiteConnection(connectionString)

        ' Open the connection
        connection.Open()

        ' Return the connection
        Return connection
    End Function


    Public Function Interpolate2RGBColors(fromColor As System.Drawing.Color, toColor As System.Drawing.Color, myMin As Double, myMax As Double, myVal As Double) As System.Drawing.Color
        'source refactored (sped up) by ChatGPT Plus
        Dim newColor As New System.Drawing.Color
        Dim R As Integer, G As Integer, B As Integer

        R = Interpolate(myMin, fromColor.R, myMax, toColor.R, myVal)
        G = Interpolate(myMin, fromColor.G, myMax, toColor.G, myVal)
        B = Interpolate(myMin, fromColor.B, myMax, toColor.B, myVal)

        newColor = System.Drawing.Color.FromArgb(255, R, G, B)
        Return newColor
    End Function


    Public Function Interpolate(ByVal X1 As Double, ByVal Y1 As Double, ByVal X2 As Double, ByVal Y2 As Double,
                                ByVal X3 As Double, Optional ByVal BlockInterpolate As Boolean = False, Optional ByVal AllowExtrapolation As Boolean = False) As Double

        If X3 < X1 And X3 < X2 Then 'is niet interpoleren maar extrapoleren
            If AllowExtrapolation Then
                Return Extrapolate(X1, Y1, X2, Y2, X3)
            Else
                Return Y1
            End If
        ElseIf X3 > X2 And X3 > X1 Then 'is niet interpoleren maar extrapoleren
            If AllowExtrapolation Then
                Return Extrapolate(X1, Y1, X2, Y2, X3)
            Else
                Return Y2
            End If
        ElseIf X1 = X2 Then
            Return Y1
        Else
            If BlockInterpolate = True Then
                Return Y1
            Else
                Return Y1 + (Y2 - Y1) / (X2 - X1) * (X3 - X1)
            End If
        End If
    End Function


    Public Function Interpolate2RGBAColors(fromColor As System.Drawing.Color, toColor As System.Drawing.Color, myMin As Double, myMax As Double, TransparentBelowLowest As Boolean, TransparentAboveHighest As Boolean, TransparentAtLowest As Boolean, TransparentAtHighest As Boolean, myVal As Double) As System.Drawing.Color
        'this is a version refactored (sped up) by ChatGPT Plus
        Dim newColor As New System.Drawing.Color
        Dim R As Integer, G As Integer, B As Integer, A As Integer 'A = opacity: 0 = fully transparent, 255 = fully opaque

        If (myVal < myMin AndAlso TransparentBelowLowest) OrElse (myVal > myMax AndAlso TransparentAboveHighest) OrElse (myVal = myMin AndAlso TransparentAtLowest) OrElse (myVal = myMax AndAlso TransparentAtHighest) Then
            A = 0
        Else
            A = 255
        End If

        R = Interpolate(myMin, fromColor.R, myMax, toColor.R, myVal)
        G = Interpolate(myMin, fromColor.G, myMax, toColor.G, myVal)
        B = Interpolate(myMin, fromColor.B, myMax, toColor.B, myVal)

        newColor = System.Drawing.Color.FromArgb(A, R, G, B)
        Return newColor
    End Function

    Friend Function Extrapolate(ByVal X1 As Double, ByVal Y1 As Double, ByVal X2 As Double, ByVal Y2 As Double, ByVal X3 As Double) As Double
        'extrapolates linearly

        Dim Rico As Double = 0
        If X3 > X2 Then
            Rico = (Y2 - Y1) / (X2 - X1)
            Extrapolate = Y2 + (X3 - X2) * Rico
        ElseIf X3 < X1 Then
            Rico = (Y2 - Y1) / (X2 - X1)
            Extrapolate = Y1 - (X1 - X3) * Rico
        Else
            Extrapolate = -999
        End If
    End Function


    Public Function SQLiteIndexExists(ByRef con As System.Data.SQLite.SQLiteConnection, TableName As String, IndexName As String) As Boolean
        Try
            Dim query As String = "SELECT name FROM sqlite_master WHERE type='index' AND tbl_name='" & TableName & "' AND name='" & IndexName & "';"
            Dim dt As New DataTable
            SQLiteQuery(con, query, dt, False)

            If dt.Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Log.AddError("Error checking if index " & IndexName & " exists in table " & TableName & ": " & ex.Message)
            Return False
        End Try
    End Function


    Public Function SQLiteColumnExists(ByRef con As SQLite.SQLiteConnection, TableName As String, ColumnName As String) As Boolean
        Try
            Dim TableSchema As Object, col As Object
            If Not con.State = ConnectionState.Open Then con.Open()
            TableSchema = con.GetSchema("COLUMNS")
            col = TableSchema.Select("TABLE_NAME='" & TableName & "' AND COLUMN_NAME='" & ColumnName & "'")
            If (col.Length > 0) Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function


    Public Function SQLiteCreateColumn(ByRef con As System.Data.SQLite.SQLiteConnection, TableName As String, ColumnName As String, DataType As enmSQLiteDataType, Optional ByVal COLINDEXNAME As String = "") As Boolean
        Try
            Dim TableSchema As Object, query As String
            Dim ColSchema As Object, col As Object
            Dim DataTypeStr As String = ""

            Select Case DataType
                Case Is = enmSQLiteDataType.SQLITETEXT
                    DataTypeStr = "TEXT"
                Case Is = enmSQLiteDataType.SQLITEREAL
                    DataTypeStr = "REAL"
                Case Is = enmSQLiteDataType.SQLITEINT
                    DataTypeStr = "INTEGER"
            End Select

            If Not con.State = ConnectionState.Open Then con.Open()
            TableSchema = con.GetSchema("TABLES")
            ColSchema = con.GetSchema("COLUMNS")

            'create the column if it does not exist
            col = ColSchema.Select("TABLE_NAME='" & TableName & "' AND COLUMN_NAME ='" & ColumnName & "'")
            If col.Length = 0 Then
                query = "ALTER TABLE " & TableName & " ADD COLUMN " & ColumnName & " " & DataTypeStr & ";"
                SQLiteNoQuery(con, query, False)

                'column was created. Now set it as indexed, if required
                If COLINDEXNAME <> "" Then
                    query = "CREATE INDEX " & COLINDEXNAME & " ON " & TableName & " (" & ColumnName & ");"
                    SQLiteNoQuery(con, query, False)
                End If
            End If


            Return True
        Catch ex As Exception
            Log.AddError("Error creating column " & ColumnName & " in database table " & TableName & ": " & ex.Message)
            Return False
        End Try

    End Function


    Public Sub CreateSQLiteIndex(ByRef con As System.Data.SQLite.SQLiteConnection, TableName As String, ColumnName As String, IndexName As String)
        Try
            Dim query As String = "CREATE INDEX " & IndexName & " ON " & TableName & " (" & ColumnName & ");"
            SQLiteNoQuery(con, query, False)
        Catch ex As Exception
            Log.AddError("Error creating index " & IndexName & " on column " & ColumnName & " in table " & TableName & ": " & ex.Message)
        End Try
    End Sub

    Public Function SQLiteQuery(ByRef con As System.Data.SQLite.SQLiteConnection, ByVal myQuery As String, ByRef myTable As System.Data.DataTable, Optional ByVal CloseAfterwards As Boolean = True, Optional ByVal RetryIfException As Boolean = True) As Boolean
        'queries a DB (SQLite) database and returns the results in a datatable
        Try
            Dim da As SQLite.SQLiteDataAdapter
            If Not con.State = ConnectionState.Open Then con.Open()

            da = New SQLite.SQLiteDataAdapter(myQuery, con.ConnectionString)
            da.Fill(myTable)

            Return True
        Catch ex As Exception
            'SIEBE 12-3-2019
            'in sommige gevallen heeft de connectie een lees/schrijfprobleem en wordt een uitzondering opgeroepen. Een oplossing blijkt te zijn om de verbinding even te sluiten en dan weer te openen
            If RetryIfException Then
                Log.AddWarning("Warning: instable database connection. Connection was closed and opened again to execute query: " & myQuery)
                con.Close()
                SQLiteQuery(con, myQuery, myTable, CloseAfterwards, False)
                Return True
            Else
                Log.AddError("Error executing query " & myQuery & ": " & ex.Message)
                Return False
            End If

            Return False
        Finally
            If CloseAfterwards Then con.Close()
        End Try

    End Function


    Public Sub UpdateProgressBar(ByVal lblText As String, ByVal i As Long, ByVal n As Long, Optional ByVal ForceUpdate As Boolean = False)

        ' Check if the application is running in a console
        Dim isConsole = Console.OpenStandardInput(1) IsNot Stream.Null

        If Setup.ProgressBar IsNot Nothing AndAlso Setup.ProgressLabel IsNot Nothing Then
            If n = 0 Then n = 1
            i = System.Math.Min(System.Math.Max(i, 0), n)

            If lblText <> String.Empty Then
                Setup.ProgressLabel.Text = lblText
                ForceUpdate = True
            End If

            If ForceUpdate OrElse System.Math.Round(i / n * 100, 0) >= (Setup.ProgressBar.Value + 1) Then
                Setup.ProgressBar.Value = CInt(i / n * 100)
                Forms.Application.DoEvents()
            End If
        End If

    End Sub

    Public Function WGS842RD(ByVal Lat As Double, ByVal Lon As Double, Optional ByRef X As Double = 0, Optional ByRef y As Double = 0) As String
        'converteert WGS84-coordinaten (Lat/Long) naar RD
        'maakt gebruik van de routines van Ejo Schrama: schrama @geo.tudelft.nl
        Dim phiBes As Double
        Dim LambdaBes As Double
        Call WGS842BESSEL(Lat, Lon, phiBes, LambdaBes)
        Call BESSEL2RD(phiBes, LambdaBes, X, y)
        WGS842RD = X & "," & y

    End Function


    Public Sub WGS842BESSEL(ByVal PhiWGS As Double, ByVal LamWGS As Double, ByRef phi As Double, ByRef lambda As Double)
        Dim dphi As Double, dlam As Double, phicor As Double, lamcor As Double

        dphi = PhiWGS - 52
        dlam = LamWGS - 5
        phicor = (-96.862 - dphi * 11.714 - dlam * 0.125) * 0.00001
        lamcor = (dphi * 0.329 - 37.902 - dlam * 14.667) * 0.00001
        phi = PhiWGS - phicor
        lambda = LamWGS - lamcor

    End Sub

    Public Sub BESSEL2RD(ByVal phiBes As Double, ByVal lamBes As Double, ByRef X As Double, ByRef y As Double)

        'converteert Lat/Long van een Bessel-functie naar X en Y in RD
        'code is geheel gebaseerd op de routines van Ejo Schrama's software:
        'schrama@geo.tudelft.nl

        Dim x0 As Double
        Dim y0 As Double
        Dim k As Double
        Dim bigr As Double
        Dim m As Double
        Dim n As Double
        Dim lambda0 As Double
        Dim phi0 As Double
        Dim l0 As Double
        Dim b0 As Double
        Dim e As Double
        Dim a As Double

        Dim d_1 As Double, d_2 As Double, r As Double, sa As Double, ca As Double, cpsi As Double, spsi As Double
        Dim b As Double, dl As Double, w As Double, q As Double
        Dim dq As Double, pi As Double, phi As Double, lambda As Double, s2psihalf As Double, cpsihalf As Double, spsihalf As Double
        Dim tpsihalf As Double

        x0 = 155000
        y0 = 463000
        k = 0.9999079
        bigr = 6382644.571
        m = 0.003773953832
        n = 1.00047585668

        pi = System.Math.PI
        'pi = 3.14159265358979
        lambda0 = pi * 0.0299313271611111
        phi0 = pi * 0.289756447533333
        l0 = pi * 0.0299313271611111
        b0 = pi * 0.289561651383333

        e = 0.08169683122
        a = 6377397.155

        phi = phiBes / 180 * pi
        lambda = lamBes / 180 * pi

        q = System.Math.Log(System.Math.Tan(phi / 2 + pi / 4))
        dq = e / 2 * System.Math.Log((e * System.Math.Sin(phi) + 1) / (1 - e * System.Math.Sin(phi)))
        q = q - dq
        w = n * q + m
        b = System.Math.Atan(System.Math.Exp(1) ^ w) * 2 - pi / 2
        dl = n * (lambda - lambda0)
        d_1 = System.Math.Sin((b - b0) / 2)
        d_2 = System.Math.Sin(dl / 2)
        s2psihalf = d_1 * d_1 + d_2 * d_2 * System.Math.Cos(b) * System.Math.Cos(b0)
        cpsihalf = System.Math.Sqrt(1 - s2psihalf)
        spsihalf = System.Math.Sqrt(s2psihalf)
        tpsihalf = spsihalf / cpsihalf
        spsi = spsihalf * 2 * cpsihalf
        cpsi = 1 - s2psihalf * 2
        sa = System.Math.Sin(dl) * System.Math.Cos(b) / spsi
        ca = (System.Math.Sin(b) - System.Math.Sin(b0) * cpsi) / (System.Math.Cos(b0) * spsi)
        r = k * 2 * bigr * tpsihalf
        X = System.Math.Round(r * sa + x0, 0)
        y = System.Math.Round(r * ca + y0, 0)

    End Sub


    Public Function SQLiteNoQuery(ByRef con As System.Data.SQLite.SQLiteConnection, ByVal myQuery As String, Optional ByVal CloseAfterwards As Boolean = True, Optional ByVal RetryIfException As Boolean = True, Optional ByRef nAffected As Integer = 0) As Boolean
        'queries an MBD (Access) database and returns the results in a datatable
        Try
            Dim da As SQLite.SQLiteDataAdapter
            Dim myTable As New System.Data.DataTable
            If Not con.State = ConnectionState.Open Then con.Open()
            da = New SQLite.SQLiteDataAdapter(myQuery, con.ConnectionString)
            da.Fill(myTable)
            nAffected = myTable.Rows.Count
            Return True
        Catch ex As Exception
            'SIEBE 12-3-2019
            'in sommige gevallen heeft de connectie een lees/schrijfprobleem en wordt een uitzondering opgeroepen. Een oplossing blijkt te zijn om de verbinding even te sluiten en dan weer te openen
            'v1.76: added error handling for situations where a second attempt to execute query did not work
            If RetryIfException Then
                Log.AddWarning("Warning: instable database connection. Connection was closed and opened again to execute query: " & myQuery)
                con.Close()
                If Not SQLiteNoQuery(con, myQuery, CloseAfterwards, False) Then
                    Log.AddError("Error executing query " & myQuery & ": " & ex.Message)
                    Return False
                Else
                    Return True
                End If
            Else
                Log.AddError("Error executing query " & myQuery & ": " & ex.Message)
                Return False
            End If
            Return False
        Finally
            If CloseAfterwards Then con.Close()
        End Try

    End Function


    Public Function SQLiteTableExists(ByRef con As SQLite.SQLiteConnection, TableName As String) As Boolean
        Dim dt As New System.Data.DataTable
        Dim query As String = "SELECT * FROM sqlite_master WHERE type='table' AND name='" & TableName & "';"
        SQLiteQuery(con, query, dt, False)
        Return (dt.Rows.Count > 0)
    End Function



    Public Sub UpdateProgressBar(ByRef myProgressBar As System.Windows.Forms.ProgressBar, ByRef myLabel As System.Windows.Forms.Label, ByVal lblText As String, ByVal i As Long, ByVal n As Long, Optional ByVal ForceUpdate As Boolean = False)
        If n = 0 Then n = 1
        i = Math.Min(Math.Max(i, 0), n)

        If lblText <> String.Empty Then
            myLabel.Text = lblText
            ForceUpdate = True
        End If

        If ForceUpdate OrElse Math.Round(i / n * 100, 0) >= (myProgressBar.Value + 1) Then
            myProgressBar.Value = CInt(i / n * 100)
            Forms.Application.DoEvents()
        End If
    End Sub


    Function IsSingleQuoteCountEven(inputString As String) As Boolean
        Dim count As Integer = 0
        For Each c As Char In inputString
            If c = "'"c Then
                count += 1
            End If
        Next
        If count Mod 2 = 0 Then
            Return True
        Else
            Return False
        End If
    End Function



    Public Function ParseString(ByRef myString As String, Optional ByVal Delimiter As String = " ",
                                Optional ByVal QuoteHandlingFlag As Integer = 1, Optional ByVal ResultMustBeNumeric As Boolean = False) As String

        Dim quoteEven As Boolean
        Dim tmpString As String = "", tmpChar As String = ""

        If myString = "" Then Return ""

        myString = myString.Trim
        quoteEven = True

        'v2.5.5.1: check if the record contains an even number of single quotes.
        If Not IsSingleQuoteCountEven(myString) Then
            Return ""
        End If

        'Quotehandlingflag: default = 1
        '0 = items between quotes are NOT being treated as separate items (parsing also between quotes)
        '1 = items between single quotes are being treated as separate items (no parsing between single quotes)
        '2 = items between double quotes are being treated as separate items (no parsing between double quotes)

        Dim i As Integer
        For i = 1 To Len(myString)
            'snoep een karakter af van de string
            tmpChar = Strings.Left(myString, 1)

            If (tmpChar = "'" And QuoteHandlingFlag = 1) Or (tmpChar = Chr(34) And QuoteHandlingFlag = 2) Then
                If quoteEven = True Then
                    quoteEven = False
                    tmpString = tmpString & tmpChar
                    myString = Right(myString, myString.Length - 1)
                Else
                    quoteEven = True 'dit betekent dat we klaar zijn
                    tmpString = Right(tmpString, tmpString.Length - 1) 'laat bij het teruggeven meteen de quotes maar weg!
                    myString = Right(myString, myString.Length - 1)
                End If
            ElseIf tmpChar = Delimiter And quoteEven = True Then
                myString = Right(myString, myString.Length - 1)
                Return tmpString

                'If Not tmpString = "" Then
                '    myString = Right(myString, myString.Length - 1)
                '    'Return tmpString
                '    Exit For
                'Else
                '    myString = Right(myString, myString.Length - 1)
                'End If
            ElseIf myString <> "" Then
                myString = Right(myString, myString.Length - 1)
                tmpString = tmpString & tmpChar
            End If
        Next

        If ResultMustBeNumeric AndAlso Not IsNumeric(tmpString) Then
            myString = tmpString & Delimiter & myString
            Return 0
        Else
            Return tmpString
        End If


    End Function

    Function EvaluateLinearEquation(weights() As Double, values() As Double, ByRef yValue As Double) As Boolean
        ' Check if arrays are of the same length
        If weights.Length <> values.Length Then
            Return False
        End If

        yValue = 0
        For i As Integer = 0 To weights.Length - 1
            yValue += weights(i) * values(i)
        Next

        Return True
    End Function


    Function EvaluateSecondDegreePolynomeExpression(expression As String, xValue As Double, ByRef yValue As Double) As Boolean
        Try
            ' Remove spaces and convert to lower case for easier parsing
            expression = expression.ToLower().Replace(" ", "")

            ' Regular expression to extract a, b, and c coefficients, including decimals
            Dim pattern As String = "^([+-]?\d*\.?\d*)x\^2([+-]\d*\.?\d*)x([+-]\d*\.?\d+)$"
            Dim regex As New Text.RegularExpressions.Regex(pattern)
            Dim match As Text.RegularExpressions.Match = regex.Match(expression)

            If Not match.Success Then
                Throw New ArgumentException("Invalid expression format.")
            End If

            ' Extract a, b, and c values
            Dim a As Double = If(String.IsNullOrEmpty(match.Groups(1).Value), 1, Convert.ToDouble(match.Groups(1).Value))
            Dim b As Double = Convert.ToDouble(match.Groups(2).Value)
            Dim c As Double = Convert.ToDouble(match.Groups(3).Value)

            ' Evaluate the expression ax^2 + bx + c
            yValue = a * xValue * xValue + b * xValue + c
            Return True
        Catch ex As Exception
            Me.Setup.Log.AddError("Unable to evaluate mathematical expression for second degree polynome: " & expression & ". Error: " & ex.Message)
            Return False
        End Try
    End Function

    Function ByteArrayToHexString(ByVal bytes As Byte()) As String
        Dim hex As New System.Text.StringBuilder(bytes.Length * 2)
        For Each b As Byte In bytes
            hex.AppendFormat("{0:x2}", b)
        Next
        Return hex.ToString()
    End Function

End Class

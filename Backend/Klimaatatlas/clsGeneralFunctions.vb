Imports System.Data.Entity.ModelConfiguration.Conventions
Imports System.Data.SQLite
Imports System.Windows

Public Class clsGeneralFunctions

    Dim Log As clsLog

    Public Enum enmSQLiteDataType
        SQLITETEXT  'note: dates can be implemented as text in YYYY-MM-DD HH:MM:SS.SSSS format
        SQLITEINT
        SQLITEREAL
        SQLITENULL
        SQLITEBLOB
    End Enum

    Public Sub New()
        Log = New clsLog()
    End Sub

    Public Enum enmRatingMethod
        constant = 1
        classification = 2
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
    End Enum

    Public Enum enmStorageType
        shapefile = 0
        sqlite = 1
    End Enum

    Public Enum enmDataType
        points = 1
        polylines = 2
        polygons = 3
        percentiles = 4
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



End Class

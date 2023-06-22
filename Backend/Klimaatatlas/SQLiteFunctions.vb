Imports System.Data.SQLite
Imports System.Text
Imports Klimaatatlas.clsGeneralFunctions

Public Module SQLiteFunctions


    Public Class clsSQLiteField

        Public sourceFieldIdx As Integer = -1 'the original field index of the source dataset this field comes from (if applicable)
        Public FieldType As enmFieldType 'a specific fieldtype designed for klimaatatlas

        Public Enum enmSQLiteDataType
            SQLITETEXT
            SQLITEINT
            SQLITEREAL
            SQLITENULL
            SQLITEBLOB
        End Enum

        Public Property FieldName As String
        Public Property DataType As enmSQLiteDataType
        Public Property HasIndex As Boolean

        Public Shared Function SQLiteDataTypeToString(ByVal dataType As enmSQLiteDataType) As String
            Select Case dataType
                Case enmSQLiteDataType.SQLITETEXT
                    Return "TEXT"
                Case enmSQLiteDataType.SQLITEINT
                    Return "INTEGER"
                Case enmSQLiteDataType.SQLITEREAL
                    Return "REAL"
                Case enmSQLiteDataType.SQLITENULL
                    Return "NULL"
                Case enmSQLiteDataType.SQLITEBLOB
                    Return "BLOB"
                Case Else
                    Throw New ArgumentOutOfRangeException("Invalid data type.")
            End Select
        End Function

        Public Sub New(myFieldName As String, myFieldType As enmFieldType, myDataType As enmSQLiteDataType)
            FieldName = myFieldName
            FieldType = myFieldType
            DataType = myDataType
        End Sub

    End Class

    Public Sub CreateOrUpdateSQLiteTable(ByVal SQLiteCon As SQLiteConnection, ByVal TableName As String, ByVal Fields As Dictionary(Of String, clsSQLiteField))

        'make sure to open the databaes
        If Not SQLiteCon.State = ConnectionState.Open Then SQLiteCon.Open()

        Dim sql As New StringBuilder()
        Dim checkTableExistsSql As String = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{TableName}';"

        Using cmd As New SQLiteCommand(checkTableExistsSql, SQLiteCon)
            Dim result = cmd.ExecuteScalar()

            ' Create table if it does not exist
            If result Is Nothing Then
                sql.Append($"CREATE TABLE {TableName} (")

                Dim fieldsSql As New List(Of String)
                For Each field In Fields.Values
                    fieldsSql.Add($"{field.FieldName} {clsSQLiteField.SQLiteDataTypeToString(field.DataType)}")
                Next

                sql.Append(String.Join(", ", fieldsSql))
                sql.Append(");")

                Using createTableCmd As New SQLiteCommand(sql.ToString(), SQLiteCon)
                    createTableCmd.ExecuteNonQuery()
                End Using
            End If
        End Using

        ' Check and create fields if they do not exist
        For Each field In Fields.Values
            Dim checkFieldExistsSql As String = $"PRAGMA table_info({TableName});"

            Dim fieldExists As Boolean = False
            Using cmd As New SQLiteCommand(checkFieldExistsSql, SQLiteCon)
                Using reader As SQLiteDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        If reader("name").ToString().ToUpper() = field.FieldName.ToUpper() Then
                            fieldExists = True
                            Exit While
                        End If
                    End While
                End Using
            End Using

            If Not fieldExists Then
                Dim addFieldSql As String = $"ALTER TABLE {TableName} ADD COLUMN {field.FieldName} {clsSQLiteField.SQLiteDataTypeToString(field.DataType)};"
                Using addFieldCmd As New SQLiteCommand(addFieldSql, SQLiteCon)
                    addFieldCmd.ExecuteNonQuery()
                End Using
            End If

            ' Check and create indexes
            If field.HasIndex Then
                Dim indexName As String = $"idx_{TableName}_{field.FieldName}"
                Dim checkIndexExistsSql As String = $"SELECT name FROM sqlite_master WHERE type='index' AND name='{indexName}';"

                Using cmd As New SQLiteCommand(checkIndexExistsSql, SQLiteCon)
                    Dim result = cmd.ExecuteScalar()

                    If result Is Nothing Then
                        Dim createIndexSql As String = $"CREATE INDEX {indexName} ON {TableName}({field.FieldName});"

                        Using createIndexCmd As New SQLiteCommand(createIndexSql, SQLiteCon)
                            createIndexCmd.ExecuteNonQuery()
                        End Using
                    End If
                End Using
            End If
        Next

        'and close the database again
        SQLiteCon.Close()


    End Sub


End Module

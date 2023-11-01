Imports System.ComponentModel
Imports System.Data.SQLite
Imports System.Text.RegularExpressions
Imports System.Transactions
Imports Klimaatatlas.clsGeneralFunctions

Public Class clsRule
    Friend Name As String
    Friend Benchmarks As New Dictionary(Of String, clsBenchmark)
    Friend EquationComponents As New List(Of clsEquationComponent)
    Private Setup As clsKlimaatatlas

    Public Sub New(ByRef mySetup As clsKlimaatatlas)
        Setup = mySetup
    End Sub

    Public Function Execute() As Boolean
        Try

            For Each Scenario As clsScenario In Setup.Scenarios.Values

                Using transaction As SQLiteTransaction = Setup.GpkgCon.BeginTransaction()

                    Dim dt As New DataTable()
                    Dim columnsToUpdate As New List(Of String)    'keep track of the fields that are needed for the results of this rule

                    'establisch a list of fields needed for this rule
                    Dim Fields As List(Of String) = getFieldNamesForScenario(Scenario)

                    'Ensure necessary columns exist before calculations and add them to the list
                    EnsureColumnExists(Setup.GpkgTable, $"{Scenario.Name}_{Name}", "REAL")
                    Fields.Add($"{Scenario.Name}_{Name}")
                    columnsToUpdate.Add($"{Scenario.Name}_{Name}")

                    For Each Component As clsEquationComponent In EquationComponents
                        EnsureColumnExists(Setup.GpkgTable, $"{Scenario.Name}_{Component.ResultsFieldName}", "REAL")
                        Fields.Add($"{Scenario.Name}_{Component.ResultsFieldName}")
                        columnsToUpdate.Add($"{Scenario.Name}_{Component.ResultsFieldName}")
                    Next

                    ' Execute a query to retrieve all features and necessary fields
                    Using cmdSelect As New SQLiteCommand("SELECT fid, " & String.Join(", ", Fields) & " FROM " & Setup.GpkgTable & ";", Setup.GpkgCon)
                        Using adapter As New SQLiteDataAdapter(cmdSelect)
                            adapter.Fill(dt)
                        End Using
                    End Using

                    For Each row As DataRow In dt.Rows
                        Dim fid As Integer = Convert.ToInt32(row("fid"))
                        Dim totalWeight As Double = 0
                        Dim ResultSum As Double = 0

                        For Each Component As clsEquationComponent In EquationComponents
                            Dim FieldName As String = Component.Benchmark.FieldNamesPerScenario.Item(Scenario.Name.Trim.ToUpper)
                            Dim value As Object = row(FieldName)
                            totalWeight += Component.Weight
                            Component.Result = Component.Benchmark.getResult(value) * Component.Weight
                            ResultSum += Component.Result * Component.Weight
                        Next

                        For Each Component As clsEquationComponent In EquationComponents
                            Dim contribution As Double = Component.Result / ResultSum

                            ' Assuming the column name is built using Scenario.Name, Component.ResultsFieldName
                            Dim columnName As String = $"{Scenario.Name}_{Component.ResultsFieldName}"

                            ' Save the contribution in the DataRow
                            If dt.Columns.Contains(columnName) Then
                                row(columnName) = contribution
                            Else
                                Throw New Exception($"Column '{columnName}' does not exist in the DataTable.")
                            End If
                        Next

                        'save the result for this rule + scenario in the DataRow
                        Dim resultColumnName As String = $"{Scenario.Name}_{Name}"
                        If dt.Columns.Contains(resultColumnName) Then
                            row(resultColumnName) = ResultSum / totalWeight
                        Else
                            Throw New Exception($"Column '{resultColumnName}' does not exist in the DataTable.")
                        End If
                    Next


                    ' At the end of each scenario, write the results back to the database
                    For Each row As DataRow In dt.Rows
                        Using cmdUpdate As New SQLiteCommand(Setup.GpkgCon)
                            Dim updateSetClauses As New List(Of String)

                            For Each columnName In columnsToUpdate
                                If dt.Columns.Contains(columnName) Then
                                    updateSetClauses.Add($"{columnName} = @{columnName}")
                                    cmdUpdate.Parameters.AddWithValue($"@{columnName}", row(columnName))
                                End If
                            Next

                            cmdUpdate.CommandText = $"UPDATE {Setup.GpkgTable} SET {String.Join(", ", updateSetClauses)} WHERE fid = @fid;"
                            cmdUpdate.ExecuteNonQuery()
                        End Using
                    Next

                    transaction.Commit() ' Committing transaction after all rows are updated
                End Using
            Next

            Return True
        Catch ex As Exception
            Me.Setup.Log.AddError("Error executing rule " & Name & ": " & ex.Message)
            Return False
        End Try
    End Function

    ' Removed Parse function since it's not needed for the JSON approach

    Public Sub EnsureColumnExists(tableName As String, columnName As String, dataType As String)
        Dim columnExists As Boolean = False

        Using cmd As New SQLiteCommand($"PRAGMA table_info({tableName});", Setup.GpkgCon)
            Using reader As SQLiteDataReader = cmd.ExecuteReader()
                While reader.Read()
                    If String.Equals(reader("name").ToString(), columnName, StringComparison.OrdinalIgnoreCase) Then
                        columnExists = True
                        Exit While
                    End If
                End While
            End Using
        End Using

        If Not columnExists Then
            Using cmd As New SQLiteCommand($"ALTER TABLE {tableName} ADD COLUMN {columnName} {dataType};", Setup.GpkgCon)
                cmd.ExecuteNonQuery()
            End Using
        End If
    End Sub

    Public Function getFieldNamesForScenario(Scenario As clsScenario) As List(Of String)
        'returns the field names that are needed for this rule and scenario
        Dim Fields As New List(Of String)
        For Each Benchmark As clsBenchmark In Benchmarks.Values
            Fields.Add(Benchmark.FieldNamesPerScenario.Item(Scenario.Name.Trim.ToUpper))
        Next
        Return Fields
    End Function

End Class


Public Class clsEquationComponent
    Friend Benchmark As clsBenchmark                'de maatlat
    Friend Weight As Double                         'weging van deze maatlat
    Friend ResultsFieldName As String
    Friend ResultsField As clsSQLiteField
    Friend Result As Double
    Private Rule As clsRule
    Private Setup As clsKlimaatatlas

    Public Sub New(ByRef mySetup As clsKlimaatatlas, ByRef myRule As clsRule, myBenchmarkName As String, myWeight As Double, myResultsFieldName As String)
        Setup = mySetup
        Rule = myRule
        Benchmark = Rule.Benchmarks.Item(myBenchmarkName.Trim.ToUpper)
        Weight = myWeight
        ResultsFieldName = myResultsFieldName
    End Sub

    Public Function calculateResult(value As Object) As Boolean
        'this function calculates the result of this component for a given value
        'then applies the weight to it and writes it to the Result property
        Result = Benchmark.getResult(value) * Weight
        Return True
    End Function

End Class

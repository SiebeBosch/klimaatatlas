﻿Imports System.ComponentModel
Imports System.Data.SQLite
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Transactions
Imports Klimaatatlas.clsGeneralFunctions
Imports NetTopologySuite
Imports NetTopologySuite.Geometries
Imports NetTopologySuite.IO

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
            ' Load the SpatiaLite extension
            If Setup.GpkgCon.State <> ConnectionState.Open Then Setup.GpkgCon.Open()
            Setup.Log.WriteToDiagnosticsFile("Geopackage successfully opened.")

            Setup.GpkgCon.EnableExtensions(True)
            Setup.Log.WriteToDiagnosticsFile("Geopackage extensions enabled.")

            Setup.GpkgCon.LoadExtension("mod_spatialite")
            Setup.Log.WriteToDiagnosticsFile("Geopackage spatialite extension loaded.")

            For Each Scenario As clsScenario In Setup.Scenarios.Values

                Setup.Log.WriteToDiagnosticsFile("Processing scenario " & Scenario.Name)
                Me.Setup.Generalfunctions.UpdateProgressBar($"Processing rule {Name} for scenario {Scenario.Name}...", 0, 10, True)

                Using transaction As SQLiteTransaction = Setup.GpkgCon.BeginTransaction()
                    Setup.Log.WriteToDiagnosticsFile("SQLite transaction started")

                    Dim dt As New DataTable()
                    Dim columnsToUpdate As New List(Of String)    'keep track of the fields that are needed for the results of this rule

                    'establisch a list of fields needed for this rule
                    Dim Fields As List(Of String) = getFieldNamesForScenario(Scenario)
                    Setup.Log.WriteToDiagnosticsFile("Field names collected for scenario " & Scenario.Name)

                    'drop the column. Below we will add it again but so we make sure all results are recalculated AND all columns nicely align to the right of the attributes table
                    RemoveColumnIfExists(Setup.GpkgTable, $"{Scenario.Name}_{Name}")
                    Setup.Log.WriteToDiagnosticsFile("Old results column removed: " & $"{Scenario.Name}_{Name}")

                    'Recreate the results column
                    EnsureColumnExists(Setup.GpkgTable, $"{Scenario.Name}_{Name}", "REAL")
                    Setup.Log.WriteToDiagnosticsFile($"(Re)created column {Scenario.Name}_{Name}")

                    Fields.Add($"{Scenario.Name}_{Name}")
                    columnsToUpdate.Add($"{Scenario.Name}_{Name}")

                    For Each Component As clsEquationComponent In EquationComponents

                        'drop the old component field, if it exists
                        RemoveColumnIfExists(Setup.GpkgTable, $"{Scenario.Name}_{Component.ResultsFieldName}")
                        Setup.Log.WriteToDiagnosticsFile("Old component field removed: " & $"{Scenario.Name}_{Component.ResultsFieldName}")

                        '(re)create the component field
                        EnsureColumnExists(Setup.GpkgTable, $"{Scenario.Name}_{Component.ResultsFieldName}", "REAL")
                        Setup.Log.WriteToDiagnosticsFile("(Re)created field for component " & Component.ResultsFieldName)

                        Fields.Add($"{Scenario.Name}_{Component.ResultsFieldName}")
                        columnsToUpdate.Add($"{Scenario.Name}_{Component.ResultsFieldName}")
                        Setup.Log.WriteToDiagnosticsFile("Field for component " & Component.ResultsFieldName & " created.")
                    Next

                    ' Execute a query to retrieve all features and necessary fields
                    Using cmdSelect As New SQLiteCommand("SELECT fid, " & String.Join(", ", Fields) & " FROM " & Setup.GpkgTable & ";", Setup.GpkgCon)
                        Using adapter As New SQLiteDataAdapter(cmdSelect)
                            adapter.Fill(dt)
                        End Using
                    End Using

                    Me.Setup.Generalfunctions.UpdateProgressBar($"Processing rule {Name} for scenario {Scenario.Name}...", 0, 10, True)
                    Dim iRow As Integer = 0, nRow As Integer = dt.Rows.Count

                    Setup.Log.WriteToDiagnosticsFile("Number of rows to process: " & dt.Rows.Count)
                    For Each row As DataRow In dt.Rows
                        iRow += 1

                        Dim RowSuccessfull As Boolean = True

                        Me.Setup.Generalfunctions.UpdateProgressBar("", iRow, nRow)
                        Dim fid As Integer = Convert.ToInt32(row("fid"))
                        Dim totalWeight As Double = 0
                        Dim ResultSum As Double = 0

                        For Each Component As clsEquationComponent In EquationComponents
                            Dim FieldName As String = Component.Benchmark.FieldNamesPerScenario.Item(Scenario.Name.Trim.ToUpper)
                            Dim Transformation As String = Component.Benchmark.TransformationPerScenario.Item(Scenario.Name.Trim.ToUpper)
                            Dim yValue As Object = row(FieldName) 'initialize the value of yValue to the value as encountered in our geopackage. When a transformation applies this can still change
                            If Transformation <> "" Then
                                'replaced the EvaluateSecondDegreePolynomeExpression function by a more generic one, also supporting exponential functions
                                Dim Result As (Boolean, Double)
                                Result = Setup.Generalfunctions.EvaluateExpression(Transformation, row(FieldName))
                                If Result.Item1 = False Then
                                    Throw New Exception($"Error evaluating mathematical expression {Transformation}. Please check if your equation obeys the a * x^2 + b * x + c or a * EXP(b * x) format convention.")
                                Else
                                    yValue = Result.Item2
                                End If
                            End If

                            totalWeight += Component.Weight
                            Dim myResult As (Boolean, Double) = Component.Benchmark.getResult(yValue)
                            If Not myResult.Item1 Then
                                RowSuccessfull = False
                                Me.Setup.Log.AddError("Error calculating result for row " & iRow & " and field " & FieldName & " and value " & row(FieldName).ToString & " for benchmark " & Component.Benchmark.Name & ".")
                                Exit For
                            Else
                                Component.Result = myResult.Item2 * Component.Weight
                                ResultSum += Component.Result
                            End If
                        Next

                        'if the row was not successfull, skip to the next row
                        If Not RowSuccessfull Then
                            'Me.Setup.Log.AddError("Unable to process row with fid " & fid & ". Skipping to the next row.")
                            Continue For
                        End If

                        'calculate the contribution of each component
                        For Each Component As clsEquationComponent In EquationComponents
                            Dim contribution As Double = Component.Result / totalWeight

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
                    Setup.Log.WriteToDiagnosticsFile("All rows processed.")

                    ' At the end of each scenario, write the results back to the database
                    Setup.Log.WriteToDiagnosticsFile("Writing results back to the database.")
                    For Each row As DataRow In dt.Rows
                        Using cmdUpdate As New SQLiteCommand(Setup.GpkgCon)
                            Dim updateSetClauses As New List(Of String)
                            Dim myfid As Integer = Convert.ToInt32(row("fid"))
                            For Each columnName In columnsToUpdate
                                If dt.Columns.Contains(columnName) Then
                                    updateSetClauses.Add($"{columnName} = @{columnName}")
                                    cmdUpdate.Parameters.AddWithValue($"@{columnName}", row(columnName))
                                End If
                            Next

                            cmdUpdate.CommandText = $"UPDATE {Setup.GpkgTable} SET {String.Join(", ", updateSetClauses)} WHERE fid = {myfid};"
                            cmdUpdate.ExecuteNonQuery()
                        End Using
                    Next
                    Setup.Log.WriteToDiagnosticsFile("Results written.")

                    transaction.Commit() ' Committing transaction after all rows are updated
                End Using
                Setup.Log.WriteToDiagnosticsFile("Scenario " & Scenario.Name & " processed.")
            Next

            Return True
        Catch ex As Exception
            Setup.Log.WriteToDiagnosticsFile("Error executing rule " & Name & ": " & ex.Message)
            Me.Setup.Log.AddError("Error executing rule " & Name & ": " & ex.Message)
            Return False
        End Try
    End Function





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

    Public Function RemoveColumnIfExists(tableName As String, columnName As String) As Boolean
        Try
            Using cmd As New SQLiteCommand($"PRAGMA table_info({tableName});", Setup.GpkgCon)
                Using reader As SQLiteDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        If String.Equals(reader("name").ToString(), columnName, StringComparison.OrdinalIgnoreCase) Then
                            'column exists, remove it
                            Using cmdRemove As New SQLiteCommand($"ALTER TABLE {tableName} DROP COLUMN {columnName};", Setup.GpkgCon)
                                cmdRemove.ExecuteNonQuery()
                                Exit While
                            End Using
                        End If
                    End While
                End Using
            End Using
            Me.Setup.Log.AddMessage("Successfully removed column " & columnName & " from table " & tableName & ".")
            Return True
        Catch ex As Exception
            Me.Setup.Log.AddError("Error removing column " & columnName & " from table " & tableName & ": " & ex.Message)
            Return False
        End Try
    End Function

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

    Public Function calculateweightedResult(value As Object) As Boolean
        Try
            'this function calculates the result of this component for a given value
            'then applies the weight to it and writes it to the Result property
            Dim myResult As (Boolean, Double)
            If Benchmark.getResult(value).Item1 Then
                Result = myResult.Item2 * Weight
                Return True
            Else
                Throw New Exception("Error retrieving result for benchmark " & Benchmark.Name & " and value " & value.ToString)
            End If
        Catch ex As Exception
            Me.Setup.Log.AddError("Error in function calculateResult of class clsRule: " & ex.Message)
            Return False
        End Try

    End Function

End Class

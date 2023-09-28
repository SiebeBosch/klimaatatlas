Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports Klimaatatlas.clsGeneralFunctions

Public Class clsRule
    Friend Name As String
    Friend EquationComponents As New List(Of clsEquationComponent)
    Private Setup As clsKlimaatatlas

    Public Sub New(ByRef mySetup As clsKlimaatatlas)
        Setup = mySetup
    End Sub

    Public Function Execute() As Boolean
        Try

            For Each Scenario As clsScenario In Setup.Scenarios.Values

                'create fields for the results of this rule & scenario: one for the final verdict and one for each component
                Dim ResultField As clsSQLiteField = Setup.featuresDataset.GetAddField(Scenario.Name & "_" & Name, enmFieldType.verdict, enmSQLiteDataType.SQLITEREAL)
                For Each Component As clsEquationComponent In EquationComponents
                    'create a field for the results of this individual component & scenario
                    Component.ResultsField = Setup.featuresDataset.GetAddField(Scenario.Name & "_" & Component.Benchmark.Name, enmFieldType.datavalue, enmSQLiteDataType.SQLITEREAL)
                Next

                'iterate through each feature
                For Each featureIdx As Integer In Setup.featuresDataset.Features.Keys
                    Dim totalWeight As Double = 0
                    Dim ResultSum As Double = 0
                    Dim ComponentResults As New List(Of Double)
                    'retrieve the individual components
                    For Each Component As clsEquationComponent In EquationComponents
                        Dim benchmarkField As clsSQLiteField = Setup.featuresDataset.Fields.Item(Component.Benchmark.fieldname.Trim.ToUpper)
                        Component.calculateResult(Setup.featuresDataset.Values(benchmarkField.fieldIdx, featureIdx))

                        'add the result of this component to the total result
                        ResultSum += Component.Result
                        totalWeight += Component.Weight
                    Next

                    'calculate the relative contribution of each component to the final result
                    For Each Component As clsEquationComponent In EquationComponents
                        'write the relative contribution of this component to the final result to the dataset
                        Setup.featuresDataset.Values(Component.ResultsField.fieldIdx, featureIdx) = If(ResultSum > 0, Component.Result / ResultSum, 0)
                    Next

                    Setup.featuresDataset.Values(ResultField.fieldIdx, featureIdx) = If(totalWeight > 0, ResultSum / totalWeight, 0)
                Next
            Next

            Return True
        Catch ex As Exception
            ' Handle the exception
            Return False
        End Try
    End Function

    ' Removed Parse function since it's not needed for the JSON approach

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
        Benchmark = Setup.Benchmarks.Item(myBenchmarkName.Trim.ToUpper)
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

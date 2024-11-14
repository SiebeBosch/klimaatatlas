Imports Klimaatatlas.clsGeneralFunctions
Imports MapWinGIS
Imports Newtonsoft.Json.Linq
Imports System.IO

Public Class clsBenchmark
    Private Setup As clsKlimaatatlas

    Public Name As String
    Friend FieldNamesPerScenario As Dictionary(Of String, String)
    Friend TransformationPerScenario As Dictionary(Of String, String)
    Friend ConstantsPerScenario As Dictionary(Of String, Double)
    Friend ClassificationType As clsKlimaatatlas.enmClassificationType

    'classification for discrete values
    Dim Classes As New Dictionary(Of String, Double)

    'classification for continuous values
    Friend ValuesRange As New SortedDictionary(Of Double, Double) 'key= value, value=verdict
    Friend NullValue As Double = Double.NaN 'value to assign when the value is null
    Friend colorScale As clsColorScale ' Set your lower and upper values here

    Public Sub New(ByRef myKlimaatatlas As clsKlimaatatlas, myName As String, myFieldNamesPerScenario As Dictionary(Of String, String), myTransformationPerScenario As Dictionary(Of String, String), myConstantsPerScenario As Dictionary(Of String, Double), myClassificationType As clsKlimaatatlas.enmClassificationType)
        Setup = myKlimaatatlas
        Name = myName
        FieldNamesPerScenario = myFieldNamesPerScenario
        TransformationPerScenario = myTransformationPerScenario
        ConstantsPerScenario = myConstantsPerScenario
        ClassificationType = myClassificationType
    End Sub

    Public Function getResult(value As Object) As Double
        If ClassificationType = clsKlimaatatlas.enmClassificationType.Discrete Then
            'cast the value to a string and look up its value in the classes list
            Dim valueString As String = value.ToString
            For i = 0 To Classes.Count - 1
                If Classes.Keys(i).Trim.ToUpper = valueString.Trim.ToUpper Then
                    Return Classes.Values(i)
                    Exit For
                End If
            Next
            Return 0
        ElseIf ClassificationType = clsKlimaatatlas.enmClassificationType.Continuous Then
            'interpolate the value in the values range
            Dim valueDouble As Double = CDbl(value)
            If valueDouble <= ValuesRange.Keys(0) Then
                Return ValuesRange.Values(0)
            ElseIf valueDouble > ValuesRange.Keys(ValuesRange.Keys.Count - 1) Then
                Return ValuesRange.Values(ValuesRange.Keys.Count - 1)
            Else
                Dim i As Integer = 0
                While valueDouble > ValuesRange.Keys(i)
                    i += 1
                End While
                Dim lowerValue As Double = ValuesRange.Keys(i - 1)
                Dim upperValue As Double = ValuesRange.Keys(i)
                Dim lowerVerdict As Double = ValuesRange.Values(i - 1)
                Dim upperVerdict As Double = ValuesRange.Values(i)
                Return lowerVerdict + (valueDouble - lowerValue) * (upperVerdict - lowerVerdict) / (upperValue - lowerValue)
            End If
        Else
            Return 0
        End If
    End Function

    Public Sub SetDiscreteClasses(myClasses As Dictionary(Of String, Double))
        Classes = myClasses
        colorScale = New clsColorScale(Setup, Classes)
    End Sub

    Public Sub SetContinuousClasses(myValuesRange As SortedDictionary(Of Double, Double))
        ValuesRange = myValuesRange
        colorScale = New clsColorScale(Setup, myValuesRange)
    End Sub

End Class

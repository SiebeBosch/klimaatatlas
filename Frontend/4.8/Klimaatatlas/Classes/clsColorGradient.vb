Imports System.Drawing
Imports HYDROC01.General

Public Class clsColorGradient

    Private Setup As clsKlimaatatlas

    Dim LowerValue As Double
    Dim UpperValue As Double
    Dim LowerColor As Color
    Dim UpperColor As Color
    Dim TransparentAtLowest As Boolean          'whether the lowest value itself should be made transparent
    Dim TransparentAtHighest As Boolean        'whether the highest value itself should be made transparent
    Dim TransparentBelowLowest As Boolean
    Dim TransparentAboveHighest As Boolean

    Public Sub New(ByRef mySetup As clsKlimaatatlas)
        Setup = mySetup
    End Sub

    Public Function getTransparentBelowLowest() As Boolean
        Return TransparentBelowLowest
    End Function

    Public Function getTransparentAboveHighest() As Boolean
        Return TransparentAboveHighest
    End Function

    Public Function GetTransparentAtLowest() As Boolean
        Return TransparentAtLowest
    End Function

    Public Function getTransparentAtHighest() As Boolean
        Return TransparentAtHighest
    End Function

    Public Function getLowerValue() As Double
        Return LowerValue
    End Function

    Public Function getUpperValue() As Double
        Return UpperValue
    End Function

    Public Function GetLowerColor() As Color
        Return LowerColor
    End Function

    Public Function GetUpperColor() As Color
        Return UpperColor
    End Function

    Public Function Generate(myLowerValue As Double, myLowerColor As Color, myUpperValue As Double, myUpperColor As Color, myTransparentBelowLowest As Boolean, myTransparentAboveHighest As Boolean, myTransparentAtLowest As Boolean, myTransparentAtHighest As Boolean) As Boolean
        LowerValue = myLowerValue
        UpperValue = myUpperValue
        LowerColor = myLowerColor
        UpperColor = myUpperColor
        TransparentBelowLowest = myTransparentBelowLowest
        TransparentAboveHighest = myTransparentAboveHighest
        TransparentAtLowest = myTransparentAtLowest
        TransparentAtHighest = myTransparentAtHighest
    End Function
    Public Function InterpolateRGBColorForValue(Value As Double) As Color
        Return Setup.GeneralFunctions.Interpolate2RGBColors(LowerColor, UpperColor, LowerValue, UpperValue, Value)
    End Function

    Public Function InterpolateRGBAColorForValue(Value As Double) As Color
        Return Setup.GeneralFunctions.Interpolate2RGBAColors(LowerColor, UpperColor, LowerValue, UpperValue, TransparentBelowLowest, TransparentAboveHighest, TransparentAtLowest, TransparentAtHighest, Value)
    End Function



    Public Function BuildGradientJSONString(Parameter As String, IsLastItemInArray As Boolean) As String
        Dim JSONStr As String = "       {" & vbCrLf
        JSONStr &= "            " & """parameter"":""" & Parameter & """," & vbCrLf
        JSONStr &= "            " & """fromColor"":{""R"":" & GetLowerColor.R & "," & """G"":" & GetLowerColor.G & "," & """B"":" & GetLowerColor.B & "," & """A"":" & GetLowerColor.A & "}," & vbCrLf
        JSONStr &= "            " & """toColor"":{""R"":" & GetUpperColor.R & "," & """G"":" & GetUpperColor.G & "," & """B"":" & GetUpperColor.B & "," & """A"":" & GetUpperColor.A & "}," & vbCrLf
        JSONStr &= "            " & """fromValue"":" & getLowerValue() & "," & vbCrLf
        JSONStr &= "            " & """toValue"":" & getUpperValue() & "," & vbCrLf
        JSONStr &= "            " & """transparentBelowLowest"":" & getTransparentBelowLowest().ToString.ToLower & "," & vbCrLf
        JSONStr &= "            " & """transparentAtLowest"":" & GetTransparentAtLowest().ToString.ToLower & "," & vbCrLf
        JSONStr &= "            " & """transparentAboveHighest"":" & getTransparentAboveHighest().ToString.ToLower & "," & vbCrLf
        JSONStr &= "            " & """transparentAtHighest"":" & getTransparentAtHighest().ToString.ToLower & "," & vbCrLf
        JSONStr &= "        }"
        If IsLastItemInArray Then
            JSONStr &= vbCrLf
        Else
            JSONStr &= "," & vbCrLf
        End If
        Return JSONStr
    End Function


    Public Function BuildLegendJSONString(Parameter As String, LegendTitle As String, IsLastItemInArray As Boolean, ValuesMultiplier As Double) As String
        'note: the values multiplier is used for situations where the actual values for the map are in e.g. cm or cm/s
        'and the legend is desired to express results in m or m/s
        'in that case the multiplier should be set to 0.01
        Dim ClassBoundaries As Dictionary(Of String, Color)
        Dim JSONStr As String = "       {" & vbCrLf
        JSONStr &= "            " & """title"":" & """" & LegendTitle & """," & vbCrLf
        JSONStr &= "            " & """parameter"":" & """" & Parameter & """," & vbCrLf

        JSONStr &= "            " & """classes"":[" & vbCrLf
        ClassBoundaries = GenerateColorLegend(5, 11, ValuesMultiplier)
        For i = 0 To ClassBoundaries.Count - 1
            Dim colorString As String = "RGBA(" & ClassBoundaries.Values(i).R & "," & ClassBoundaries.Values(i).G & "," & ClassBoundaries.Values(i).B & "," & ClassBoundaries.Values(i).A & ")"
            JSONStr &= "                {" & """title"":""" & ClassBoundaries.Keys(i) & """, ""color"":""" & colorString & """}," & vbCrLf
        Next
        JSONStr &= "            ]" & vbCrLf

        If Not IsLastItemInArray Then
            JSONStr &= "        }," & vbCrLf
        Else
            JSONStr &= "        }" & vbCrLf
        End If
        Return JSONStr
    End Function

    Public Function GenerateColorLegend(MinClasses As Integer, MaxClasses As Integer, ValuesMultiplier As Double) As Dictionary(Of String, Color)
        'this function attempts to find an optimal subdivision of a given values range, such that the number of decimals in each class is as small as possible
        'try to find a neat cut between upper and lower value

        'note: the values multiplier is used for situations where the actual values for the map are in e.g. cm or cm/s
        'and the legend is desired to express results in m or m/s
        'in that case the multiplier should be set to 0.01
        Dim Legend As New Dictionary(Of String, Color)

        Dim ClassSize As Double
        Dim CurVal As Double
        Dim nDecimals As Integer

        Dim BestnClasses As Integer = MaxClasses
        Dim BestnDecimals As Integer = 7

        For nClasses = MinClasses To MaxClasses
            ClassSize = (UpperValue - LowerValue) / nClasses * ValuesMultiplier 'v2.4.5.5
            For nDecimals = 0 To 6
                If Math.Round(ClassSize, nDecimals) = ClassSize Then
                    'we found a class size where the number of decimals is limited. Now compare with the best so far
                    If nDecimals < BestnDecimals Then
                        BestnDecimals = nDecimals
                        BestnClasses = nClasses
                    End If
                    Exit For
                End If
            Next
        Next

        'finalize our classification
        ClassSize = (UpperValue - LowerValue) / BestnClasses
        Dim LegendStr As String = ""
        Dim LegendColor As Color

        'start with the lower end of our legend
        If TransparentBelowLowest Then
            LegendStr = "< " & Math.Round(LowerValue * ValuesMultiplier, BestnDecimals).ToString
            LegendColor = InterpolateRGBAColorForValue(LowerValue - 1)
            If Not Legend.ContainsKey(LegendStr) Then Legend.Add(LegendStr, LegendColor)
            'If Not Legend.ContainsKey(LegendStr) Then Legend.Add(Math.Round(LowerValue * ValuesMultiplier, BestnDecimals).ToString, LowerColor)
        Else
            'the lowest color is valid for all values <= lowervalue
            LegendStr = "<= " & Math.Round(LowerValue * ValuesMultiplier, BestnDecimals).ToString
            If Not Legend.ContainsKey(LegendStr) Then Legend.Add(LowerValue * ValuesMultiplier, LowerColor)
        End If

        For i = 1 To BestnClasses - 1
            CurVal = LowerValue + i * ClassSize
            LegendStr = Math.Round(CurVal * ValuesMultiplier, BestnDecimals).ToString
            LegendColor = InterpolateRGBAColorForValue(CurVal)
            If Not Legend.ContainsKey(LegendStr) Then Legend.Add(LegendStr, LegendColor)
        Next
        If TransparentAboveHighest Then
            'Legend.Add(Math.Round(UpperValue, BestnDecimals).ToString, UpperColor)
            LegendStr = "> " & Math.Round(UpperValue * ValuesMultiplier, BestnDecimals).ToString
            LegendColor = InterpolateRGBAColorForValue(UpperValue + 1)
            If Not Legend.ContainsKey(LegendStr) Then Legend.Add(LegendStr, LegendColor)
        Else
            LegendStr = ">= " & Math.Round(UpperValue * ValuesMultiplier, BestnDecimals).ToString
            If Not Legend.ContainsKey(LegendStr) Then Legend.Add(LegendStr, UpperColor)
        End If

        Return Legend

    End Function

End Class

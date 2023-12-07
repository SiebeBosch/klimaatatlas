Imports System.Drawing
Imports System.Drawing.Drawing2D
Public Class clsColorScale

    Private Setup As clsKlimaatatlas
    Dim ClassificationType As clsKlimaatatlas.enmClassificationType
    Private ValuesRange As SortedDictionary(Of Double, Double)        'for continuous values
    Private Classes As Dictionary(Of String, Double)            'for discrete values

    Public Sub New(_Setup As clsKlimaatatlas, myValuesRange As SortedDictionary(Of Double, Double))
        Setup = _Setup
        ClassificationType = clsKlimaatatlas.enmClassificationType.Continuous
        ValuesRange = myValuesRange
    End Sub

    Public Sub New(_Setup As clsKlimaatatlas, _Classes As Dictionary(Of String, Double))
        Setup = _Setup
        ClassificationType = clsKlimaatatlas.enmClassificationType.Discrete
        Classes = _Classes
    End Sub

    Public Sub DrawColorScale(e As PaintEventArgs, rect As Rectangle)

        ' Adjust StringFormat flags for centered alignment of the text inside each color bar
        Dim format As New StringFormat() With {
                    .Alignment = StringAlignment.Center,
                    .LineAlignment = StringAlignment.Center
                }

        ' Set a new format for drawing numerical value below the color bar
        Dim belowBarFormat As New StringFormat() With {
                    .Alignment = StringAlignment.Center,
                    .LineAlignment = StringAlignment.Near
                }

        If ClassificationType = clsKlimaatatlas.enmClassificationType.Continuous Then

            ' Set up the linear gradient brush and color blending

            'if the values are descending, draw the color scale from red to green
            'if the values are ascending, draw the color scale from green to red

            Dim lgb As LinearGradientBrush
            Dim cb As ColorBlend
            If ValuesRange.Values(0) > ValuesRange.Values(ValuesRange.Keys.Count - 1) Then
                lgb = New LinearGradientBrush(rect, Color.Red, Color.Green, 0.0F)
                cb = New ColorBlend()
                cb.Colors = New Color() {Color.Red, Color.Yellow, Color.Green}
            Else
                lgb = New LinearGradientBrush(rect, Color.Green, Color.Red, 0.0F)
                cb = New ColorBlend()
                cb.Colors = New Color() {Color.Green, Color.Yellow, Color.Red}
            End If

            cb.Positions = New Single() {0.0F, 0.5F, 1.0F}
            lgb.InterpolationColors = cb

            ' Draw the gradient bar
            e.Graphics.FillRectangle(lgb, rect)

            e.Graphics.DrawString(ValuesRange.Keys(0), New Font("Arial", 10), Brushes.Black, rect.Left, rect.Bottom, belowBarFormat)
            e.Graphics.DrawString(ValuesRange.Keys(ValuesRange.Keys.Count - 1), New Font("Arial", 10), Brushes.Black, rect.Right, rect.Bottom, belowBarFormat)
        ElseIf ClassificationType = clsKlimaatatlas.enmClassificationType.Discrete Then
            Dim numberOfClasses As Integer = Classes.Count
            Dim classWidth As Single = rect.Width / numberOfClasses

            For i As Integer = 0 To numberOfClasses - 1
                Dim classValue As Double = Classes.Values(i)
                Dim normalizedValue As Single = CSng((classValue - 0) / (1 - 0))
                Dim classColor As Color = InterpolateColor(Color.Green, Color.Red, normalizedValue)

                Dim classRect As New RectangleF(rect.Left + i * classWidth, rect.Top, classWidth, rect.Height)
                Using brush As New SolidBrush(classColor)
                    e.Graphics.FillRectangle(brush, classRect)
                End Using


                ' Draw the textual value inside the color bar
                e.Graphics.DrawString(Classes.Keys(i), New Font("Arial", 10), Brushes.Black, classRect, format)

                ' Draw the numerical value below the color bar
                'e.Graphics.DrawString(Classes(i).Item2.ToString(), New Font("Arial", 10), Brushes.Black, classRect.Left + classWidth / 2, rect.Bottom, belowBarFormat)
            Next
        End If
    End Sub
    Private Function InterpolateColor(color1 As Color, color2 As Color, fraction As Single) As Color
        Dim middleColor As Color = Color.Yellow ' Set your desired middle color

        If fraction <= 0.5 Then
            Return InterpolateBetweenColors(color1, middleColor, fraction * 2)
        Else
            Return InterpolateBetweenColors(middleColor, color2, (fraction - 0.5) * 2)
        End If
    End Function

    Private Function InterpolateBetweenColors(color1 As Color, color2 As Color, fraction As Single) As Color
        Dim hsl1 As Tuple(Of Double, Double, Double) = RgbToHsl(color1)
        Dim hsl2 As Tuple(Of Double, Double, Double) = RgbToHsl(color2)

        Dim h As Double = hsl1.Item1 + fraction * (hsl2.Item1 - hsl1.Item1)
        Dim s As Double = hsl1.Item2 + fraction * (hsl2.Item2 - hsl1.Item2)
        Dim l As Double = hsl1.Item3 + fraction * (hsl2.Item3 - hsl1.Item3)

        Return HslToRgb(h, s, l)
    End Function

    Private Function RgbToHsl(c As Color) As Tuple(Of Double, Double, Double)
        Dim r As Double = c.R / 255.0
        Dim g As Double = c.G / 255.0
        Dim b As Double = c.B / 255.0

        Dim max As Double = Math.Max(r, Math.Max(g, b))
        Dim min As Double = Math.Min(r, Math.Min(g, b))
        Dim delta As Double = max - min

        Dim h As Double = 0
        Dim s As Double = 0
        Dim l As Double = (max + min) / 2.0

        If delta <> 0 Then
            s = If(l < 0.5, delta / (max + min), delta / (2.0 - max - min))

            If r = max Then
                h = (g - b) / delta
            ElseIf g = max Then
                h = 2 + (b - r) / delta
            ElseIf b = max Then
                h = 4 + (r - g) / delta
            End If

            h *= 60
            If h < 0 Then h += 360
        End If

        Return Tuple.Create(h, s, l)
    End Function

    Private Function HslToRgb(h As Double, s As Double, l As Double) As Color
        Dim r, g, b As Double

        If s = 0 Then
            r = l
            g = l
            b = l
        Else
            Dim q As Double = If(l < 0.5, l * (1 + s), l + s - l * s)
            Dim p As Double = 2 * l - q

            h = h / 360 ' Convert from degrees to decimal
            r = HueToRgb(p, q, h + 1 / 3.0)
            g = HueToRgb(p, q, h)
            b = HueToRgb(p, q, h - 1 / 3.0)
        End If

        ' Clamp and convert to integer
        Return Color.FromArgb(Clamp(CInt(Math.Round(r * 255)), 0, 255),
                          Clamp(CInt(Math.Round(g * 255)), 0, 255),
                          Clamp(CInt(Math.Round(b * 255)), 0, 255))
    End Function

    Private Function Clamp(value As Integer, min As Integer, max As Integer) As Integer
        Return If(value < min, min, If(value > max, max, value))
    End Function

    Private Function HueToRgb(p As Double, q As Double, t As Double) As Double
        If t < 0 Then t += 1
        If t > 1 Then t -= 1
        If t < 1 / 6.0 Then Return p + (q - p) * 6 * t
        If t < 1 / 2.0 Then Return q
        If t < 2 / 3.0 Then Return p + (q - p) * (2 / 3.0 - t) * 6
        Return p
    End Function

End Class

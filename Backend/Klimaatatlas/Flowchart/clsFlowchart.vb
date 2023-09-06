Imports System.Collections.ObjectModel
Imports Klimaatatlas.clsGeneralFunctions
''' <summary>
''' 'This class represents a flowchart.
''' </summary>
Public Class clsFlowchart
    Public Nodes As Dictionary(Of String, clsFlowchartNode)
    Public Connections As Dictionary(Of String, clsFlowchartConnection)


    Public Sub New()
        Nodes = New Dictionary(Of String, clsFlowchartNode)
        Connections = New Dictionary(Of String, clsFlowchartConnection)

        CreateDummyChart()


    End Sub

    Public Sub CreateDummyChart()
        'for debugging purposes
        'dummydata
        Dim node1 As New clsFlowchartInput("Tgem.zom", New Point(20, 20))
        Dim node2 As New clsFlowchartInput("Diepte", New Point(320, 20))
        Dim node3 As New clsFlowchartDecision("> 15", New Point(20, 100), ">15")
        Dim node4 As New clsFlowChartMultiSelect("Waterdiepte1", New Point(320, 100), New List(Of String) From {">0.75", "[0.50-0.75]", "<=0.50"}, New List(Of Color) From {Color.Green, Color.Green, Color.Green})
        Dim node5 As New clsFlowChartMultiSelect("Waterdiepte2", New Point(320, 200), New List(Of String) From {">0.75", "[0.50-0.75]", "<=0.50"}, New List(Of Color) From {Color.Green, Color.Green, Color.Green})
        Dim node6 As New clsFlowChartVerdict("Kroos", New Point(520, 100), New List(Of String) From {"goed", "middelmatig", "slecht"}, New List(Of Color) From {Color.Green, Color.Yellow, Color.Red})
        Dim connection1 As New clsFlowchartConnection(node1, clsGeneralFunctions.enmFlowChartNodeEdge.bottom, 0, node3, clsGeneralFunctions.enmFlowChartNodeEdge.top, 0)
        Dim connection2 As New clsFlowchartConnection(node3, clsGeneralFunctions.enmFlowChartNodeEdge.right, 0, node4, clsGeneralFunctions.enmFlowChartNodeEdge.left, 0)
        Dim connection3 As New clsFlowchartConnection(node3, clsGeneralFunctions.enmFlowChartNodeEdge.bottom, 0, node5, clsGeneralFunctions.enmFlowChartNodeEdge.left, 0)

        Dim connection4 As New clsFlowchartConnection(node4, clsGeneralFunctions.enmFlowChartNodeEdge.right, 0, node6, clsGeneralFunctions.enmFlowChartNodeEdge.left, 1)
        Dim connection4a As New clsFlowchartConnection(node4, clsGeneralFunctions.enmFlowChartNodeEdge.right, 1, node6, clsGeneralFunctions.enmFlowChartNodeEdge.left, 2)
        Dim connection4b As New clsFlowchartConnection(node4, clsGeneralFunctions.enmFlowChartNodeEdge.right, 2, node6, clsGeneralFunctions.enmFlowChartNodeEdge.left, 2)

        Dim connection5 As New clsFlowchartConnection(node5, clsGeneralFunctions.enmFlowChartNodeEdge.right, 0, node6, clsGeneralFunctions.enmFlowChartNodeEdge.left, 0)
        Dim connection5a As New clsFlowchartConnection(node5, clsGeneralFunctions.enmFlowChartNodeEdge.right, 1, node6, clsGeneralFunctions.enmFlowChartNodeEdge.left, 1)
        Dim connection5b As New clsFlowchartConnection(node5, clsGeneralFunctions.enmFlowChartNodeEdge.right, 2, node6, clsGeneralFunctions.enmFlowChartNodeEdge.left, 2)

        Dim connection6 As New clsFlowchartConnection(node2, clsGeneralFunctions.enmFlowChartNodeEdge.bottom, 0, node4, clsGeneralFunctions.enmFlowChartNodeEdge.top, 0)

        addNode(node1)
        addNode(node2)
        addNode(node3)
        addNode(node4)
        addNode(node5)
        addNode(node6)
        addConnection(connection1)
        addConnection(connection6)
        addConnection(connection2)
        addConnection(connection3)
        addConnection(connection4)
        addConnection(connection4a)
        addConnection(connection4b)
        addConnection(connection5)
        addConnection(connection5a)
        addConnection(connection5b)

    End Sub

    Public Sub addNode(node As clsFlowchartNode)
        Nodes.Add(node.Label, node)
    End Sub

    Public Sub addConnection(connection As clsFlowchartConnection)
        Connections.Add(connection.FromNode.Label & "_" & connection.FromItemIdx & "-" & connection.ToNode.Label & "_" & connection.ToItemIdx, connection)
    End Sub

    Public Sub connectNodes(fromNode As clsFlowchartNode, fromEdge As clsGeneralFunctions.enmFlowChartNodeEdge, fromItemIdx As Integer, toNode As clsFlowchartNode, toEdge As clsGeneralFunctions.enmFlowChartNodeEdge, toItemIdx As Integer)
        Dim connection As New clsFlowchartConnection(fromNode, fromEdge, fromItemIdx, toNode, toEdge, toItemIdx)
        Connections.Add(fromNode.Label & "-" & toNode.Label, connection)
    End Sub

    Public Sub DrawFlowchart(ByRef g As Graphics)
        Dim myPen As New Pen(Color.Black)
        Dim myBrush As New SolidBrush(Color.Black)
        Dim myFont As New Font("Arial", 10)
        Dim myStringFormat As New StringFormat()
        myStringFormat.Alignment = StringAlignment.Center
        myStringFormat.LineAlignment = StringAlignment.Center

        ' Draw the flowchart nodes
        For Each node As clsFlowchartNode In Nodes.Values
            node.Draw(g)
        Next

        ' Update code for connections
        For Each connection As clsFlowchartConnection In Connections.Values
            ' Calculate the start and end coordinates based on the specified edges
            Dim startX, startY, endX, endY As Integer

            ' Setting up coordinates for the FromNode based on FromEdge
            Select Case connection.FromEdge
                Case enmFlowChartNodeEdge.left
                    startX = connection.FromNode.Position.X
                    startY = connection.FromNode.Position.Y + 25
                Case enmFlowChartNodeEdge.right
                    startX = connection.FromNode.Position.X + 100
                    startY = connection.FromNode.Position.Y + 25
                    If TypeOf connection.FromNode Is clsFlowChartMultiSelect Then
                        Dim multiDecisionNode As clsFlowChartMultiSelect = TryCast(connection.FromNode, clsFlowChartMultiSelect)
                        If multiDecisionNode IsNot Nothing Then
                            startX = multiDecisionNode.GetX(connection.FromEdge, connection.FromItemIdx)
                            startY = multiDecisionNode.GetY(connection.FromEdge, connection.FromItemIdx)
                        End If
                    End If
                Case enmFlowChartNodeEdge.top
                    startX = connection.FromNode.Position.X + 50
                    startY = connection.FromNode.Position.Y
                Case enmFlowChartNodeEdge.bottom
                    startX = connection.FromNode.Position.X + 50
                    startY = connection.FromNode.Position.Y + 50
                Case Else
                    startX = connection.FromNode.Position.X + 100
                    startY = connection.FromNode.Position.Y + 25
            End Select

            ' Setting up coordinates for the ToNode based on ToEdge
            Select Case connection.ToEdge
                Case enmFlowChartNodeEdge.left
                    endX = connection.ToNode.Position.X
                    endY = connection.ToNode.Position.Y
                    If TypeOf connection.ToNode Is clsFlowChartVerdict Then
                        Dim Node As clsFlowChartVerdict = TryCast(connection.ToNode, clsFlowChartVerdict)
                        If Node IsNot Nothing Then
                            endX = Node.GetX(connection.ToEdge, connection.ToItemIdx)
                            endY = Node.GetY(connection.ToEdge, connection.ToItemIdx)
                        End If
                    ElseIf TypeOf connection.ToNode Is clsFlowChartMultiSelect Then
                        Dim Node As clsFlowChartMultiSelect = TryCast(connection.ToNode, clsFlowChartMultiSelect)
                        If Node IsNot Nothing Then
                            endX = Node.GetX(connection.ToEdge, connection.ToItemIdx)
                            endY = Node.GetY(connection.ToEdge, connection.ToItemIdx)
                        End If
                    End If
                        Case enmFlowChartNodeEdge.right
                    endX = connection.ToNode.Position.X + 100
                    endY = connection.ToNode.Position.Y + 25
                Case enmFlowChartNodeEdge.top
                    endX = connection.ToNode.Position.X + 50
                    endY = connection.ToNode.Position.Y
                Case enmFlowChartNodeEdge.bottom
                    endX = connection.ToNode.Position.X + 50
                    endY = connection.ToNode.Position.Y + 50
                Case Else
                    endX = connection.ToNode.Position.X
                    endY = connection.ToNode.Position.Y + 25
            End Select

            ' Draw the connection
            g.DrawLine(myPen, startX, startY, endX, endY)
        Next
    End Sub

End Class


Public Class clsFlowchartDecision
    Inherits clsFlowchartNode

    Public Clause As String

    Public Sub New(mylabel As String, myPosition As Point, myClause As String)
        MyBase.New(enmFlowchartNodeType.Decision, mylabel, myPosition)
        Clause = myClause
    End Sub

    Public Overrides Sub Draw(ByRef g As Graphics)
        Dim myPen As New Pen(Color.Black)
        Dim myTextBrush As New SolidBrush(Color.Black)
        Dim myFillBrush As New SolidBrush(Color.LightBlue)
        Dim myFont As New Font("Arial", 10)
        Dim myStringFormat As New StringFormat()
        myStringFormat.Alignment = StringAlignment.Center
        myStringFormat.LineAlignment = StringAlignment.Center

        ' Draw the diamond shape for decision node
        Dim points() As Point = {
        New Point(Position.X + 50, Position.Y),
        New Point(Position.X + 100, Position.Y + 25),
        New Point(Position.X + 50, Position.Y + 50),
        New Point(Position.X, Position.Y + 25)
    }
        g.FillPolygon(myFillBrush, points)
        g.DrawPolygon(myPen, points)

        ' Draw the label inside the diamond
        g.DrawString(Label, myFont, myTextBrush, New RectangleF(Position.X, Position.Y, 100, 50), myStringFormat)

        ' Adjustments for "Yes" and "No" positions
        Dim noSize As SizeF = g.MeasureString("No", myFont)

        ' Draw "Yes" on the right side of the diamond
        g.DrawString("Yes", myFont, myTextBrush, Position.X + 110, Position.Y + 25 - myFont.GetHeight(g))

        ' Draw "No" on the bottom side of the diamond, adjusted for half the text width
        g.DrawString("No", myFont, myTextBrush, Position.X + 50 + myFont.GetHeight(g), Position.Y + 55)
    End Sub


End Class

Public Class clsFlowchartInput
    Inherits clsFlowchartNode

    Public Sub New(mylabel As String, myPosition As Point)
        MyBase.New(enmFlowchartNodeType.Input, mylabel, myPosition)
    End Sub

    Public Overrides Sub Draw(ByRef g As Graphics)
        Dim myPen As New Pen(Color.Black)
        Dim myTextBrush As New SolidBrush(Color.Black)
        Dim myFillBrush As New SolidBrush(Color.LightYellow)  ' New Brush for fill color, assuming you want it yellow for inputs
        Dim myFont As New Font("Arial", 10)
        Dim myStringFormat As New StringFormat()
        myStringFormat.Alignment = StringAlignment.Center
        myStringFormat.LineAlignment = StringAlignment.Center

        ' Draw the oval shape for input node
        Dim ovalRectangle As New Rectangle(Position.X, Position.Y, 100, 50)
        g.FillEllipse(myFillBrush, ovalRectangle)  ' Fill the oval first
        g.DrawEllipse(myPen, ovalRectangle)  ' Then draw the outline

        ' Draw the label inside the oval
        g.DrawString(Label, myFont, myTextBrush, New RectangleF(Position.X, Position.Y, 100, 50), myStringFormat)
    End Sub

End Class
Public Class clsFlowChartMultiSelect
    Inherits clsFlowchartNode

    Public Classes As List(Of String)
    Public Colors As List(Of Color) ' New member for the list of colors
    Public Property RowHeight As Integer = 20 ' Added RowHeight property with a default value of 20
    Public Property LeftPadding As Integer = 5

    Public Sub New(mylabel As String, myPosition As Point, myClasses As List(Of String), myColors As List(Of Color))
        MyBase.New(enmFlowchartNodeType.MultiSelect, mylabel, myPosition)
        Classes = myClasses
        Colors = myColors
    End Sub

    Public Overrides Sub Draw(ByRef g As Graphics)
        Dim myPen As New Pen(Color.Black)
        Dim myTextBrush As New SolidBrush(Color.Black)
        Dim myFont As New Font("Arial", 10)

        Dim triangleCount As Integer = Classes.Count
        Dim rectWidth As Integer = 100
        Dim triangleBase As Integer = 10

        ' Use RowHeight for triangle height
        Dim triangleHeight As Integer = RowHeight

        ' Draw the main label above the object
        'Dim labelRect As New RectangleF(Position.X, Position.Y - myFont.Height - 5, rectWidth, myFont.Height)
        'Dim labelFormat As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Near}
        'g.DrawString(Label, myFont, myTextBrush, labelRect, labelFormat)

        ' Draw each row
        For i = 0 To Classes.Count - 1
            Dim currentColor As Color = Colors(i)
            Dim myFillBrush As New SolidBrush(currentColor) ' Updated fill brush for this row
            Dim rowTopLeft As New Point(Position.X, Position.Y + i * triangleHeight)
            DrawRow(g, rowTopLeft, triangleBase, triangleHeight, rectWidth, Classes(i), myPen, myFillBrush, myTextBrush, myFont)
        Next
    End Sub

    Public Function GetX(edge As enmFlowChartNodeEdge, classIndex As Integer) As Integer
        Dim triangleBase As Integer = 10
        Dim rectWidth As Integer = 100
        Dim triangleHeight As Integer = RowHeight

        Select Case edge
            Case enmFlowChartNodeEdge.top, enmFlowChartNodeEdge.bottom
                Return Position.X + rectWidth / 2

            Case enmFlowChartNodeEdge.right
                Return Position.X + rectWidth + triangleBase

            Case enmFlowChartNodeEdge.left
                Return Position.X

            Case Else
                Throw New Exception("Invalid edge specified.")
        End Select
    End Function

    Public Function GetY(edge As enmFlowChartNodeEdge, classIndex As Integer) As Integer
        Dim triangleHeight As Integer = RowHeight

        Select Case edge
            Case enmFlowChartNodeEdge.top
                Return Position.Y + classIndex * triangleHeight

            Case enmFlowChartNodeEdge.right
                Return Position.Y + classIndex * triangleHeight + triangleHeight / 2

            Case enmFlowChartNodeEdge.left
                'this is the throughput side of our multiselect node. Connect our input to the center of the left edge
                Return Position.Y + (Classes.Count * RowHeight) / 2

            Case enmFlowChartNodeEdge.bottom
                Return Position.Y + (classIndex + 1) * triangleHeight

            Case Else
                Throw New Exception("Invalid edge specified.")
        End Select
    End Function


    Private Sub DrawRow(ByRef g As Graphics, topLeft As Point, triangleBase As Integer, triangleHeight As Integer, rectWidth As Integer, decision As String, myPen As Pen, myFillBrush As SolidBrush, myTextBrush As SolidBrush, myFont As Font)
        Dim myStringFormat As New StringFormat() With {
            .Alignment = StringAlignment.Near,
            .LineAlignment = StringAlignment.Center
        }

        ' Points for the triangle on the right side
        Dim trianglePoints() As Point = {
            New Point(topLeft.X + rectWidth, topLeft.Y),
            New Point(topLeft.X + rectWidth + triangleBase, topLeft.Y + triangleHeight / 2),
            New Point(topLeft.X + rectWidth, topLeft.Y + triangleHeight)
        }

        ' Draw triangle
        g.FillPolygon(myFillBrush, trianglePoints)

        ' Points for rectangle to the left of the triangle
        Dim rectPoints() As Point = {
            topLeft,
            New Point(topLeft.X + rectWidth, topLeft.Y),
            New Point(topLeft.X + rectWidth, topLeft.Y + triangleHeight),
            New Point(topLeft.X, topLeft.Y + triangleHeight)
        }

        ' Draw rectangle
        g.FillPolygon(myFillBrush, rectPoints)

        ' Outline for the combined shape (rectangle + triangle)
        Dim combinedPoints() As Point = {
            rectPoints(0),
            rectPoints(1),
            trianglePoints(1),
            trianglePoints(2),
            rectPoints(3)
        }
        g.DrawPolygon(myPen, combinedPoints)

        ' Draw the label inside the rectangle, to the left of the triangle
        Dim labelPosition As New PointF(topLeft.X + LeftPadding, topLeft.Y + triangleHeight / 2)
        g.DrawString(decision, myFont, myTextBrush, labelPosition, myStringFormat)
    End Sub



End Class




Public Class clsFlowChartVerdict
    Inherits clsFlowchartNode

    Public Verdicts As List(Of String)
    Public Colors As List(Of Color) ' New member for the list of colors
    Public Property RowHeight As Integer = 20 ' Added RowHeight property with a default value of 20

    Public Sub New(mylabel As String, myPosition As Point, myVerdicts As List(Of String), myColors As List(Of Color))
        MyBase.New(enmFlowchartNodeType.Verdict, mylabel, myPosition)
        Verdicts = myVerdicts
        Colors = myColors
    End Sub


    Public Function GetX(edge As enmFlowChartNodeEdge, classIndex As Integer) As Integer
        Dim triangleBase As Integer = 10
        Dim rectWidth As Integer = 100
        Dim triangleHeight As Integer = RowHeight

        Select Case edge
            Case enmFlowChartNodeEdge.top, enmFlowChartNodeEdge.bottom
                Return Position.X + rectWidth / 2

            Case enmFlowChartNodeEdge.right
                Return Position.X

            Case enmFlowChartNodeEdge.left
                Return Position.X

            Case Else
                Throw New Exception("Invalid edge specified.")
        End Select
    End Function

    Public Function GetY(edge As enmFlowChartNodeEdge, classIndex As Integer) As Integer
        Dim triangleHeight As Integer = RowHeight

        Select Case edge
            Case enmFlowChartNodeEdge.top
                Return Position.Y + classIndex * triangleHeight

            Case enmFlowChartNodeEdge.right, enmFlowChartNodeEdge.left
                Return Position.Y + classIndex * triangleHeight + triangleHeight / 2

            Case enmFlowChartNodeEdge.bottom
                Return Position.Y + (classIndex + 1) * triangleHeight

            Case Else
                Throw New Exception("Invalid edge specified.")
        End Select
    End Function

    Public Overrides Sub Draw(ByRef g As Graphics)
        Dim myPen As New Pen(Color.Black)
        Dim myTextBrush As New SolidBrush(Color.Black)
        Dim myFont As New Font("Arial", 10)

        Dim triangleCount As Integer = Verdicts.Count
        Dim rectWidth As Integer = 100
        Dim triangleBase As Integer = 10

        ' Use RowHeight for triangle height
        Dim triangleHeight As Integer = RowHeight

        ' Draw the main label above the object
        Dim labelRect As New RectangleF(Position.X, Position.Y - myFont.Height - 5, rectWidth + triangleBase, myFont.Height)
        Dim labelFormat As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Near}
        g.DrawString(Label, myFont, myTextBrush, labelRect, labelFormat)

        ' Draw each row
        For i = 0 To Verdicts.Count - 1
            Dim currentColor As Color = Colors(i)
            Dim myFillBrush As New SolidBrush(currentColor) ' Updated fill brush for this row
            Dim rowTopLeft As New Point(Position.X, Position.Y + i * triangleHeight)
            DrawRow(g, rowTopLeft, triangleBase, triangleHeight, rectWidth, Verdicts(i), myPen, myFillBrush, myTextBrush, myFont)
        Next
    End Sub

    Private Sub DrawRow(ByRef g As Graphics, topLeft As Point, triangleBase As Integer, triangleHeight As Integer, rectWidth As Integer, verdict As String, myPen As Pen, myFillBrush As SolidBrush, myTextBrush As SolidBrush, myFont As Font)
        Dim myStringFormat As New StringFormat() With {
            .Alignment = StringAlignment.Far,
            .LineAlignment = StringAlignment.Center
        }

        ' Points for the triangle
        Dim trianglePoints() As Point = {
            New Point(topLeft.X + triangleBase, topLeft.Y),
            New Point(topLeft.X, topLeft.Y + triangleHeight / 2),
            New Point(topLeft.X + triangleBase, topLeft.Y + triangleHeight)
        }

        ' Draw triangle
        g.FillPolygon(myFillBrush, trianglePoints)

        ' Points for rectangle to the right of the triangle
        Dim rectPoints() As Point = {
            New Point(topLeft.X + triangleBase, topLeft.Y),
            New Point(topLeft.X + triangleBase + rectWidth, topLeft.Y),
            New Point(topLeft.X + triangleBase + rectWidth, topLeft.Y + triangleHeight),
            New Point(topLeft.X + triangleBase, topLeft.Y + triangleHeight)
        }

        ' Draw rectangle
        g.FillPolygon(myFillBrush, rectPoints)

        ' Outline for the combined shape (triangle + rectangle)
        Dim combinedPoints() As Point = {
            trianglePoints(0),
            trianglePoints(1),
            trianglePoints(2),
            rectPoints(2),
            rectPoints(1),
            rectPoints(0)
        }
        g.DrawPolygon(myPen, combinedPoints)

        ' Draw the label
        Dim labelPosition As New PointF(topLeft.X + triangleBase + rectWidth - 5, topLeft.Y + triangleHeight / 2)
        g.DrawString(verdict, myFont, myTextBrush, labelPosition, myStringFormat)
    End Sub

End Class



Imports System.IO

''' <summary>
''' Geen constructor nodig
''' </summary>
''' <remarks></remarks>
Public Class clsLog
    Friend Errors As IList(Of String) = New List(Of String)
    Friend Warnings As IList(Of String) = New List(Of String)
    Friend Messages As IList(Of String) = New List(Of String)
    Friend Header As String
    Public CmdArgs As IList(Of String) = New List(Of String)

    Friend DiagnosticsFilePath As String

    ' Het event wat afgevuurd moet worden als er een nieuwe message is
    Public Event ShowMessage(ByVal sender As Object, ByVal e As MessageEventArgs)

    ' Hier wordt het event daadwerkelijk in de lucht getrapt
    Public Overridable Sub OnStart(ByVal e As MessageEventArgs)
        RaiseEvent ShowMessage(Me, e)
    End Sub

    Public Sub New()

    End Sub

    Public Sub SetDiagnosticsPath(myDiagnosticsFilePath As String)
        DiagnosticsFilePath = myDiagnosticsFilePath
        If System.IO.File.Exists(DiagnosticsFilePath) Then System.IO.File.Delete(DiagnosticsFilePath)
        Using dialogWriter As New StreamWriter(DiagnosticsFilePath)
            dialogWriter.WriteLine("DIAGNOSTICS FILE, CREATED ON: " & String.Format(Now, "yyyy-MM-dd HH:mm:ss"))
        End Using
    End Sub

    Public Sub WriteHeader(myHeader As String)
        Header = myHeader
    End Sub

    Public Function CountAll() As Integer
        Return Errors.Count + Warnings.Count + Messages.Count
    End Function

    Public Function GetLastError() As String
        If Errors.Count > 0 Then
            Return Errors(Errors.Count - 1)
        Else
            Return ""
        End If
    End Function

    Public Function CountErrors() As Integer
        Return Errors.Count
    End Function

    Public Sub ShowErrors()
        Dim myStr As String = ""
        For i = 0 To Errors.Count - 1
            myStr &= Errors(i) & vbCrLf
        Next
        MsgBox(myStr)
    End Sub

    Public Sub ShowAll()
        Dim i As Long
        Dim myStr As String = Header & vbCrLf & vbCrLf
        myStr &= "ERRORS:" & vbCrLf
        For i = 0 To Errors.Count - 1
            myStr &= Errors(i) & vbCrLf
        Next
        myStr &= vbCrLf
        myStr &= "WARNINGS:" & vbCrLf
        For i = 0 To Warnings.Count - 1
            myStr &= Warnings(i) & vbCrLf
        Next
        myStr &= vbCrLf
        myStr &= "MESSAGES:" & vbCrLf
        For i = 0 To Messages.Count - 1
            myStr &= Messages(i) & vbCrLf
        Next
        MsgBox(myStr)
    End Sub

    Public Function GetErrors() As List(Of String) 'of string
        Return Errors
    End Function

    Public Sub Clear()
        Errors = New List(Of String)
        Warnings = New List(Of String)
        Messages = New List(Of String)
        CmdArgs = New List(Of String)
    End Sub

    Public Sub write(ByVal myPath As String, ByVal Show As Boolean)

        Using myWriter = New System.IO.StreamWriter(myPath)
            myWriter.WriteLine(Header & vbCrLf & vbCrLf)
            myWriter.WriteLine("ERRORS:")
            For Each myError As String In Errors
                myWriter.WriteLine(myError)
            Next
            myWriter.WriteLine("")
            myWriter.WriteLine("WARNINGS:")
            For Each myWarning As String In Warnings
                myWriter.WriteLine(myWarning)
            Next
            myWriter.WriteLine("")
            myWriter.WriteLine("MESSAGES:")
            For Each myMessage As String In Messages
                myWriter.WriteLine(myMessage)
            Next
        End Using
        If Show Then System.Diagnostics.Process.Start("notepad.exe", myPath)
    End Sub

    Public Sub writeToConsole(WriteErrors As Boolean, WriteWarnings As Boolean, WriteMessages As Boolean)
        If WriteErrors Then
            Console.WriteLine("ERRORS:")
            For Each myError As String In Errors
                Console.WriteLine(myError)
            Next
        End If
        Console.WriteLine("")
        If WriteWarnings Then
            Console.WriteLine("")
            Console.WriteLine("WARNINGS:")
            For Each myWarning As String In Warnings
                Console.WriteLine(myWarning)
            Next
        End If
        Console.WriteLine("")
        If WriteMessages Then
            Console.WriteLine("MESSAGES:")
            For Each myMessage As String In Messages
                Console.WriteLine(myMessage)
            Next
        End If
    End Sub

    Public Sub AddError(ByVal myMsg As String)
        Call Errors.Add(myMsg)
        Call WriteToDiagnosticsFile(myMsg)
        'RaiseEvent
        'Me.OnStart(New MessageEventArgs(myMsg, MessageEventArgs.MessageTypes.Error))
    End Sub

    Public Sub AddWarning(ByVal myMsg As String)
        Call Warnings.Add(myMsg)
        Call WriteToDiagnosticsFile(myMsg)
        'Me.OnStart(New MessageEventArgs(myMsg, MessageEventArgs.MessageTypes.Warning))
    End Sub

    Public Sub AddMessage(ByVal myMsg As String)
        Call Messages.Add(myMsg)
        Call WriteToDiagnosticsFile(myMsg)
        'Me.OnStart(New MessageEventArgs(myMsg, MessageEventArgs.MessageTypes.Message))
    End Sub

    ''' <summary>
    ''' Raises the message event with the debug message
    ''' </summary>
    ''' <param name="msg">The message</param>
    ''' <remarks>Paul Meems, 8 Jube 2012</remarks>
    Friend Sub AddDebugMessage(ByVal msg As String)
        Me.OnStart(New MessageEventArgs(msg, MessageEventArgs.MessageTypes.Debug))
    End Sub

    Public Function WriteToDiagnosticsFile(myMessage As String) As Boolean
        If Not DiagnosticsFilePath Is Nothing AndAlso System.IO.File.Exists(DiagnosticsFilePath) Then
            Using dialogWriter As New StreamWriter(DiagnosticsFilePath, True)
                dialogWriter.WriteLine(myMessage)
            End Using
        End If
    End Function
    Friend Function GetMessages() As List(Of String)
        Return Messages
    End Function

End Class

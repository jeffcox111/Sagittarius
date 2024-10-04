Public Class MessageHud

    Private Messages As New Collection
    Public LinesToDisplay As Integer = 8


    Public Structure TextMessage
        Public Text As String
        Public textColor As Color

    End Structure

    Public Sub AddMessage(ByVal text As String)
        'add message to queue
        Dim t As New TextMessage
        t.Text = text
        t.textColor = System.Drawing.Color.Red
        Messages.Add(t)

        'keep queue at 100 max messages
        If Messages.Count > 100 Then
            Messages.Remove(1)
        End If

    End Sub
    Public Sub AddMessage(ByVal text As String, ByVal col As Color)
        'add message to queue
        Dim t As New TextMessage
        t.Text = text
        t.textColor = col
        Messages.Add(t)

        'keep queue at 100 max messages
        If Messages.Count > 100 Then
            Messages.Remove(1)
        End If
    End Sub
    Public Sub DrawMessageHud()
        On Error GoTo errhandler
        'put the hud on the screen
        Dim i As Integer
        Dim offset As Integer = 10 * LinesToDisplay
        Dim LineHeight = 14
        For i = 0 To (LinesToDisplay - 1)

            TextFont.DrawText(Nothing, Messages(Messages.Count - i).Text, New Rectangle(300, offset, 300, LineHeight), Microsoft.DirectX.Direct3D.DrawTextFormat.Left, Messages(Messages.Count - i).textColor)
            'TextFont.DrawText(Nothing, Messages(Messages.Count - i).text, 300, offset, System.Drawing.Rectangle)

            offset -= 10
        Next

errhandler:
        Exit Sub

    End Sub
End Class

Public Class InputBoxHud

    Public InputText As String
    Public Prompt As String = "> "

    Public Sub DrawInputBox()

        TextFont.DrawText(Nothing, Prompt & InputText, New Rectangle(300, 300, 400, 20), Microsoft.DirectX.Direct3D.DrawTextFormat.Left, Color.Red)

    End Sub

    Public Sub AddText(ByVal text As String)
         InputText += text

    End Sub

    Public Sub BackSpace()

        Try
            InputText = InputText.Substring(0, InputText.Length - 1)

        Catch ex As Exception
            Exit Sub

        End Try
    End Sub

    Public Sub ClearText()
        InputText = ""

    End Sub

    Public Sub ProcessMessage()

        InputText = LCase(InputText)

        If InputText.StartsWith("unload models") Then
            
            GraphicLoader.ClearModels()


        ElseIf InputText.StartsWith("unload sprites") Then
            GraphicLoader.ClearSprites()

        ElseIf InputText.StartsWith("load models") Then
            Try

                Dim words() As String
                words = Split(InputText, " ")

                Dim filename As String
                filename = words(2)

                GraphicLoader.AddModels(filename)

            Catch
                HudMessager.AddMessage("Error loading models from file.")

            End Try

        ElseIf InputText.StartsWith("unload model") Then
            Try
                Dim words() As String
                words = Split(InputText, " ")

                Dim modelname As String
                modelname = words(2)

                ModelsManager.models.Remove(modelname)

            Catch ex As Exception
                HudMessager.AddMessage("Unable to unload model.")

            End Try
        ElseIf InputText.StartsWith("exit") Then
            End

        ElseIf InputText.StartsWith("enable behavior") Then
            Try
                Dim words() As String
                words = Split(InputText, " ")

                Dim modelname, behaviorname As String

                ModelsManager.models(modelname).behaviors(behaviorname).continuous = True

                HudMessager.AddMessage("Behavior enabled.")
            Catch ex As Exception

                HudMessager.AddMessage("Couldn't enable behavior.")
            End Try
        ElseIf InputText.StartsWith("fillmode") Then
            Try
                Dim words() As String
                words = Split(InputText, " ")
                Dim fm As Microsoft.DirectX.Direct3D.FillMode

                If words(1) = "solid" Then
                    fm = Microsoft.DirectX.Direct3D.FillMode.Solid
                ElseIf words(1) = "wire" Then
                    fm = Microsoft.DirectX.Direct3D.FillMode.WireFrame
                ElseIf words(1) = "point" Then
                    fm = Microsoft.DirectX.Direct3D.FillMode.Point
                Else
                    HudMessager.AddMessage("Unknown fillmode")
                End If

                Environment.ChangeFillMode(fm)
            Catch ex As Exception

            End Try

        ElseIf InputText.StartsWith("skybox") Then
            Try
                Dim words() As String
                words = Split(InputText, " ")
                
                If words(1) = "on" Then
                    Environment.SkyboxEnable(True)
                ElseIf words(1) = "off" Then
                    Environment.SkyboxEnable(False)
                End If
            Catch ex As Exception

            End Try

        Else
            HudMessager.AddMessage("Unknown command.")

        End If


        'reset inputtext
        InputText = ""

    End Sub
End Class

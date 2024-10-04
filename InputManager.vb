Imports Microsoft.DirectX
Imports Microsoft.DirectX.DirectInput
Imports System.Threading


Public Class InputManager

    Public InputThread As System.Threading.Thread

    Private InputDevice As New device(Microsoft.DirectX.DirectInput.SystemGuid.Keyboard)

    Private GameRunning As Boolean = True



    Public Sub New()

        InputDevice.Acquire()

        InputThread = New System.Threading.Thread(AddressOf InputLoop)
        InputThread.Start()

    End Sub

    Public Sub InputLoop()
        While GameRunning
            'receive input
            Dim key As Microsoft.DirectX.DirectInput.Key

            Dim currentkeys() As Microsoft.DirectX.DirectInput.Key = InputDevice.GetPressedKeys()
            For Each key In currentkeys
                System.Windows.Forms.Application.DoEvents()

                'process key input from user
                Select Case CurrentEngineState
                    Case mdlGlobals.EngineState.Running3D
                        'process key input from user
                        Select Case key
                            Case key.W 'move forward
                                'Dim int As Boolean
                                'Dim info() As IntersectInformation
                                'Dim m As New Model
                                'm = ModelsManager.models("terrain")

                                'int = m.mesh.Intersect(New Vector3(c.xPos, c.yPos, c.zPos), New Vector3(c.xView, c.yView, c.zView), info)
                                'If int = True Then
                                '    HudMessager.AddMessage("hit ground")
                                'End If
                                c.MoveCam(c.Forward * IncreaseSpeed)

                            Case key.S 'move backward

                                c.MoveCam(c.Backward * IncreaseSpeed)

                            Case key.A 'strafe left
                                c.StrafeCam(c.StrafeLeft * IncreaseSpeed)

                            Case key.D 'strafe right
                                c.StrafeCam(c.StrafeRight * IncreaseSpeed)

                            Case key.E 'zoom in

                                If Zoomed = False Then
                                    FieldOfViewY /= ZoomLevel
                                    Zoomed = True
                                End If



                            Case key.H

                                GraphicLoader.CopyModel("slayer", "bubba")
                                ModelsManager.models("bubba").xpos = 12
                                ModelsManager.models("bubba").ypos = 30
                                ModelsManager.models("bubba").zpos = 0
                                ModelsManager.models("bubba").yview = 30
                                ModelsManager.models("bubba").xview = 12

                                GraphicLoader.CopyModel("slayer", "bubba2")
                                With ModelsManager.models("bubba2")
                                    .xpos = -12
                                    .ypos = 30
                                    .zpos = 0
                                    .yview = 30
                                    .xview = -12
                                End With

                            Case key.Z 'change zoom level

                                Select Case ZoomLevel
                                    Case 2
                                        ZoomLevel = 5
                                        If Zoomed = True Then FieldOfViewY = Math.PI / 4 / ZoomLevel
                                        Exit Select
                                    Case 5
                                        ZoomLevel = 10
                                        If Zoomed = True Then FieldOfViewY = Math.PI / 4 / ZoomLevel
                                        Exit Select
                                    Case 10
                                        ZoomLevel = 20
                                        If Zoomed = True Then FieldOfViewY = Math.PI / 4 / ZoomLevel
                                        Exit Select
                                    Case 20
                                        ZoomLevel = 2
                                        If Zoomed = True Then FieldOfViewY = Math.PI / 4 / ZoomLevel
                                        Exit Select
                                End Select 'zoomlevel
                            Case key.B
                                Dim b As Model.ModelBehaviorEventArgs
                                b.Name = "rotate"
                                b.DynamicArgs = "0.09"
                                ModelsManager.models(1).BeginBehavior(b)

                            Case key.V
                                Dim b As Model.ModelBehaviorEventArgs
                                b.Name = "rotate"
                                b.DynamicArgs = "-0.09"
                                ModelsManager.models(1).BeginBehavior(b)

                            Case key.R
                                Dim b As Model.ModelBehaviorEventArgs
                                b.Name = "bank_up"
                                b.DynamicArgs = "0.09"
                                ModelsManager.models(1).BeginBehavior(b)


                            Case key.C
                                Dim b As Model.ModelBehaviorEventArgs
                                b.Name = "bank_up"
                                b.DynamicArgs = "-0.09"
                                ModelsManager.models(1).BeginBehavior(b)

                            Case key.Left
                                Dim b As Model.ModelBehaviorEventArgs
                                b.Name = "yaw"
                                b.DynamicArgs = "-0.09"
                                ModelsManager.models(1).BeginBehavior(b)

                            Case key.Right
                                Dim b As Model.ModelBehaviorEventArgs
                                b.Name = "yaw"
                                b.DynamicArgs = "0.09"
                                ModelsManager.models(1).BeginBehavior(b)


                            Case key.N 'cycle to next track in mp3 list
                                SoundTrackSystem.CycleTrack()

                            Case key.M 'toggle music
                                If SoundTrackSystem.PlaySoundTrack = True Then
                                    SoundTrackSystem.StopSoundTrack()
                                Else
                                    SoundTrackSystem.BeginSoundTrack()
                                End If

                            Case key.Escape 'exit the application

                                End

                            Case key.K
                                HudMessager.AddMessage("Current Position:", System.Drawing.Color.Pink)
                                HudMessager.AddMessage(c.xPos.ToString() & ", " & c.yPos.ToString() & ", " & c.zPos.ToString())

                            Case key.L
                                'Dim b As New ModelBehavior
                                'b = ModelsManager.GetBehavior("slayer", "move_forward")
                                'b.Continuous = True
                                ModelsManager.models("slayer").behaviors("move_forward").continuous = True

                            Case key.O
                                ModelsManager.models(1).behaviors(1).continuous = False

                            Case key.D1
                                ModelsManager.models("cobra_blade").behaviors("propellor_spin").continuous = True
                                HudMessager.AddMessage("Props spinning")
                            Case key.D2
                                ModelsManager.models("cobra").behaviors("chopper_lift").continuous = True
                                ModelsManager.models("cobra_blade").behaviors("chopper_lift").continuous = True
                                ModelsManager.models("cobra").behaviors("chopper_lower").continuous = False
                                ModelsManager.models("cobra_blade").behaviors("chopper_lower").continuous = False

                                HudMessager.AddMessage("Lifting off")

                            Case key.D3
                                ModelsManager.models("cobra").behaviors("chopper_lift").continuous = False
                                ModelsManager.models("cobra_blade").behaviors("chopper_lift").continuous = False
                                ModelsManager.models("cobra").behaviors("chopper_lower").continuous = False
                                ModelsManager.models("cobra_blade").behaviors("chopper_lower").continuous = False

                                HudMessager.AddMessage("Maintaining elevation")

                            Case key.D4
                                ModelsManager.models("cobra").behaviors("chopper_lower").continuous = True
                                ModelsManager.models("cobra_blade").behaviors("chopper_lower").continuous = True

                                ModelsManager.models("cobra").behaviors("chopper_lift").continuous = False
                                ModelsManager.models("cobra_blade").behaviors("chopper_lift").continuous = False

                                HudMessager.AddMessage("Lowering elevation")
                            Case key.X
                                CurrentEngineState = mdlGlobals.EngineState.DisplayingInputBox

                            Case key.Space
                                CurrentEngineState = mdlGlobals.EngineState.RunningGUI


                            Case key.Z
                                GraphicLoader.CopyModel("slayer", "bubba")
                                ModelsManager.models("bubba").xpos = 5
                                ModelsManager.models("bubba").ypos = 30
                                ModelsManager.models("bubba").zpos = 0

                            Case key.Return

                                ChangeWindowMode()

                        End Select 'key.code


                        Exit Select 'get user input




                    Case mdlGlobals.EngineState.DisplayingInputBox
                        If key <> key.Return And key <> key.Back And key <> key.Escape And key <> key.Period And key <> key.LeftShift And key <> key.LeftShift Then

                            If key = key.Space Then
                                InputBoxMessager.AddText(" ")
                            Else
                                InputBoxMessager.AddText(key.ToString())
                            End If

                        ElseIf key = key.Return Then

                            'process message
                            InputBoxMessager.ProcessMessage()
                            CurrentEngineState = mdlGlobals.EngineState.Running3D

                        ElseIf key = key.Back Then
                            InputBoxMessager.BackSpace()

                        ElseIf key = key.Period Then
                            InputBoxMessager.InputText += "."

                        ElseIf key = key.Escape Then
                            End

                        End If

                    Case mdlGlobals.EngineState.RunningGUI
                        If key <> key.Return And key <> key.Back And key <> key.Escape And key <> key.Period And key <> key.LeftShift And key <> key.LeftShift Then

                            If key = key.Space Then
                                InputBoxMessager.AddText(" ")
                            Else
                                InputBoxMessager.AddText(key)
                            End If

                        ElseIf key = key.Return Then

                            'process message
                            InputBoxMessager.ProcessMessage()
                            CurrentEngineState = mdlGlobals.EngineState.Running3D

                        ElseIf key = key.Back Then
                            InputBoxMessager.BackSpace()

                        ElseIf key = key.Period Then
                            InputBoxMessager.InputText += "."

                        ElseIf key = key.Escape Then
                            End

                        End If


                End Select

            Next



        End While
    End Sub

    Public Sub Dispose()
        GameRunning = False
        InputThread.Abort()
        InputThread = Nothing

    End Sub
End Class

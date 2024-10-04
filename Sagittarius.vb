Imports System
Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Data
Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D

Public Class Sagittarius

    Inherits System.Windows.Forms.Form


    Private components As System.ComponentModel.IContainer


    '/ <summary>
    '/ Required designer variable.
    '/ </summary>
    Private angle As Single = 0.0F

    Shared Sub Main()

        Try
            ' Show our form and initialize our graphics engine
            frm.Show()
            frm.InitializeGraphics()
            Application.Run(frm)

        Finally
            frm.Dispose()
        End Try
    End Sub 'Main
    Public Sub New()
        '
        ' Required for Windows Form Designer support
        '
        InitializeComponent()

        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.Opaque, True)
    End Sub 'New

    Public Sub InitializeGraphics()

        Dim current As Format = Manager.Adapters(0).CurrentDisplayMode.Format
        'Dim current As Format = Format.X8B8G8R8

        ' Set our presentation parameters
        Dim presentParams As New PresentParameters

        presentParams.Windowed = False
        presentParams.SwapEffect = SwapEffect.Discard
        presentParams.BackBufferFormat = current
        presentParams.BackBufferCount = 2
        presentParams.BackBufferWidth = 1024
        presentParams.BackBufferHeight = 768
        presentParams.AutoDepthStencilFormat = DepthFormat.D24X8

        presentParams.EnableAutoDepthStencil = True
       

        'Create our device
        device = New device(0, DeviceType.Hardware, Me, CreateFlags.HardwareVertexProcessing, presentParams)

        'make some fog
        device.RenderState.FogVertexMode = FogMode.Linear
        device.RenderState.FogColor = Color.Honeydew

        device.RenderState.FogEnable = True
        device.RenderState.FogDensity = 10
        device.RenderState.FogStart = 100
        device.RenderState.FogEnd = 2000

        ' device.RenderState.MultiSampleAntiAlias = True


        ' try to set up a texture minify filter, pick anisotropic first
        If device.DeviceCaps.TextureFilterCaps.SupportsMinifyAnisotropic Then
            device.SamplerState(0).MinFilter = TextureFilter.Anisotropic
        ElseIf device.DeviceCaps.TextureFilterCaps.SupportsMinifyLinear Then
            device.SamplerState(0).MinFilter = TextureFilter.Linear
        End If

        ' do the same thing for magnify filter
        If device.DeviceCaps.TextureFilterCaps.SupportsMagnifyAnisotropic Then
            device.SamplerState(0).MagFilter = TextureFilter.Anisotropic
        ElseIf device.DeviceCaps.TextureFilterCaps.SupportsMagnifyLinear Then
            device.SamplerState(0).MagFilter = TextureFilter.Linear
        End If

        If device.DeviceCaps.TextureFilterCaps.SupportsMipMapLinear Then
            device.SamplerState(0).MipFilter = TextureFilter.Anisotropic
        End If

        'init model manager
        ModelsManager.d3dDevice = device

        'init sprite manager
        spManager.d3dDevice = device

        ' Load our meshes, sprites, and behaviors
        GraphicLoader.AddModels("LoadModels.vbs")
        GraphicLoader.AddSprites()
        GraphicLoader.AddBehaviors()

        'grab a font
        Dim localfont As New System.Drawing.Font("Arial", 8, FontStyle.Bold)
        TextFont = New Direct3D.Font(device, localfont)

        'start the music
        SoundTrackSystem.BuildSoundTrack()
        SoundTrackSystem.BeginSoundTrack()
        frmLoad.Close()

    End Sub 'InitializeGraphics



    Private Sub SetupCamera()
        'initialize perspective
        device.RenderState.CullMode = Cull.None



        'device.RenderState.AntiAliasedLineEnable = True
        If Not CurrentEngineState = mdlGlobals.EngineState.RunningGUI Then
            device.Transform.Projection = Matrix.PerspectiveFovLH(FieldOfViewY, AspectRatio, clipNear, clipFar)


            'find middle of screen
            Dim w As Long = Me.Width >> 1
            Dim h As Long = Me.Height >> 1
            Dim l As Long

            Dim mousePos As POINTAPI
            c.RotateByMouse(Control.MousePosition.X, Control.MousePosition.Y, Me.Left + w, Me.Top + h)

            'keep the cursor in the middle of the screen
            SetCursorPos(Me.Left + w, Me.Top + h)


            'look
            device.Transform.View = Matrix.LookAtLH(New Vector3(c.xPos, c.yPos, c.zPos), New Vector3(c.xView, c.yView, c.zView), New Vector3(0, 1, 0))

        End If

        'device.RenderState.Ambient = Color.DarkBlue;
        device.Lights(0).Type = LightType.Directional
        device.Lights(0).Diffuse = Color.White
        device.Lights(0).Direction = New Vector3(0, -1, -1)
        device.Lights(0).Commit()
        device.Lights(0).Enabled = True

        With device.Lights(1)
            .Type = LightType.Directional
            .Diffuse = Color.White
            .Direction = New Vector3(-1, 0, 1)
            .Commit()
            .Enabled = True
        End With

        With device.Lights(2)
            .Type = LightType.Directional
            .Diffuse = Color.White
            .Direction = New Vector3(1, 1, 0)
            .Commit()
            .Enabled = True

        End With

        With device.Lights(3)
            .Type = LightType.Spot
            .Diffuse = Color.Red
            .Direction = New Vector3(0, -1, 0)
            .Range = 1000
            .InnerConeAngle = 0.5
            .OuterConeAngle = 1
            .Falloff = 1
            .Commit()
            .Enabled = True
        End With




    End Sub 'SetupCamera


    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)

        ElapsedTime = Utility.Timer(DirectXTimer.GetElapsedTime)

        Select Case CurrentEngineState
            Case mdlGlobals.EngineState.Running3D
                'get ready for new scene
                device.Clear(ClearFlags.Target Or ClearFlags.ZBuffer, Color.Black, 1.0F, 0)
                SetupCamera()

                device.BeginScene()

                'draw skybox
                Environment.DrawSkyBox()

                'draw models
                ModelsManager.DrawModels()

                'draw sprites
                spManager.DrawSprites()

                'draw text hud
                DrawHud()


                'end the scene and render
                device.EndScene()

                device.Present()
                Me.Invalidate()

                Exit Select

            Case mdlGlobals.EngineState.DisplayingInputBox
                'get ready for new scene
                device.Clear(ClearFlags.Target Or ClearFlags.ZBuffer, Color.Black, 1.0F, 0)
                SetupCamera()

                'device.EndScene()
                device.BeginScene()

                'draw models
                ModelsManager.DrawModels()

                'draw sprites
                spManager.DrawSprites()

                'draw text hud
                DrawHud()

                'draw inputboxhud
                InputBoxMessager.DrawInputBox()

                'end the scene and render
                device.EndScene()

                device.Present()
                Me.Invalidate()

                Exit Select

            Case mdlGlobals.EngineState.RunningGUI
                'display menu system

                'get ready for new scene
                device.Clear(ClearFlags.Target Or ClearFlags.ZBuffer, Color.Black, 1.0F, 0)
                SetupCamera()

                'device.EndScene()
                device.BeginScene()

                'draw models
                ModelsManager.DrawModels()

                'draw sprites
                spManager.DrawSprites()

                'draw text hud
                DrawHud()

                'draw inputboxhud
                InputBoxMessager.DrawInputBox()

                'end the scene and render
                device.EndScene()

                device.Present()
                Me.Invalidate()

                Exit Select

                Exit Select

        End Select


    End Sub 'OnPaint

    Public Sub DrawHud()
      
        HudMessager.DrawMessageHud()

    End Sub
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub 'Dispose

#Region "Windows Form Designer generated code"

    '/ <summary>
    '/ Required method for Designer support - do not modify
    '/ the contents of this method with the code editor.
    '/ </summary>
    'Public WithEvents Timer1 As System.Windows.Forms.Timer
    Private Sub InitializeComponent()
        '
        'Sagittarius
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(792, 573)
        Me.Cursor = System.Windows.Forms.Cursors.Cross
        Me.Name = "Sagittarius"
        Me.Text = "Sagittarius"

    End Sub 'InitializeComponent
#End Region




    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'set initial camera position
        c.xPos = 0
        c.yPos = 20
        c.zPos = 0
        c.xView = 0
        c.yView = 20
        c.zView = -10


        'use form's current size as camera's aspect ratio
        AspectRatio = Me.Width / Me.Height

    End Sub

    Protected Overrides Sub OnKeyDown(ByVal e As System.Windows.Forms.KeyEventArgs)

        Select Case CurrentEngineState
            Case mdlGlobals.EngineState.Running3D
                'process key input from user
                Select Case e.KeyCode
                    Case Keys.W 'move forward
                        'Dim int As Boolean
                        'Dim info() As IntersectInformation
                        'Dim m As New Model
                        'm = ModelsManager.models("terrain")

                        'int = m.mesh.Intersect(New Vector3(c.xPos, c.yPos, c.zPos), New Vector3(c.xView, c.yView, c.zView), info)
                        'If int = True Then
                        '    HudMessager.AddMessage("hit ground")
                        'End If
                        c.MoveCam(c.Forward * IncreaseSpeed)

                    Case Keys.S 'move backward

                        c.MoveCam(c.Backward * IncreaseSpeed)

                    Case Keys.A 'strafe left
                        c.StrafeCam(c.StrafeLeft * IncreaseSpeed)

                    Case Keys.D 'strafe right
                        c.StrafeCam(c.StrafeRight * IncreaseSpeed)

                    Case Keys.E 'zoom in

                        If Zoomed = False Then
                            FieldOfViewY /= ZoomLevel
                            Zoomed = True
                        End If



                    Case Keys.H

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

                    Case Keys.Z 'change zoom level

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
                    Case Keys.B
                        Dim b As Model.ModelBehaviorEventArgs
                        b.Name = "rotate"
                        b.DynamicArgs = "0.09"
                        ModelsManager.models(1).BeginBehavior(b)

                    Case Keys.V
                        Dim b As Model.ModelBehaviorEventArgs
                        b.Name = "rotate"
                        b.DynamicArgs = "-0.09"
                        ModelsManager.models(1).BeginBehavior(b)

                    Case Keys.R
                        Dim b As Model.ModelBehaviorEventArgs
                        b.Name = "bank_up"
                        b.DynamicArgs = "0.09"
                        ModelsManager.models(1).BeginBehavior(b)


                    Case Keys.C
                        Dim b As Model.ModelBehaviorEventArgs
                        b.Name = "bank_up"
                        b.DynamicArgs = "-0.09"
                        ModelsManager.models(1).BeginBehavior(b)

                    Case Keys.Left
                        Dim b As Model.ModelBehaviorEventArgs
                        b.Name = "yaw"
                        b.DynamicArgs = "-0.09"
                        ModelsManager.models(1).BeginBehavior(b)

                    Case Keys.Right
                        Dim b As Model.ModelBehaviorEventArgs
                        b.Name = "yaw"
                        b.DynamicArgs = "0.09"
                        ModelsManager.models(1).BeginBehavior(b)


                    Case Keys.N 'cycle to next track in mp3 list
                        SoundTrackSystem.CycleTrack()

                    Case Keys.M 'toggle music
                        If SoundTrackSystem.PlaySoundTrack = True Then
                            SoundTrackSystem.StopSoundTrack()
                        Else
                            SoundTrackSystem.BeginSoundTrack()
                        End If

                    Case Keys.Escape 'exit the application
                        Me.Close()

                    Case Keys.K
                        HudMessager.AddMessage("Current Position:", System.Drawing.Color.Pink)
                        HudMessager.AddMessage(c.xPos.ToString() & ", " & c.yPos.ToString() & ", " & c.zPos.ToString())

                    Case Keys.L
                        'Dim b As New ModelBehavior
                        'b = ModelsManager.GetBehavior("slayer", "move_forward")
                        'b.Continuous = True
                        ModelsManager.models("slayer").behaviors("move_forward").continuous = True

                    Case Keys.O
                        ModelsManager.models(1).behaviors(1).continuous = False

                    Case Keys.D1
                        ModelsManager.models("cobra_blade").behaviors("propellor_spin").continuous = True
                        HudMessager.AddMessage("Props spinning")
                    Case Keys.D2
                        ModelsManager.models("cobra").behaviors("chopper_lift").continuous = True
                        ModelsManager.models("cobra_blade").behaviors("chopper_lift").continuous = True
                        ModelsManager.models("cobra").behaviors("chopper_lower").continuous = False
                        ModelsManager.models("cobra_blade").behaviors("chopper_lower").continuous = False

                        HudMessager.AddMessage("Lifting off")

                    Case Keys.D3
                        ModelsManager.models("cobra").behaviors("chopper_lift").continuous = False
                        ModelsManager.models("cobra_blade").behaviors("chopper_lift").continuous = False
                        ModelsManager.models("cobra").behaviors("chopper_lower").continuous = False
                        ModelsManager.models("cobra_blade").behaviors("chopper_lower").continuous = False

                        HudMessager.AddMessage("Maintaining elevation")

                    Case Keys.D4
                        ModelsManager.models("cobra").behaviors("chopper_lower").continuous = True
                        ModelsManager.models("cobra_blade").behaviors("chopper_lower").continuous = True

                        ModelsManager.models("cobra").behaviors("chopper_lift").continuous = False
                        ModelsManager.models("cobra_blade").behaviors("chopper_lift").continuous = False

                        HudMessager.AddMessage("Lowering elevation")
                    Case Keys.X
                        CurrentEngineState = mdlGlobals.EngineState.DisplayingInputBox

                    Case Keys.Space
                        CurrentEngineState = mdlGlobals.EngineState.RunningGUI


                    Case Keys.Z
                        GraphicLoader.CopyModel("slayer", "bubba")
                        ModelsManager.models("bubba").xpos = 5
                        ModelsManager.models("bubba").ypos = 30
                        ModelsManager.models("bubba").zpos = 0

                    Case Keys.Enter

                        ChangeWindowMode()



                End Select 'keys.code


                Exit Select 'get user input




            Case mdlGlobals.EngineState.DisplayingInputBox
                If e.KeyCode <> Keys.Enter And e.KeyCode <> Keys.Back And e.KeyCode <> Keys.Escape And e.KeyCode <> Keys.OemPeriod And e.KeyCode <> Keys.Shift And e.KeyCode <> Keys.ShiftKey Then

                    If e.KeyCode = Keys.Space Then
                        InputBoxMessager.AddText(" ")
                    Else
                        InputBoxMessager.AddText(e.KeyData.ToString)
                    End If

                ElseIf e.KeyCode = Keys.Enter Then

                    'process message
                    InputBoxMessager.ProcessMessage()
                    CurrentEngineState = mdlGlobals.EngineState.Running3D

                ElseIf e.KeyCode = Keys.Back Then
                    InputBoxMessager.BackSpace()

                ElseIf e.KeyCode = Keys.OemPeriod Then
                    InputBoxMessager.InputText += "."

                ElseIf e.KeyCode = Keys.Escape Then
                    End

                End If

            Case mdlGlobals.EngineState.RunningGUI
                If e.KeyCode <> Keys.Enter And e.KeyCode <> Keys.Back And e.KeyCode <> Keys.Escape And e.KeyCode <> Keys.OemPeriod And e.KeyCode <> Keys.Shift And e.KeyCode <> Keys.ShiftKey Then

                    If e.KeyCode = Keys.Space Then
                        InputBoxMessager.AddText(" ")
                    Else
                        InputBoxMessager.AddText(e.KeyData.ToString)
                    End If

                ElseIf e.KeyCode = Keys.Enter Then

                    'process message
                    InputBoxMessager.ProcessMessage()
                    CurrentEngineState = mdlGlobals.EngineState.Running3D

                ElseIf e.KeyCode = Keys.Back Then
                    InputBoxMessager.BackSpace()

                ElseIf e.KeyCode = Keys.OemPeriod Then
                    InputBoxMessager.InputText += "."

                ElseIf e.KeyCode = Keys.Escape Then
                    End

                End If


        End Select

    End Sub

    Private Sub Form1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
        'increase movement speed
        If e.Button = MouseButtons.Right Then
            IncreaseSpeed = 6
        End If
    End Sub

    Private Sub Form1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseUp
        'revert to old movement speed
        If e.Button = MouseButtons.Right Then
            IncreaseSpeed = 1
        End If
    End Sub

    Protected Overrides Sub OnKeyUp(ByVal e As System.Windows.Forms.KeyEventArgs)
        'process user input on key release
        Select Case e.KeyCode

            Case Keys.E 'zoom out
                FieldOfViewY = Math.PI / 4
                Zoomed = False
        End Select
    End Sub

    Public Sub ShowLoading()
        frmLoad.Show()

        frmLoad.ProgressBar1.Minimum = 0
        frmLoad.ProgressBar1.Maximum = 11

    End Sub

    Protected Overrides Sub OnClosing(ByVal e As System.ComponentModel.CancelEventArgs)
        'KeboardInput.Dispose()

    End Sub

    Protected Overrides Sub OnClick(ByVal e As System.EventArgs)
        Dim x, y As Integer
        x = Control.MousePosition.X
        y = Control.MousePosition.Y

        Dim s As SpriteData
        For Each s In spManager.Sprites
            If x > s.xPos And x < (s.xPos + s.TextureSize.Width) And y > s.yPos And y < (s.yPos + s.TextureSize.Height) Then
                s.OnClick()

            End If
        Next
    End Sub

    Private Sub Sagittarius_ParentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.ParentChanged

    End Sub
End Class 'Form1
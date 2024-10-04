Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D
Imports Microsoft.DirectX.DirectSound
Imports Microsoft.DirectX.AudioVideoPlayback

Module mdlGlobals

    'API references and required structures
    Public Declare Function SetCursorPos Lib "user32" Alias "SetCursorPos" (ByVal x As Integer, ByVal y As Integer) As Long

    Public Structure POINTAPI
        Public x As Long
        Public y As Long
    End Structure


    'globals for game engine
    '3d device
    Public WithEvents device As Direct3D.Device = Nothing
    Public Windowed As Boolean = False

    'main window
    Public frm As New Sagittarius

    'graphic loader
    Public GraphicLoader As New LoadManager

    'models
    ' Public models As New Collection
    Public ModelsManager As New modelManager

    'Sprites
    Public spManager As New SpriteManager

    'Input management
    'Public KeboardInput As New InputManager

    'world level behaviors
    Public WorldBehaviors As New Collection

    'map arrangement
    Public MapOffset As Single = 7.7 '96.5 

    'camera properties
    Public c As New CCamera
    Public clipNear As Single = 0.1F
    Public clipFar As Single = 10000.0F
    Public FieldOfViewY As Single = System.Convert.ToSingle(Math.PI) / 4
    Public AspectRatio As Single
    Public Zoomed As Boolean = False
    Public ZoomLevel As Single = 2
    Public IncreaseSpeed As Integer = 1

    'font generation
    Public TextFont As Direct3D.Font

    'Loading status window
    Public frmLoad As New frmLoading
    Public ModelsLoaded As Integer

    'Sound device & audio-video-playback
    Public SoundTrackCol As New Collection
    Public SoundTrack As IEnumerator

    'Sountrack
    Public SoundTrackSystem As New SoundTrackManager

    'Environment controller
    Public Environment As New SagEnvironment

    'MessageHud
    Public HudMessager As New MessageHud

    'InputBoxHud
    Public InputBoxMessager As New InputBoxHud

    'EngineState
    Public CurrentEngineState As EngineState = EngineState.Running3D

    'DirectX Timer
    Public Utility As DXUtil
    Public ElapsedTime As Single



    Public Enum EngineState
        Running3D = 0
        DisplayingInputBox = 1
        RunningGUI = 2
        'add more engine states here

    End Enum

    Public Sub ChangeWindowMode()
        Windowed = Not Windowed

        'If Windowed = False Then
        Dim current As Format = Manager.Adapters(0).CurrentDisplayMode.Format
        'Dim current As Format = Format.X8B8G8R8

        ' Set our presentation parameters
        Dim presentParams As New PresentParameters

        presentParams.Windowed = Windowed
        presentParams.SwapEffect = SwapEffect.Discard
        presentParams.BackBufferFormat = current
        presentParams.BackBufferCount = 2
        If Windowed = False Then
            presentParams.BackBufferWidth = 1024
            presentParams.BackBufferHeight = 768
        Else
            presentParams.BackBufferWidth = 640
            presentParams.BackBufferHeight = 480
        End If

        presentParams.AutoDepthStencilFormat = DepthFormat.D24X8

        presentParams.EnableAutoDepthStencil = True

        ModelsManager.d3dDevice.Reset(presentParams)

        If Windowed = False Then
            frm.SetBounds(1, 1, 1024, 768)
        Else
            frm.SetBounds(100, 100, 640, 480)
        End If


        'make some fog
        device.RenderState.FogVertexMode = FogMode.Linear
        device.RenderState.FogColor = Color.Black
        device.RenderState.FogEnable = True
        device.RenderState.FogDensity = 10
        device.RenderState.FogStart = 100
        device.RenderState.FogEnd = 1000

        device.RenderState.MultiSampleAntiAlias = True


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

      
    End Sub

    
End Module

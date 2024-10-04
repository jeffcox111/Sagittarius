Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D

Public Class SpriteManager
    Public Sprites As New Collection
    Public d3dDevice As Direct3D.Device

    Private Center = New Vector3(0, 0, 0)

   

    Public Sub DrawSprites()
        Dim s As SpriteData
        Dim theSprite As New Sprite(d3dDevice)


        theSprite.Begin(SpriteFlags.AlphaBlend)

        For Each s In Sprites
            s.LoopBehaviors()
            theSprite.Draw(s.SpriteTexture, s.TextureSize, Center, New Vector3(s.xPos, s.yPos, 0), s.BlendColor)

        Next

        theSprite.End()
        theSprite.Dispose()
        theSprite = Nothing


    End Sub
    Public Sub LoadSprite(ByVal Path As String, ByVal sName As String, ByVal x As Single, ByVal y As Single)

        Dim tex As Texture

        tex = TextureLoader.FromFile(d3dDevice, Path)


        Dim surf As Surface = tex.GetSurfaceLevel(0)
        Dim d As SurfaceDescription
        d = surf.Description

        Dim sData As New SpriteData
        sData.SpriteTexture = tex
        sData.TextureSize = New Rectangle(0, 0, d.Width, d.Height)
        sData.xPos = x
        sData.yPos = y
        sData.BlendColor = Color.White
        sData.Name = sName
        'sData.Center = New Vector3((x + d.Width) / 2, (y + d.Height) / 2, 0)

        Sprites.Add(sData, sName)


    End Sub

    Public Sub Reposition(ByVal sName As String, ByVal x As Integer, ByVal y As Integer)
        Dim s As New SpriteData
        Dim tmp As New SpriteData


        For Each s In Sprites
            If s.Name = sName Then
                tmp = s
            End If
        Next

        tmp.xPos = x
        tmp.yPos = y

        Sprites.Remove(sName)
        Sprites.Add(tmp, sName)

    End Sub

End Class

Public Class SpriteData
    Public Name As String

    Public SpriteTexture As Texture
    Public TextureSize As Rectangle

    Public xPos, yPos As Integer
    Public Width, Height As Integer


    Public Angle As Single

    Public BlendColor As Color

    'Public Behaviors() As Behavior
    Public Behaviors As New Collection
    Public ClickBehavior As New SpriteBehavior


    Public Structure SpriteBehaviorEventArgs
        Public Name As String
        Public Myself As SpriteData
        Public DynamicArgs As String

    End Structure

    Public Sub BeginBehavior(ByVal b As SpriteBehaviorEventArgs)

        Dim tmp As SpriteBehavior
        For Each tmp In Behaviors
            If tmp.Name = b.Name Then
                b.Myself = Me
                tmp.BeginBehavior(b)
            End If
        Next

    End Sub

    Public Sub LoopBehaviors()
        Dim tmpb As SpriteBehavior
        For Each tmpb In Behaviors
            If tmpb.Continuous = True Then
                Dim args As SpriteData.SpriteBehaviorEventArgs
                args.Myself = Me
                tmpb.BeginBehavior(args)

            End If
        Next
    End Sub

    Public Sub OnClick()
        Try
            Dim b As New SpriteBehaviorEventArgs
            b.Myself = Me

            ClickBehavior.BeginBehavior(b)
        Catch
            Exit Sub
        End Try

    End Sub


End Class

Public Class SpriteBehavior
    Public Name As String
    Public Continuous As Boolean = False

    Public Event Behavior(ByVal b As SpriteData.SpriteBehaviorEventArgs)


    Public Sub BeginBehavior(ByVal b As SpriteData.SpriteBehaviorEventArgs)
        RaiseEvent Behavior(b)
    End Sub

End Class


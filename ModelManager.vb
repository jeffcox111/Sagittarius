Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D

Public Class ModelManager
    Public models As New Collection
    Public d3dDevice As Direct3D.Device

    Public Sub DrawModels()
        ' Draw our Meshes
        Dim i As Integer
        For i = 1 To models.Count 'UBound(models)
            models(i).LoopBehaviors()
            'models(i).DrawMesh(device, angle / System.Convert.ToSingle(Math.PI), angle / System.Convert.ToSingle(Math.PI) * 2.0F, angle / System.Convert.ToSingle(Math.PI) / 4.0F, models(i).xview, models(i).yview, models(i).zview)
            models(i).DrawMesh(d3dDevice, models(i).mYaw, models(i).mPitch, models(i).mRoll, models(i).xview, models(i).yview, models(i).zview)

        Next

    End Sub
    Public Function GetModel(ByVal Name As String) As Model
        Dim m As New Model

        Try

        
        For Each m In models
            If m.Name = Name Then
                Return m
            Else

            End If
            Next

        Catch ex As Exception
            HudMessager.AddMessage("Could not find model.")
            End
        End Try
    End Function
    Public Function GetBehavior(ByVal ModelName As String, ByVal BehaviorName As String) As ModelBehavior

        Dim m As New Model
        m = ModelsManager.GetModel(ModelName)

        Dim b As New ModelBehavior

        Try


            For Each b In m.Behaviors
                If b.Name = BehaviorName Then
                    Return b
                Else

                End If
            Next

        Catch ex As Exception
            HudMessager.AddMessage("Could not find behavior.")
            End
        End Try
    End Function

    Public Function ClearModelData() As Model

        Dim m As New Model
        Return m

    End Function

End Class

Public Class Model
    'name
    Public Name As String

    'mesh object and texturing
    Public mesh As mesh = Nothing
    Public meshMaterials() As Material
    Public meshTextures() As Texture

    'location,orientation,etc
    Public xPos, yPos, zPos As Single
    Public xView, yView, zView As Single
    Public xUp, yUp, zUp As Single
    Public xStrafe, yStrafe, zStrafe As Single
    Public mYaw, mPitch, mRoll As Single
    Public CurrentRotationAngle As Single

    'physics stuff
    Public xVel, yVel, zVel As Single


    'scaling
    'THIS ISN'T BEING USED ...yet
    Public Scale As Single = 1
    
    'determine if the model is going to be visible
    Public Enabled As Boolean = True

    'bounding sphere for collision detection
    Public BoundingSphereRadius As Single

    'rendering fillmode
    Public Fillmode As Direct3D.FillMode

    'this is my file
    Public FileName As String


    'Public Behaviors() As Behavior
    Public Behaviors As New Collection

    Public Structure ModelBehaviorEventArgs
        Public Name As String
        Public Myself As Model
        Public DynamicArgs As String

    End Structure

    Public Sub New()
        xPos = 0
        yPos = 0
        zPos = 0
        xView = 0
        yView = 0
        zView = 0
        xUp = 0
        yUp = 1
        zUp = 0
        xStrafe = 0
        yStrafe = 0
        zStrafe = 0
        mYaw = 0
        mPitch = 0
        mRoll = 0

        Fillmode = Fillmode.Solid

    End Sub
    Public Sub New(ByVal xp As Single, ByVal yp As Single, ByVal zp As Single)
        xPos = xp
        yPos = yp
        zPos = zp
        xView = 0
        yView = 0
        zView = 0
        xUp = 0
        yUp = 1
        zUp = 0
        xStrafe = 0
        yStrafe = 0
        zStrafe = 0
        mYaw = 0
        mPitch = 0
        mRoll = 0
        Fillmode = Fillmode.Solid

    End Sub
    Public Sub BeginBehavior(ByVal b As ModelBehaviorEventArgs)

        Dim tmp As ModelBehavior
        For Each tmp In Behaviors
            If tmp.Name = b.Name Then
                b.Myself = Me
                tmp.BeginBehavior(b)
            End If
        Next

    End Sub
    Public Sub LoadMesh(ByVal file As String)
        Dim mtrl() As ExtendedMaterial
        Dim adj As GraphicsStream

        FileName = file

        Try
            ' Load our mesh
            mesh = mesh.FromFile(file, MeshFlags.Managed, device, adj, mtrl)

            ' If we have any materials, store them
            If Not (mtrl Is Nothing) AndAlso mtrl.Length > 0 Then
                meshMaterials = New Material(mtrl.Length) {}
                meshTextures = New Texture(mtrl.Length) {}

                ' Store each material and texture
                Dim i As Integer
                For i = 0 To mtrl.Length - 1
                    meshMaterials(i) = mtrl(i).Material3D
                    If Not (mtrl(i).TextureFilename Is Nothing) AndAlso mtrl(i).TextureFilename <> String.Empty Then
                        ' We have a texture, try to load it
                        meshTextures(i) = TextureLoader.FromFile(device, mtrl(i).TextureFilename)
                    End If
                Next i
            End If

            Dim tmp As Mesh = mesh.Clean(mesh, adj, adj)

            'tmp = mesh.Clean(mesh, adj, adj)
            mesh.Dispose()
            mesh = tmp

            Dim a() As Integer
            mesh = mesh.Simplify(mesh, a, 1, MeshFlags.SimplifyVertex)
            
        Catch


            'Throw New SystemException

        End Try
     

    End Sub 'LoadMesh
    Public Sub DrawMesh(ByVal device As device, ByVal yaw As Single, ByVal pitch As Single, ByVal roll As Single, ByVal x As Single, ByVal y As Single, ByVal z As Single)

        'check to see if model is enabled
        If Enabled = False Then Exit Sub

        device.RenderState.FillMode = Me.Fillmode

        Dim i As Integer
        For i = 0 To meshMaterials.Length - 1
            device.Material = meshMaterials(i)
            device.SetTexture(0, meshTextures(i))

            
            device.Transform.World = Matrix.Multiply(Matrix.RotationYawPitchRoll(mYaw, mPitch, mRoll), Matrix.Translation(xPos, yPos, zPos))
            mesh.DrawSubset(i)
        Next i

    End Sub 'DrawMesh
    Public Sub LoopBehaviors()
        Dim tmpb As ModelBehavior
        For Each tmpb In Behaviors
            If tmpb.Continuous = True Then
                Dim args As ModelBehaviorEventArgs
                args.Myself = Me
                tmpb.BeginBehavior(args)

            End If
        Next
    End Sub
    Public Sub MoveModel(ByVal direction As Single)
        Dim xLookDirection, yLookDirection, zLookDirection As Single
        xLookDirection = 0
        yLookDirection = 0
        zLookDirection = 0

        'The look direction is the view (where we are looking) minus the position (where we are).
        xLookDirection = xView - xPos
        yLookDirection = yView - yPos
        zLookDirection = zView - zPos

        'Normalize the direction.
        Dim dp As Single
        dp = 1 / Math.Sqrt(xLookDirection * xLookDirection + yLookDirection * yLookDirection + zLookDirection * zLookDirection)

        xLookDirection *= dp
        yLookDirection *= dp
        zLookDirection *= dp

        'Call UpdateCamera to move our camera in the direction we want.
        UpdateModel(xLookDirection, yLookDirection, zLookDirection, direction)

    End Sub
    Public Sub UpdateModel(ByVal xDir As Single, ByVal yDir As Single, ByVal zDir As Single, ByVal dir As Single)

        'Move the camera on the X and Z axis.
        xPos += xDir * dir
        zPos += zDir * dir
        yPos += yDir * dir
        'Move the view along with the position
        xView += xDir * dir
        zView += zDir * dir
        yView += yDir * dir
    End Sub
    Public Sub StrafeModel(ByVal direction As Single)
        CalculateStrafe()
        UpdateModel(xStrafe, yStrafe, zStrafe, direction)

    End Sub
    Public Sub CalculateStrafe()
        Dim xdir As Single = 0
        Dim ydir As Single = 0
        Dim zdir As Single = 0
        Dim xcross As Single = 0
        Dim ycross As Single = 0
        Dim zcross As Single = 0


        'First we will get the direction we are looking.
        xdir = xView - xPos
        ydir = yView - yPos
        zdir = zView - zPos

        'Normalize the direction.
        Dim dp As Single
        dp = System.Convert.ToSingle(1 / Math.Sqrt(xdir * xdir + ydir * ydir + zdir * zdir))


        xdir *= dp
        ydir *= dp
        zdir *= dp

        'Get the cross product of the direction we are looking and the up direction.
        xcross = (ydir * zUp) - (zdir * yUp)
        ycross = (zdir * xUp) - (xdir * zUp)
        zcross = (xdir * yUp) - (ydir * xUp)

        'Save our strafe (cross product) values in xStrafe, yStrafe, and zStrafe.
        xStrafe = xcross
        yStrafe = ycross
        zStrafe = zcross

    End Sub
    Public Sub RotateByAngle(ByVal mousePosX As Single, ByVal mousePosY As Single, ByVal midX As Integer, ByVal midY As Integer)
        Dim yDirection As Single = 0.0F
        Dim yRotation As Single = 0.0F

        'If the mouseX and mouseY are at the middle of the screen then we can't rotate the view.
        If mousePosX = midX And mousePosY = midY Then
            Exit Sub
        End If

        'Next we get the direction of each axis.  We divide by 1000 to get a smaller value back.
        yDirection = ((midX - mousePosX)) / 1000.0F
        yRotation = ((midY - mousePosY)) / 1000.0F


        Dim xAxis As Single = 0
        Dim yAxis As Single = 0
        Dim zAxis As Single = 0
        Dim xDir As Single = 0
        Dim yDir As Single = 0
        Dim zDir As Single = 0

        'Get the Direction of the view.
        xDir = xView - xPos
        yDir = yView - yPos
        zDir = zView - zPos

        'Get the cross product of the direction and the up.
        xAxis = (yDir * zUp) - (zDir * yUp)
        yAxis = (zDir * xUp) - (xDir * zUp)
        zAxis = (xDir * yUp) - (yDir * xUp)

        'normalize it
        Dim len As Single
        len = 1 / Math.Sqrt(xAxis * xAxis + yAxis * yAxis + zAxis * zAxis)
        xAxis *= len
        yAxis *= len
        zAxis *= len

        'rotate the model
        RotateModel(yRotation, xAxis, yAxis, zAxis)
        RotateModel(-yDirection, 0, 1, 0)


    End Sub
    Public Sub RotateModel(ByVal AngleDir As Single, ByVal xSpeed As Single, ByVal ySpeed As Single, ByVal zSpeed As Single)

        Dim xNewLookDirection As Single = 0
        Dim yNewLookDirection As Single = 0
        Dim zNewLookDirection As Single = 0
        Dim xLookDirection As Single = 0
        Dim yLookDirection As Single = 0
        Dim zLookDirection As Single = 0
        Dim CosineAngle As Single = 0
        Dim SineAngle As Single = 0

        'First we will need to calculate the cos and sine of our angle.
        GetCos(CosineAngle, AngleDir)
        GetSine(SineAngle, AngleDir)

        'Next get the look direction (where we are looking) just like in the move camera function.
        xLookDirection = xView - xPos
        yLookDirection = yView - yPos
        zLookDirection = zView - zPos

        'Normalize the direction.
        Dim dp As Single
        dp = 1 / Math.Sqrt(xLookDirection * xLookDirection + yLookDirection * yLookDirection + zLookDirection * zLookDirection)

        xLookDirection *= dp
        yLookDirection *= dp
        zLookDirection *= dp

        'Calculate the new X, Y, and Z positions.
        xNewLookDirection = (CosineAngle + (1 - CosineAngle) * xSpeed) * xLookDirection
        xNewLookDirection += ((1 - CosineAngle) * xSpeed * ySpeed - zSpeed * SineAngle) * yLookDirection
        xNewLookDirection += ((1 - CosineAngle) * xSpeed * zSpeed + ySpeed * SineAngle) * zLookDirection


        yNewLookDirection = ((1 - CosineAngle) * xSpeed * ySpeed + zSpeed * SineAngle) * xLookDirection
        yNewLookDirection += (CosineAngle + (1 - CosineAngle) * ySpeed) * yLookDirection
        yNewLookDirection += ((1 - CosineAngle) * ySpeed * zSpeed - xSpeed * SineAngle) * zLookDirection

        zNewLookDirection = ((1 - CosineAngle) * xSpeed * zSpeed - ySpeed * SineAngle) * xLookDirection
        zNewLookDirection += ((1 - CosineAngle) * ySpeed * zSpeed + xSpeed * SineAngle) * yLookDirection
        zNewLookDirection += (CosineAngle + (1 - CosineAngle) * zSpeed) * zLookDirection

        'add the new rotations to the old view 
        xView = xPos + xNewLookDirection
        yView = yPos + yNewLookDirection
        zView = zPos + zNewLookDirection
        mYaw = xPos + xNewLookDirection
        mPitch = yPos + yNewLookDirection
        mRoll = zPos + zNewLookDirection
    End Sub

    Public Sub RotateYaw(ByVal Angle As Single)

        mYaw += Angle

        Dim v As New Vector3(mYaw, mPitch, mRoll)

        Dim curView As New Vector3(xView, yView, zView)

        v = Vector3.Multiply(curView, Angle)


        xView = v.X
        yView = v.Y
        zView = v.Z

    End Sub

    Public Sub RotatePitch(ByVal Angle As Single)

    End Sub

    Public Sub RotateRoll(ByVal Angle As Single)
        mRoll += Angle

        xUp = mYaw
        yUp = mPitch
        zUp = mRoll
    End Sub
    Public Sub GetCos(ByRef a As Single, ByVal b As Single)
        a = Math.Cos(b)

    End Sub
    Public Sub GetSine(ByRef a As Single, ByVal b As Single)
        a = Math.Sin(b)

    End Sub
End Class

Public Class ModelBehavior
    Public Name As String
    Public Continuous As Boolean = False

    Public Event Behavior(ByVal b As Model.ModelBehaviorEventArgs)


    Public Sub BeginBehavior(ByVal b As Model.ModelBehaviorEventArgs)
        RaiseEvent Behavior(b)
    End Sub

  

End Class
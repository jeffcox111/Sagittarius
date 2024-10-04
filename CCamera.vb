Public Class CCamera

    'properties of the camera
    Public xPos, yPos, zPos As Single
    Public xView, yView, zView As Single
    Public xUp, yUp, zUp As Single
    Public xStrafe, yStrafe, zStrafe As Single
    Public CurrentRotationAngle As Single

    'constants for frame-based movement/.
    Public Forward = 0.8F
    Public Backward = -0.8F
    Public StrafeLeft = 0.8F
    Public StrafeRight = -0.8F


    Public Sub New()
        xPos = 0
        yPos = 0
        zPos = -2.0F
        xView = 0
        yView = 0
        zView = 0
        xUp = 0
        yUp = 1
        zUp = 0
        xStrafe = 0
        yStrafe = 0
        zStrafe = 0


    End Sub

    Public Sub MoveCam(ByVal direction As Single)
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
        UpdateCamera(xLookDirection, yLookDirection, zLookDirection, direction)

    End Sub


    Public Sub UpdateCamera(ByVal xDir As Single, ByVal yDir As Single, ByVal zDir As Single, ByVal dir As Single)

        'terrain crap!
        'Dim h As Color = heightmap.GetPixel((Math.Abs(c.xPos) / 100) * 504, (Math.Abs(c.yPos) / 100) * 360)
        'Dim hy As Single = Math.Abs(255 - h.R) / 100
        'yPos = (hy) / 2 + 0.2

        'Move the camera on the X and Z axis.
        xPos += xDir * dir
        zPos += zDir * dir
        yPos += yDir * dir
        'Move the view along with the position
        xView += xDir * dir
        zView += zDir * dir
        yView += yDir * dir
    End Sub


    Public Sub StrafeCam(ByVal direction As Single)
        CalculateStrafe()
        UpdateCamera(xStrafe, yStrafe, zStrafe, direction)

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

    Public Sub RotateCamera(ByVal AngleDir As Single, ByVal xSpeed As Single, ByVal ySpeed As Single, ByVal zSpeed As Single)

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

    End Sub
    Public Sub RotateByMouse(ByVal mousePosX As Single, ByVal mousePosY As Single, ByVal midX As Integer, ByVal midY As Integer)
        Dim yDirection As Single = 0.0F
        Dim yRotation As Single = 0.0F

        'If the mouseX and mouseY are at the middle of the screen then we can't rotate the view.
        If mousePosX = midX And mousePosY = midY Then
            Exit Sub
        End If

        'Next we get the direction of each axis.  We divide by 1000 to get a smaller value back.
        yDirection = ((midX - mousePosX)) / 1000.0F
        yRotation = ((midY - mousePosY)) / 1000.0F

        'We use curentRotX to help use keep the camera from rotating too far in either direction.
        CurrentRotationAngle -= yRotation

        'Stop the camera from going to high...
        If CurrentRotationAngle > 1.0F Then
            CurrentRotationAngle = 1.0F
            Exit Sub
        End If

        'Stop the camera from going to low...
        If CurrentRotationAngle < -1.0F Then
            CurrentRotationAngle = -1.0F
            Exit Sub
        End If

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

        'rotate the camera
        RotateCamera(yRotation, xAxis, yAxis, zAxis)
        RotateCamera(-yDirection, 0, 1, 0)


    End Sub

    Public Sub GetCos(ByRef a As Single, ByVal b As Single)
        a = Math.Cos(b)

    End Sub
    Public Sub GetSine(ByRef a As Single, ByVal b As Single)
        a = Math.Sin(b)

    End Sub
End Class

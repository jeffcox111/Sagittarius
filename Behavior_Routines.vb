Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D

Module Behavior_Routines

    Public Sub move_forward(ByVal b As Model.ModelBehaviorEventArgs)

        b.Myself.MoveModel(5 * ElapsedTime)

    End Sub

  

    Public Sub rotate(ByVal b As Model.ModelBehaviorEventArgs)
        'b.Myself.RotateModel(0.001, b.Myself.xView, b.Myself.yView, b.Myself.zView)
        'was using 0.001 for angle above

        'b.Myself.RotateByAngle(1, 0, b.Myself.xView, b.Myself.yView)

        b.Myself.mRoll += System.Convert.ToSingle(b.DynamicArgs)

    End Sub

    Public Sub bank_up(ByVal b As Model.ModelBehaviorEventArgs)

        'b.Myself.RotateModel(System.Convert.ToSingle(b.DynamicArgs) / 1000, b.Myself.xView, b.Myself.yView, b.Myself.zView)

        b.Myself.mPitch += System.Convert.ToSingle(b.DynamicArgs)

    End Sub

    Public Sub yaw(ByVal b As Model.ModelBehaviorEventArgs)
        'b.Myself.mYaw += System.Convert.ToSingle(b.DynamicArgs)
        b.Myself.RotateYaw(System.Convert.ToSingle(b.DynamicArgs))

    End Sub

    Public Sub sprite_move(ByVal b As SpriteData.SpriteBehaviorEventArgs)

        b.Myself.xPos += 1
        b.Myself.yPos += 1

    End Sub

    Public Sub ship_collide(ByVal b As Model.ModelBehaviorEventArgs)

        Dim info() As IntersectInformation
        

            If b.Myself.mesh.Intersect(New Vector3(b.Myself.xPos, b.Myself.yPos, b.Myself.zPos), New Vector3(b.Myself.xPos, b.Myself.yPos, b.Myself.zPos), info) Then
                HudMessager.AddMessage("Ship has hit the ground")

            End If

      
    End Sub

    Public Sub propellor_spin(ByVal b As Model.ModelBehaviorEventArgs)
        b.Myself.mYaw += (3 * ElapsedTime)

    End Sub

    Public Sub chopper_lift(ByVal b As Model.ModelBehaviorEventArgs)
        b.Myself.yPos += (3 * ElapsedTime)
        device.Lights(3).YPosition = b.Myself.yPos '- 1

    End Sub

    Public Sub chopper_lower(ByVal b As Model.ModelBehaviorEventArgs)
        b.Myself.yPos -= (3 * ElapsedTime)
        device.Lights(3).YPosition = b.Myself.yPos '- 1
    End Sub

    Public Sub chopper_collide(ByVal b As Model.ModelBehaviorEventArgs)
        Dim i As New IntersectInformation

        Dim M As New Model
        Dim T As New Model

        M = ModelsManager.models("cobra")
        T = ModelsManager.models("terrain")
        'HudMessager.AddMessage("got here")
        ' If c.mesh.Intersect(New Vector3(c.xPos, c.yPos, c.zPos), New Vector3(0, 0, 0), i) Then
        If T.mesh.Intersect(New Vector3(M.xPos, M.yPos, M.zPos), New Vector3(M.xPos, M.yPos, M.zPos)) Then
            'HudMessager.AddMessage("chopper hit ground")
            M.Behaviors("chopper_lower").continuous = False
        End If

    End Sub

    Public Sub terrain_glide(ByVal b As Model.ModelBehaviorEventArgs)

        Dim i As New IntersectInformation
        Dim t As New Model
        t = ModelsManager.models("terrain")

        If t.mesh.Intersect(New Vector3(b.Myself.xPos, b.Myself.yPos, b.Myself.zPos), New Vector3(b.Myself.xPos, b.Myself.yPos, b.Myself.zPos)) Then
            b.Myself.yPos += 0.16 'i.Dist '0.01
            b.Myself.yView += 0.16 'i.Dist '0.01
            ' HudMessager.AddMessage(i.Dist)

            'b.Myself.yVel = 0
        End If
    End Sub

    Public Sub gravity(ByVal b As Model.ModelBehaviorEventArgs)

        Dim t As Model
        t = ModelsManager.models("terrain")
        If t.mesh.Intersect(New Vector3(b.Myself.xPos, b.Myself.yPos, b.Myself.zPos), New Vector3(b.Myself.xPos, b.Myself.yPos, b.Myself.zPos)) = False Then
            b.Myself.yPos -= 0.16
            b.Myself.yView -= 0.16
            

        End If
    End Sub

    Public Sub sprite_toby_click(ByVal b As SpriteData.SpriteBehaviorEventArgs)

        HudMessager.AddMessage("Toby!")

    End Sub
End Module

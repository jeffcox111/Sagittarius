Public Class LoadManager

    Public Sub AddModels(ByVal modelsfile As String)

        'skybox.LoadMesh("skybox2.x")
        'skybox.xPos = 0
        'skybox.yPos = 0
        'skybox.zPos = 0

        'create a file and reader
        Dim fil As System.IO.File
        Dim sfile As New System.IO.StreamReader(fil.OpenRead(modelsfile))

        'create script object and strings to store code
        Dim script As New MSScriptControl.ScriptControl
        Dim scripts() As String
        Dim tmpScript As String

        'get scripts from LoadModels.VBS
        Dim allscripts As String = sfile.ReadToEnd
        scripts = Split(allscripts, "<!>")

        'dummy model object
        Dim m As New Model

        Try
            'run all the load model scripts
            For Each tmpScript In scripts
                script = New MSScriptControl.ScriptControl

                script.Language = "VBScript" 'CHANGE THIS LINE TO SUPPORT JavaScript!
                script.AddObject("HudMessager", HudMessager, True)
                script.AddObject("Model", m, True)
                script.AddObject("GraphicLoader", GraphicLoader, True)


                script.ExecuteStatement(tmpScript)

                ModelsManager.models.Add(m, m.Name)

                m = New Model

            Next

            HudMessager.AddMessage(scripts.Length.ToString() & " models loaded.")

            sfile.Close()

            'catch a problem with the LoadModels script
        Catch ex As Exception
            'MsgBox("Exception report: " & ex.Message)
            HudMessager.AddMessage("Problem loading model.")

            'End

        End Try

        Try
            Environment.LoadSkyBox("j_skybox.x")

        Catch ex As Exception

        End Try
        
    End Sub
    Public Sub CopyModel(ByVal name As String, ByVal newname As String)


        Dim m As New Model
        Dim t As Model

        Try
            t = ModelsManager.models(name)
            m.Name = newname

            With m
                .Behaviors = t.Behaviors
                .BoundingSphereRadius = t.BoundingSphereRadius
                .CurrentRotationAngle = t.CurrentRotationAngle
                .Enabled = t.Enabled
                .FileName = t.FileName
                .mesh = t.mesh
                .meshMaterials = t.meshMaterials
                .meshTextures = t.meshTextures
                .mPitch = t.mPitch
                .mRoll = t.mRoll
                .mYaw = t.mYaw
                .Scale = t.Scale
                .xPos = t.xPos
                .xStrafe = t.xStrafe
                .xUp = t.xUp
                .xVel = t.xVel
                .xView = t.xView
                .yPos = t.yPos
                .yStrafe = t.yStrafe
                .yUp = t.yUp
                .yVel = t.yVel
                .yView = t.yView
                .zPos = t.zPos
                .zStrafe = t.zStrafe
                .zUp = t.zUp
                .zVel = t.zVel
                .zView = t.zView

            End With

            ModelsManager.models.Add(m, newname)
         
        Catch ex As Exception
            'HudMessager.AddMessage("copy failed")
            Exit Sub
        End Try

        'Dim m As New Model

    End Sub
    Public Sub AddBehaviors()

        'add model behaviors
        Dim tmpbehavior As New ModelBehavior

        tmpbehavior.Name = "move_forward"
        AddHandler tmpbehavior.Behavior, AddressOf move_forward
        'tmpbehavior.Continuous = True
        ModelsManager.models("slayer").Behaviors.Add(tmpbehavior, tmpbehavior.Name)

        tmpbehavior = New ModelBehavior
        tmpbehavior.Name = "rotate"
        AddHandler tmpbehavior.Behavior, AddressOf rotate
        ModelsManager.models("slayer").behaviors.add(tmpbehavior, tmpbehavior.Name)

        tmpbehavior = New ModelBehavior
        tmpbehavior.Name = "bank_up"
        AddHandler tmpbehavior.Behavior, AddressOf bank_up
        ModelsManager.models("slayer").behaviors.add(tmpbehavior, tmpbehavior.Name)

        tmpbehavior = New ModelBehavior
        tmpbehavior.Name = "yaw"
        AddHandler tmpbehavior.Behavior, AddressOf yaw
        ModelsManager.models("slayer").behaviors.add(tmpbehavior, tmpbehavior.Name)

        tmpbehavior = New ModelBehavior
        tmpbehavior.Name = "chopper_collide"
        tmpbehavior.Continuous = True
        AddHandler tmpbehavior.Behavior, AddressOf chopper_collide
        ModelsManager.models("cobra").behaviors.add(tmpbehavior)

        tmpbehavior = New ModelBehavior
        tmpbehavior.Name = "propellor_spin"
        AddHandler tmpbehavior.Behavior, AddressOf propellor_spin
        ModelsManager.models("cobra_blade").behaviors.add(tmpbehavior, tmpbehavior.Name)

        tmpbehavior = New ModelBehavior
        tmpbehavior.Name = "chopper_lift"
        AddHandler tmpbehavior.Behavior, AddressOf chopper_lift
        ModelsManager.models("cobra_blade").behaviors.add(tmpbehavior, tmpbehavior.Name)
        ModelsManager.models("cobra").behaviors.add(tmpbehavior, tmpbehavior.Name)

        tmpbehavior = New ModelBehavior
        tmpbehavior.Name = "chopper_lower"
        AddHandler tmpbehavior.Behavior, AddressOf chopper_lower
        ModelsManager.models("cobra_blade").behaviors.add(tmpbehavior, tmpbehavior.Name)
        ModelsManager.models("cobra").behaviors.add(tmpbehavior, tmpbehavior.Name)

        tmpbehavior = New ModelBehavior
        tmpbehavior.Name = "terrain_glide"
        tmpbehavior.Continuous = True
        AddHandler tmpbehavior.Behavior, AddressOf terrain_glide
        ModelsManager.models("slayer").behaviors.add(tmpbehavior, tmpbehavior.Name)

        tmpbehavior = New ModelBehavior
        tmpbehavior.Name = "gravity"
        tmpbehavior.Continuous = True
        AddHandler tmpbehavior.Behavior, AddressOf gravity
        ModelsManager.models("slayer").behaviors.add(tmpbehavior, tmpbehavior.Name)
        'ModelsManager.models("cobra").behaviors.add(tmpbehavior, tmpbehavior.Name)
        'ModelsManager.models("cobra_blade").behaviors.add(tmpbehavior, tmpbehavior.Name)

        'add sprite behaviors

        Dim tmpspritebehavior As New SpriteBehavior
        tmpspritebehavior.Name = "sprite_toby_click"
        tmpspritebehavior.Continuous = False
        AddHandler tmpspritebehavior.Behavior, AddressOf sprite_toby_click
        'spManager.Sprites("Toby").behaviors.add(tmpspritebehavior, tmpspritebehavior.Name)
        spManager.Sprites("Toby").clickbehavior = tmpspritebehavior




    End Sub
    Public Sub AddSprites()

        'create a file and reader
        Dim fil As System.IO.File
        Dim sfile As New System.IO.StreamReader(fil.OpenRead("loadsprites.vbs"))

        'create script object and strings to store code
        Dim script As New MSScriptControl.ScriptControl
        Dim scripts() As String
        Dim tmpScript As String


        'get scripts from LoadModels.VBS
        Dim allscripts As String = sfile.ReadToEnd
        scripts = Split(allscripts, "<!>")


        Try
            'run all the load model scripts
            For Each tmpScript In scripts
                script = New MSScriptControl.ScriptControl

                script.Language = "VBScript" 'CHANGE THIS LINE TO SUPPORT JavaScript!
                script.AddObject("HudMessager", HudMessager, True)
                script.AddObject("spManager", spManager, True)


                script.ExecuteStatement(tmpScript)

            Next

            HudMessager.AddMessage(spManager.Sprites.Count.ToString & " sprites loaded.")

            sfile.Close()

            'catch a problem with the LoadModels script
        Catch ex As Exception
            'MsgBox("Exception report: " & ex.Message)
            HudMessager.AddMessage("Problem loading sprite.")

            'End

        End Try


    End Sub
    Public Sub ClearModels()
        Dim i As Integer
        Try
            For i = ModelsManager.models.Count To 1 Step -1
                ModelsManager.models.Remove(i)
            Next
            HudMessager.AddMessage("Models unloaded.")
        Catch
            HudMessager.AddMessage("Error unloading model " & i & ".")
        End Try
    End Sub
    Public Sub ClearSprites()
        Dim i As Integer
        Try
            For i = spManager.Sprites.Count To 1 Step -1
                spManager.Sprites.Remove(i)
            Next
            HudMessager.AddMessage("Sprites unloaded.")
        Catch
            HudMessager.AddMessage("Error unloading sprite " & i & ".")
        End Try

    End Sub

End Class

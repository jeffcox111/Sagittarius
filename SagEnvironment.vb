Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D

Public Class SagEnvironment


    Public SkyBox As New Model

    Public Sub LoadSkyBox(ByVal skyboxModel As String)


        Try
            SkyBox.LoadMesh(skyboxModel)
        Catch
            HudMessager.AddMessage("Could not init Skybox", System.Drawing.Color.Green)

        End Try

    End Sub

    Public Sub DrawSkyBox()
        SkyBox.xPos = c.xPos
        SkyBox.yPos = c.yPos
        SkyBox.zPos = c.zPos

        device.RenderState.ZBufferEnable = False
        device.RenderState.ZBufferWriteEnable = False
        device.RenderState.Lighting = False

        SkyBox.DrawMesh(device, 0, 0, 0, c.xPos, c.yPos, c.zPos)

        device.RenderState.ZBufferEnable = True
        device.RenderState.ZBufferWriteEnable = True
        device.RenderState.Lighting = True

    End Sub

    Public Sub ChangeFillMode(ByVal fill As FillMode)

        Dim tmp As Model
        For Each tmp In ModelsManager.models
            tmp.Fillmode = fill

        Next
        SkyBox.Fillmode = fill

    End Sub

    Public Sub SkyboxEnable(ByVal enable As Boolean)
        SkyBox.Enabled = enable

    End Sub
End Class

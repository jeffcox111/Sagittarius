Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D
Imports Microsoft.DirectX.DirectSound
Imports Microsoft.DirectX.AudioVideoPlayback

Public Class SoundTrackManager

    Public soundDevice As New DirectSound.Device
    Public WithEvents currentTrack As Audio
    Public PlaySoundTrack As Boolean = True


    Public Sub BuildSoundTrack()
        soundDevice.SetCooperativeLevel(frm, CooperativeLevel.Normal)

        Dim soundfiles() As String

        Dim musicFolder As System.IO.Directory

        Dim musicfiles() As System.IO.File

        soundfiles = musicFolder.GetFiles("Music\")

        Dim tmpFile As String
        For Each tmpFile In soundfiles
            SoundTrackCol.Add(tmpFile)
        Next

        SoundTrack = SoundTrackCol.GetEnumerator



    End Sub
    Public Sub BeginSoundTrack()
        PlaySoundTrack = True

        Randomize()

        Dim i As Integer = Rnd() * SoundTrackCol.Count
        Dim p As Integer
        For p = 0 To i
            SoundTrack.MoveNext()
        Next p

        Try
            currentTrack = New Audio(SoundTrack.Current)
            currentTrack.Play()
        Catch
            SoundTrack.Reset()
            SoundTrack.MoveNext()
            currentTrack = New Audio(SoundTrack.Current)
            currentTrack.Play()
        End Try

    End Sub
    Public Sub StopSoundTrack()

        currentTrack.Stop()
        PlaySoundTrack = False


    End Sub
    Public Sub CycleTrack()
        currentTrack.Stop()
        SoundTrack.MoveNext()

        If PlaySoundTrack = True Then


            Try
                currentTrack = New Audio(SoundTrack.Current)
                currentTrack.Play()
            Catch
                SoundTrack.Reset()
                SoundTrack.MoveNext()
                currentTrack = New Audio(SoundTrack.Current)
                currentTrack.Play()
            End Try

        End If


    End Sub
    Private Sub currentTrack_Ending(ByVal sender As Object, ByVal e As System.EventArgs) Handles currentTrack.Ending
        CycleTrack()

    End Sub
End Class

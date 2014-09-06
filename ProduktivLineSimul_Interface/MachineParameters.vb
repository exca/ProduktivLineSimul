Public Class MachineParameters

    ''' <summary>
    ''' Machine for whith the action applies
    ''' </summary>
    ''' <remarks></remarks>
    Dim relatedMachine As ProduktivLineSimul.clsModular

    ''' <summary>
    ''' Create the user command control and show it
    ''' </summary>
    ''' <param name="_machine"></param>
    ''' <remarks></remarks>
    Public Sub New(ByRef _machine As ProduktivLineSimul.clsModular)
        relatedMachine = _machine
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Load equipments configuration
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadValues()

        If relatedMachine.IsConveyor Then
            TrackBar1.Enabled = False
            NumericUpDown1.Enabled = False
            NumericUpDown2.Enabled = False
            TrackBar3.Enabled = False
            TrackBar2.Value = CInt(relatedMachine.TotalAccumulation)
        ElseIf relatedMachine.IsMachine Then
            TrackBar1.Value = CInt(relatedMachine.speed * 3600)
            NumericUpDown1.Value = CInt(relatedMachine.MTBF)
            NumericUpDown2.Value = CInt(relatedMachine.MTTR)
            TrackBar3.Value = CInt(relatedMachine.rejectrate * 10000)
            TrackBar2.Enabled = False
        End If

        'Refresh labels
        UpdateLabels()

    End Sub

    ''' <summary>
    ''' Update displayed values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateLabels()

        Me.Text = relatedMachine.EquipmentName + " Configuration"

        If relatedMachine.IsConveyor Then
            Label1.Text = "Machine Speed : "
            Label2.Text = "MTBF (secs.): "
            Label3.Text = "MTTR (secs.): "
            Label5.Text = "Rejection rate: "
            Label4.Text = "Accumulation: " & TrackBar2.Value & " Bottles"
        ElseIf relatedMachine.IsMachine Then
            Label1.Text = "Machine Speed : " & CInt(TrackBar1.Value) & " BPH"
            Label2.Text = "MTBF (secs.): "
            Label3.Text = "MTTR (secs.): "
            Label5.Text = "Rejection rate: " & CDec(TrackBar3.Value / 100) & " %"
            Label4.Text = "Accumulation: "
        End If

    End Sub

    ''' <summary>
    ''' Refresh displayed values
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RefreshLabels(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar1.Scroll, TrackBar3.Scroll, TrackBar2.Scroll
        UpdateLabels()
    End Sub

    ''' <summary>
    ''' Reset user modifications
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        LoadValues()
    End Sub

    ''' <summary>
    ''' Close user control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    ''' <summary>
    ''' Apply user configuration on the equipment
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If relatedMachine.IsConveyor Then
            relatedMachine.SetAccumulator(TrackBar2.Value, 15, 50)
        ElseIf relatedMachine.IsMachine Then
            relatedMachine.speed = CDec(TrackBar1.Value / 3600)
            relatedMachine.MTBF = NumericUpDown1.Value
            relatedMachine.MTTR = NumericUpDown2.Value
            relatedMachine.rejectrate = CDec(TrackBar3.Value / 10000)
        End If
    End Sub

    Private Sub NumericUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NumericUpDown1.ValueChanged, NumericUpDown2.ValueChanged
        Try
            If Not NumericUpDown1.Value = 0 And Not NumericUpDown2.Value = 0 Then
                Label6.Text = "Availability = " & Decimal.Floor(CDec(NumericUpDown1.Value / (NumericUpDown1.Value + NumericUpDown2.Value)) * 10000) / 100 & " %"
            Else
                Label6.Text = "Availability = 100 %"
            End If
        Catch ex As Exception
            Label6.Text = "Availability = N/A"
        End Try
    End Sub

End Class
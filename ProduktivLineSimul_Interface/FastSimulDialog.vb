Imports System.Windows.Forms

Public Class FastSimulDialog

    Dim SimulTime As Integer = 8 * 3600
    Dim SimulTimeLeft As Integer = 8 * 3600
    Dim lastSimulTimeLeft As Integer = 8 * 3600
    Dim lastExecutionTime As Decimal = 20
    Dim privateThread As Threading.Thread

    Public Sub New(ByVal _totalsecs As Integer)
        InitializeComponent()
        SimulTime = _totalsecs
        SimulTimeLeft = _totalsecs
        lastSimulTimeLeft = _totalsecs
    End Sub

    Private Sub FastSimulDialog_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Timer1.Start()
        privateThread = New Threading.Thread(AddressOf privateWork)
        privateThread.Start()
    End Sub

    Private Sub privateWork()
        Try
            'Execution wihtout calculating formulas
            While SimulTimeLeft > 1
                ProduktivLineSimul.Simulate_oneStep(False)
                SimulTimeLeft -= 1
            End While
            'Last execution calculating formulas
            ProduktivLineSimul.Simulate_oneStep(True)
            SimulTimeLeft -= 1
        Catch ex As Exception
        End Try
    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        privateThread.Abort()
        'Last execution calculating formulas
        ProduktivLineSimul.Simulate_oneStep(True)
        SimulTimeLeft -= 1
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        If privateThread.IsAlive Then
            Try
                Dim ExecutionTime As Decimal = CDec(SimulTime / ((lastSimulTimeLeft - SimulTimeLeft) * 2))
                lastExecutionTime *= 3
                lastExecutionTime += ExecutionTime
                lastExecutionTime /= 4
                Label1.Text = "Simulated time left: " & ProduktivLineSimul.toTimeDuration(SimulTimeLeft) & _
                    vbCrLf & "Calculation time (on this computer): " & ProduktivLineSimul.toTimeDuration(lastExecutionTime) & _
                    " - left: " & ProduktivLineSimul.toTimeDuration(CDec(lastExecutionTime * (SimulTimeLeft / SimulTime)))
                lastSimulTimeLeft = SimulTimeLeft
                ProgressBar1.Value = CInt(100 * ((SimulTime - SimulTimeLeft) / SimulTime))
            Catch
            End Try
        Else
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End If
    End Sub


End Class

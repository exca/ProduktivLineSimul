Public Class MainForm

    ''Constants declaration

    ''Variables declaration

    Dim PET_Blower As ProduktivLineSimul.clsModular
    Dim PET_Filler As ProduktivLineSimul.clsModular
    Dim PET_Accu1 As ProduktivLineSimul.clsModular
    Dim PET_Labeller As ProduktivLineSimul.clsModular
    Dim PET_Accu2 As ProduktivLineSimul.clsModular
    Dim PET_Packer As ProduktivLineSimul.clsModular
    Dim PET_Pal As ProduktivLineSimul.clsModular
    Dim PET_PalWrapper As ProduktivLineSimul.clsModular


    ''' <summary>
    ''' Initialize variables and modules for simulation tool
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Initialize simulator distributions
        ProduktivLineSimul.initDistributions()

        'Initialize default project
        Initialize_Project(ProjectTypes.DefaultLine)


    End Sub

    ''' <summary>
    ''' Types of projects handled by the simulator
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum ProjectTypes
        DefaultLine
        PETLine
    End Enum

    ''' <summary>
    ''' Initialize the project in the simulator view
    ''' </summary>
    ''' <param name="_projectType"></param>
    ''' <remarks></remarks>
    Private Sub Initialize_Project(ByVal _projectType As ProjectTypes)

        ProduktivLineSimul.resetLinks()
        ProduktivLineSimul.ResetInputOutputCounts()

        'Choose from project type
        Select Case _projectType

            Case Else 'Default : ProjectTypes.PETLine or ProjectTypes.DefaultLine
                'Hide all other projects
                FlowLayoutPanelPETLINE.Hide()
                'Show PETLine project
                FlowLayoutPanelPETLINE.Show()

                'Call project
                Project_PETLine()
        End Select

        ProduktivLineSimul.DefineRoutesAndInit()
        RefreshValues()

    End Sub

    ''' <summary>
    ''' Initialize PETLine project
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Project_PETLine()

        PET_Blower = New ProduktivLineSimul.clsModular("Blower", 25000, ProduktivLineSimul.clsModular.enumSpeedUnit.per_Hour, ProduktivLineSimul.clsModular.enumParameters.Eff_MTTR, 0.99, 65)
        PET_Blower.rejectrate = 0.0015

        PET_Filler = New ProduktivLineSimul.clsModular("Filler", 25000, ProduktivLineSimul.clsModular.enumSpeedUnit.per_Hour, ProduktivLineSimul.clsModular.enumParameters.Eff_MTTR, 0.98, 45)
        PET_Filler.rejectrate = 0.0025

        PET_Accu1 = New ProduktivLineSimul.clsModular(ProduktivLineSimul.clsModular.enumModularType.Transport)
        PET_Accu1.SetAccumulator(800, 15, 42)

        PET_Labeller = New ProduktivLineSimul.clsModular("Labeler", 28750, ProduktivLineSimul.clsModular.enumSpeedUnit.per_Hour, ProduktivLineSimul.clsModular.enumParameters.Eff_MTTR, 0.95, 35)
        PET_Labeller.rejectrate = 0.001

        PET_Accu2 = New ProduktivLineSimul.clsModular(ProduktivLineSimul.clsModular.enumModularType.Transport)
        PET_Accu2.SetAccumulator(800, 15, 48)

        PET_Packer = New ProduktivLineSimul.clsModular("Packer", 32500, ProduktivLineSimul.clsModular.enumSpeedUnit.per_Hour, ProduktivLineSimul.clsModular.enumParameters.Eff_MTTR, 0.95, 55)
        PET_Packer.unitCycle = 6

        PET_Pal = New ProduktivLineSimul.clsModular("Palletizer", 32500, ProduktivLineSimul.clsModular.enumSpeedUnit.per_Hour, ProduktivLineSimul.clsModular.enumParameters.Eff_MTTR, 0.98, 60)
        PET_Pal.unitCycle = 6 * 8

        PET_PalWrapper = New ProduktivLineSimul.clsModular("Pallet Wrapper", 32500, ProduktivLineSimul.clsModular.enumSpeedUnit.per_Hour, ProduktivLineSimul.clsModular.enumParameters.Eff_MTTR, 0.99, 50)
        PET_PalWrapper.unitCycle = 6 * 8 * 5

        ProduktivLineSimul.addLink(ProduktivLineSimul.modInput, PET_Blower)
        ProduktivLineSimul.addLink(PET_Blower, PET_Filler)
        ProduktivLineSimul.addLink(PET_Filler, PET_Accu1)
        ProduktivLineSimul.addLink(PET_Accu1, PET_Labeller)
        ProduktivLineSimul.addLink(PET_Labeller, PET_Accu2)
        ProduktivLineSimul.addLink(PET_Accu2, PET_Packer)
        ProduktivLineSimul.addLink(PET_Packer, PET_Pal)
        ProduktivLineSimul.addLink(PET_Pal, PET_PalWrapper)
        ProduktivLineSimul.addLink(PET_PalWrapper, ProduktivLineSimul.modOutput)

    End Sub

    ''' <summary>
    ''' Enable timer for real-time simulation
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub StartSimul_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton2.Click, StartToolStripMenuItem.Click
        Timer1.Interval = CInt(1000 * ProduktivLineSimul.TIMEBASE)
        Timer1.Start()
    End Sub

    ''' <summary>
    ''' Simulate one step at each interval of time
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Timer1.Stop()

        'Simulate
        ProduktivLineSimul.Simulate_oneStep()

        'Show results
        RefreshValues()

        Timer1.Start()
    End Sub

    ''' <summary>
    ''' Refresh displayed values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RefreshValues()

        ''PET LINE
        'Blower
        PictureBox2.Image = getRessourceForMachineState(PET_Blower)
        Label1.Text = getInfosOnEquipment(PET_Blower)

        'Filler
        PictureBox3.Image = getRessourceForMachineState(PET_Filler)
        Label2.Text = getInfosOnEquipment(PET_Filler, True)

        'Accu1
        PictureBox5.Image = getRessourceForMachineState(PET_Accu1)
        Label3.Text = getInfosOnEquipment(PET_Accu1)
        ProgressBar1.Value = Val(PET_Accu1.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Content_use_percent))
        Label9.Text = PET_Accu1.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Content_use_percent_txt)

        'Labeller
        PictureBox7.Image = getRessourceForMachineState(PET_Labeller)
        Label4.Text = getInfosOnEquipment(PET_Labeller)

        'Accu2
        PictureBox9.Image = getRessourceForMachineState(PET_Accu2)
        Label5.Text = getInfosOnEquipment(PET_Accu2)
        ProgressBar2.Value = Val(PET_Accu2.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Content_use_percent))
        Label10.Text = PET_Accu2.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Content_use_percent_txt)

        'Packer
        PictureBox11.Image = getRessourceForMachineState(PET_Packer)
        Label6.Text = getInfosOnEquipment(PET_Packer)

        'Pal
        PictureBox13.Image = getRessourceForMachineState(PET_Pal)
        Label7.Text = getInfosOnEquipment(PET_Pal)

        'PalWrap
        PictureBox15.Image = getRessourceForMachineState(PET_PalWrapper)
        Label8.Text = getInfosOnEquipment(PET_PalWrapper)


    End Sub

    ''' <summary>
    ''' Return image to be deisplayed for equipment state
    ''' </summary>
    ''' <param name="_machine"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getRessourceForMachineState(ByRef _machine As ProduktivLineSimul.clsModular) As Image
        Select Case _machine.lastState
            Case ProduktivLineSimul.clsModular.enumStates.Running
                Return My.Resources._02_redo_icon

            Case ProduktivLineSimul.clsModular.enumStates.Stopped
                Return My.Resources._158_wrench_2_icon

            Case Else 'Undefined or Waiting
                Return My.Resources._11_clock_icon
        End Select
    End Function

    ''' <summary>
    ''' Return basical information about the equipment state
    ''' </summary>
    ''' <param name="_machine"></param>
    ''' <param name="_criticalMachine"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getInfosOnEquipment(ByRef _machine As ProduktivLineSimul.clsModular, Optional ByVal _criticalMachine As Boolean = False) As String
        Return "Speed: " & _machine.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Speed_Current) & " / " & _machine.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Speed_Max) & _
            vbCrLf & "Running time: " & _machine.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Time_Running) & _
            vbCrLf & "Stop time: " & _machine.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Time_Stopped) & _
            vbCrLf & "Total products: " & _machine.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Counter_Processed) & _
            vbCrLf & "Total rejects: " & _machine.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Counter_Rejected) & _
            IIf(_criticalMachine, vbCrLf & "Line OEE: " & _machine.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.OEE), "")
    End Function

    ''' <summary>
    ''' User interraction, stop on PET_Blower
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        PET_Blower.stopRequest()
    End Sub

    ''' <summary>
    ''' User interraction, stop on PET_Filler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub PictureBox4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox4.Click
        PET_Filler.stopRequest()
    End Sub

    ''' <summary>
    ''' User interraction, stop on PET_Labeller
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub PictureBox8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox8.Click
        PET_Labeller.stopRequest()
    End Sub

    ''' <summary>
    ''' User interraction, stop on PET_Packer
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub PictureBox12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox12.Click
        PET_Packer.stopRequest()
    End Sub

    ''' <summary>
    ''' User interraction, stop on PET_Pal
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub PictureBox14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox14.Click
        PET_Pal.stopRequest()
    End Sub

    ''' <summary>
    ''' User interraction, stop on PET_PalWrapper
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub PictureBox16_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox16.Click
        PET_PalWrapper.stopRequest()
    End Sub

    ''' <summary>
    ''' Launch a 8-hours simulation
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ToolStripButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton1.Click, Simulate8HoursToolStripMenuItem.Click
        Timer1.Stop()

        'Simulate 8 hours
        Dim fastExecution As New FastSimulDialog(8 * 60 * 60)
        fastExecution.ShowDialog()

        'Show results
        RefreshValues()
    End Sub

    ''' <summary>
    ''' Pause simulation
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ToolStripButton3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton3.Click
        Timer1.Stop()
    End Sub

End Class

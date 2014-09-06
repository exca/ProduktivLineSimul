Public Class UserControls

    ''' <summary>
    ''' Machine for whith the action applies
    ''' </summary>
    ''' <remarks></remarks>
    Dim relatedMachine As ProduktivLineSimul.clsModular

    ''' <summary>
    ''' Quit user control if loosing focus
    ''' </summary>
    ''' <remarks></remarks>
    Dim quitOnFocus As Boolean = True


    ''' <summary>
    ''' Create the user command control and show it
    ''' </summary>
    ''' <param name="_machine"></param>
    ''' <remarks></remarks>
    Public Sub New(ByRef _machine As ProduktivLineSimul.clsModular)
        relatedMachine = _machine
        InitializeComponent()
        Me.Show()
    End Sub

    ''' <summary>
    ''' Loading user command control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub UserControls_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim formSize As New Size(36, 120)
        Me.Size = formSize
        Dim formPosition As New Point(MousePosition.X - 5, MousePosition.Y - 5)
        Me.Location = formPosition
    End Sub

    ''' <summary>
    ''' Start or Stop selected Machine
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        quitOnFocus = False
        relatedMachine.stopRequest()
        Me.Close()
    End Sub

    ''' <summary>
    ''' Open the configuration control for the machine
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub PictureBox2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox2.Click
        quitOnFocus = False
        Dim myConfig As New MachineParameters(relatedMachine)
        myConfig.Owner = MainForm
        myConfig.LoadValues()
        myConfig.Show()
        Me.Close()
    End Sub

    ''' <summary>
    ''' Open the statistics control for the machine
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        quitOnFocus = False
        Dim myStats As New ChartDisplay(relatedMachine)
        myStats.Owner = MainForm
        myStats.UpdateValues()
        myStats.Show()
        Me.Close()
    End Sub


    ''' <summary>
    ''' Close controle when leaving focus
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub UserControls_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate, MyBase.Leave
        If quitOnFocus Then Me.Close()
    End Sub
End Class
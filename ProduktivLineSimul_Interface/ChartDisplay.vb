Imports System.Drawing.Imaging
Imports System.Drawing

Public Class ChartDisplay

    ''' <summary>
    ''' Machine for whith the action applies
    ''' </summary>
    ''' <remarks></remarks>
    Dim relatedMachine As ProduktivLineSimul.clsModular

    Dim GraphWhiteBrush As Brush
    Dim GraphDefaultBrush As Brush
    Dim FaultOwnBrush As Brush
    Dim FaultWaitUpBrush As Brush
    Dim FaultWaitDownBrush As Brush
    Dim FaultRunBrush As Brush
    Dim GraphDefaultFont As Font

    ''' <summary>
    ''' Create the user command control and show it
    ''' </summary>
    ''' <param name="_machine"></param>
    ''' <remarks></remarks>
    Public Sub New(ByRef _machine As ProduktivLineSimul.clsModular)
        relatedMachine = _machine

        GraphWhiteBrush = Brushes.AntiqueWhite
        GraphDefaultBrush = Brushes.DarkGray
        FaultOwnBrush = Brushes.Red
        FaultRunBrush = Brushes.Green
        FaultWaitUpBrush = Brushes.LightCyan
        FaultWaitDownBrush = Brushes.DarkCyan
        GraphDefaultFont = New Font(FontFamily.GenericSerif, 8, FontStyle.Regular)

        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Displaying form at cursor position
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ChartDisplay_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim formPosition As New Point(MousePosition.X - 25, MousePosition.Y - 25)
        Me.Location = formPosition
    End Sub

    ''' <summary>
    ''' Update displayed values
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateValues()

        Me.Text = relatedMachine.EquipmentName + " Statistics"

        'Create the bitmap
        Dim resultChart As New Bitmap(My.Resources.GraphBaground_200x200)
        Using Graphic = Graphics.FromImage(resultChart)

            'Define the first x for running time
            Dim xLast As Integer = 10
            Dim xDest As Integer = relatedMachine.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Time_Running_percent, False) / 100 * 180
            If xDest > 1 Then
                Graphic.FillRectangle(FaultRunBrush, xLast, 5, xDest, 30)
                Graphic.FillRectangle(FaultRunBrush, 10, 45, Math.Min(xDest, 50), 30)
                xLast += xDest
            End If
            Graphic.DrawString(relatedMachine.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Time_Running), GraphDefaultFont, Brushes.Black, 10 + Math.Min(xDest, 50), 45 + 15)
            Graphic.DrawString("Running time:", GraphDefaultFont, Brushes.Black, 10 + Math.Min(xDest, 50), 45)

            'Own stops
            xDest = relatedMachine.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Time_Stopped_percent, False) / 100 * 180
            If xDest > 1 Then
                Graphic.FillRectangle(FaultOwnBrush, xLast, 5, xDest, 30)
                Graphic.FillRectangle(FaultOwnBrush, 10, 85, Math.Min(xDest, 50), 30)
                xLast += xDest
            End If
            Graphic.DrawString(relatedMachine.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Time_Stopped), GraphDefaultFont, Brushes.Black, 10 + Math.Min(xDest, 50), 85 + 15)
            Graphic.DrawString("Downtime:", GraphDefaultFont, Brushes.Black, 10 + Math.Min(xDest, 50), 85)

            'Downstream stops
            xDest = relatedMachine.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Time_WaitingDownstream_percent, False) / 100 * 180
            If xDest > 1 Then
                Graphic.FillRectangle(FaultWaitDownBrush, xLast, 5, xDest, 30)
                Graphic.FillRectangle(FaultWaitDownBrush, 10, 125, Math.Min(xDest, 50), 30)
                xLast += xDest
            End If
            Graphic.DrawString(relatedMachine.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Time_WaitingDownstream), GraphDefaultFont, Brushes.Black, 10 + Math.Min(xDest, 50), 125 + 15)
            Graphic.DrawString("Waiting outfeed:", GraphDefaultFont, Brushes.Black, 10 + Math.Min(xDest, 50), 125)

            'Upstream stops
            xDest = relatedMachine.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Time_WaitingUpstream_percent, False) / 100 * 180
            If xDest > 1 Then
                Graphic.FillRectangle(FaultWaitUpBrush, xLast, 5, xDest, 30)
                Graphic.FillRectangle(FaultWaitUpBrush, 10, 165, Math.Min(xDest, 50), 30)
                xLast += xDest
            End If
            Graphic.DrawString(relatedMachine.getFormulaResult(ProduktivLineSimul.clsModular.enumFormulas.Time_WaitingUpstream), GraphDefaultFont, Brushes.Black, 10 + Math.Min(xDest, 50), 165 + 15)
            Graphic.DrawString("Waiting infeed:", GraphDefaultFont, Brushes.Black, 10 + Math.Min(xDest, 50), 165)

        End Using


        PictureBox1.Image = resultChart

    End Sub

    ''' <summary>
    ''' Refresh displayed values every second
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        UpdateValues()
    End Sub

End Class
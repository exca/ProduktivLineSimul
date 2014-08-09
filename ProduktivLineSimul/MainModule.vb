Module MainModule

    Public Const TIMEBASE As Decimal = 1
    Public Const NUMDECIMAL As Integer = 2
    Public Const EPSILON As Decimal = 0.0001
    Public Const MAX_SPEED As Decimal = 10000
    Public Const MAX_TMBF As Decimal = 3600 * 8
    Friend LastTRPNum As Integer = 0

    Friend Const RealisticDistributions As Boolean = False

    Friend distriFiller() As Decimal

    Friend allModules As New List(Of clsModular)
    Friend allLinks As New List(Of clsLink)
    Friend displayedModules As New List(Of clsModular)

    Dim modInput As New clsModular(clsModular.enumModularType.Input)
    Dim modOutput As New clsModular(clsModular.enumModularType.Output)
    Dim modCriticalMachine As clsModular

    Sub Main()

        initDistributions()

        'TestDistributions()

        Execute_project()

        Console.ReadLine()

    End Sub

    Public Sub testDistributions()

        For j = 1 To 10
            'Create an array to stroe experiences
            Dim a As New List(Of Decimal)

            'Generate 100 experiences
            For i = 1 To 1000
                a.Add(NormalMTBF(100))
            Next

            'Display the experiences
            ShowDistributions(a)

            Console.ReadLine()
        Next j

    End Sub

    ''' <summary>
    ''' Show in the console the distribution of a list of values
    ''' </summary>
    ''' <param name="_input">List of values to show</param>
    ''' <remarks></remarks>
    Public Sub ShowDistributions(ByVal _input As List(Of Decimal))

        Dim distri As New SortedDictionary(Of Long, Integer)

        Dim result As Long
        Dim patamar As Long
        Dim mean As Decimal = 0
        Dim division As Long = 10

        'Classify experiences by interval
        For Each oneMT As Decimal In _input
            result = oneMT
            patamar = Math.DivRem(result, division, 0) + 1
            If Not distri.ContainsKey(patamar) Then
                distri.Add(patamar, 1)
            Else
                distri(patamar) += 1
            End If
            mean += result
        Next

        'Calculate the average experience value
        mean = mean / _input.Count

        'Display results
        Console.WriteLine("Mean = " & displayDecimal(mean))

        For i = 1 To distri.Keys.Max
            If distri.ContainsKey(i) Then
                Console.Write((i * division).ToString & " (" & distri(i) & ") ")
                For j = 1 To Math.Min(distri(i), 50)
                    Console.Write("=")
                Next j
            Else
                Console.Write((i * division).ToString & " (0) ")
            End If
            Console.WriteLine()
        Next

        Console.WriteLine()

    End Sub

    Public Sub Execute_project()

        'Project_Waters5L()
        'Project_Oil1L()
        'Project_PerfectLine()
        'Project_TestReturn()
        Project_Cosmetic200mL()

        'modInput.Content = 30000

        modOutput.defineRoutes(allModules)

        For Each onemodule As clsModular In allModules
            onemodule.init()
        Next

        Dim i As Integer = 3600 * 8
        Dim i_rem As Integer = 1
        While i > 0

            For Each onelink As clsLink In allLinks
                onelink.resetPotential()
            Next
            For Each onemodule As clsModular In allModules
                onemodule.checkPotential_pass1()
            Next
            For Each onemodule As clsModular In allModules
                onemodule.checkPotential_pass2()
            Next
            For Each onemodule As clsModular In allModules
                onemodule.run()
            Next

            'Console.Clear()
            'For Each onemodule As clsModular In displayedModules
            '    onemodule.details(True)
            'Next
            'System.Threading.Thread.Sleep(1000)

            Math.DivRem(i, 3600, i_rem)
            If i_rem = 0 Then
                modCriticalMachine.details(False, False, False, True)
            End If

            i -= 1

        End While

        Console.Clear()
        For Each onemodule As clsModular In displayedModules
            onemodule.details(False, False, False, True)
        Next

    End Sub

    Public Sub Project_Waters5L()


        Dim modSbo As New clsModular("Combi", 5000, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, 0.95, 179)
        modCriticalMachine = modSbo
        'modSbo.rejectrate = 0.005

        Dim modTrp1 As New clsModular(clsModular.enumModularType.Transport)
        modTrp1.SetAccumulator(70, 72, modSbo.speed * 1.2)

        Dim modColoc As New clsModular("Coloc. alças", 5000 * 1.2, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, 0.98, 57)

        Dim modTrp2 As New clsModular(clsModular.enumModularType.Transport)

        Dim modRot As New clsModular("Rotuladora", 5000 * 1.22, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, 0.943, 44)
        'modRot.rejectrate = 0.005

        Dim modTrp3 As New clsModular(clsModular.enumModularType.Transport)

        Dim modWrap As New clsModular("Wrapper", 5000 * 1.24, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, 0.937, 45)
        modWrap.unitCycle = 2

        Dim modTrp4 As New clsModular(clsModular.enumModularType.Transport)

        Dim modPal As New clsModular("Pal", 5000 * 1.26, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, 0.986, 56)
        modPal.unitCycle = 6

        allLinks.Add(New clsLink(modInput, modSbo))
        allLinks.Add(New clsLink(modSbo, modTrp1))
        allLinks.Add(New clsLink(modTrp1, modColoc))
        allLinks.Add(New clsLink(modColoc, modTrp2))
        allLinks.Add(New clsLink(modTrp2, modRot))
        allLinks.Add(New clsLink(modRot, modTrp3))
        allLinks.Add(New clsLink(modTrp3, modWrap))
        allLinks.Add(New clsLink(modWrap, modTrp4))
        allLinks.Add(New clsLink(modTrp4, modPal))
        allLinks.Add(New clsLink(modPal, modOutput))

        displayedModules.Add(modSbo)
        displayedModules.Add(modTrp1)
        displayedModules.Add(modColoc)
        displayedModules.Add(modRot)
        displayedModules.Add(modWrap)
        displayedModules.Add(modPal)
        displayedModules.Add(modOutput)

    End Sub

    Public Sub Project_Cosmetic200mL()

        Dim modUnscr As New clsModular("Unscrambler", 204, clsModular.enumSpeedUnit.per_min, clsModular.enumParameters.Eff_MTTR, 0.9866, 53)

        Dim modTrp1 As New clsModular(clsModular.enumModularType.Transport)
        modTrp1.SetAccumulator(5 * 204 / 60, 5, 204 / 60)

        Dim modFiller As New clsModular("Filler", 200, clsModular.enumSpeedUnit.per_min, clsModular.enumParameters.Eff_MTTR, 0.9479, 27)
        modCriticalMachine = modFiller

        Dim modTrp2 As New clsModular(clsModular.enumModularType.Transport)
        'modTrp2.SetAccumulator(5 * 204 / 60, 5, 204 / 60)
        modTrp2.SetAccumulator((5 + 90) * 204 / 60, 10, 204 / 60)

        Dim modLabel As New clsModular("Labeler", 204, clsModular.enumSpeedUnit.per_min, clsModular.enumParameters.Eff_MTTR, 0.9642, 60)

        Dim modTrp3 As New clsModular(clsModular.enumModularType.Transport)
        modTrp3.SetAccumulator(5 * 204 / 60, 5, 204 / 60)

        Dim modPack As New clsModular("Packer", 204, clsModular.enumSpeedUnit.per_min, clsModular.enumParameters.Eff_MTTR, 0.9168, 68)
        modPack.unitCycle = 6

        allLinks.Add(New clsLink(modInput, modUnscr))
        allLinks.Add(New clsLink(modUnscr, modTrp1))
        allLinks.Add(New clsLink(modTrp1, modFiller))
        allLinks.Add(New clsLink(modFiller, modTrp2))
        allLinks.Add(New clsLink(modTrp2, modLabel))
        allLinks.Add(New clsLink(modLabel, modTrp3))
        allLinks.Add(New clsLink(modTrp3, modPack))
        allLinks.Add(New clsLink(modPack, modOutput))

        displayedModules.Add(modUnscr)
        displayedModules.Add(modFiller)
        displayedModules.Add(modTrp2)
        displayedModules.Add(modLabel)
        displayedModules.Add(modPack)
        displayedModules.Add(modOutput)

    End Sub


    Public Sub Project_PerfectLine()

        Dim ProdPerHour As Decimal = 50000
        Dim ProdPerMin As Decimal = ProdPerHour / 60
        Dim EffAll As Decimal = 0.98
        Dim MTTRAll As Decimal = 60
        Dim Accu As Decimal = 1.5

        Dim modSBO As New clsModular("Blower", ProdPerHour * 1.1, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, EffAll, MTTRAll)

        Dim modTrp1 As New clsModular(clsModular.enumModularType.Transport)
        modTrp1.Content_max = Accu * ProdPerMin

        Dim modFiller As New clsModular("Filler", ProdPerHour, clsModular.enumSpeedUnit.per_Hour)
        modCriticalMachine = modFiller

        Dim modTrp2 As New clsModular(clsModular.enumModularType.Transport)
        modTrp2.Content_max = Accu * ProdPerMin

        Dim modLabel As New clsModular("Labeler", ProdPerHour * 1.1, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, EffAll, MTTRAll)

        Dim modTrp3 As New clsModular(clsModular.enumModularType.Transport)
        modTrp3.Content_max = Accu * ProdPerMin

        Dim modPacker As New clsModular("Packer", ProdPerHour * 1.2, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, EffAll, MTTRAll)
        modPacker.unitCycle = 6

        Dim modTrp4 As New clsModular(clsModular.enumModularType.Transport)
        modTrp4.Content_max = Accu * ProdPerMin

        Dim modPal As New clsModular("Pal", ProdPerHour * 1.3, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, EffAll, MTTRAll)
        modPal.unitCycle = 24

        allLinks.Add(New clsLink(modInput, modSbo))
        allLinks.Add(New clsLink(modSbo, modTrp1))
        allLinks.Add(New clsLink(modTrp1, modFiller))
        allLinks.Add(New clsLink(modFiller, modTrp2))
        allLinks.Add(New clsLink(modTrp2, modLabel))
        allLinks.Add(New clsLink(modLabel, modTrp3))
        allLinks.Add(New clsLink(modTrp3, modPacker))
        allLinks.Add(New clsLink(modPacker, modTrp4))
        allLinks.Add(New clsLink(modTrp4, modPal))
        allLinks.Add(New clsLink(modPal, modOutput))

        displayedModules.Add(modInput)
        displayedModules.Add(modSBO)

        displayedModules.Add(modTrp1)
        displayedModules.Add(modFiller)
        displayedModules.Add(modTrp2)

        displayedModules.Add(modLabel)
        displayedModules.Add(modPacker)
        displayedModules.Add(modPal)
        displayedModules.Add(modOutput)


    End Sub

    Public Sub Project_Oil1L()

        Dim prodPerSec As Decimal = 20400 / 3600

        Dim modOrientador As New clsModular("Unscrambler", 18400, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, 0.884, 13)
        'Dim modOrientador As New clsModular("Orientador_novo", 21480, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, 0.95, 40)

        Dim modTrp1 As New clsModular(clsModular.enumModularType.Transport)
        modTrp1.SetAccumulator(340, 60, prodPerSec)

        Dim modFiller As New clsModular("Filler", 19200, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, 0.746, 10)
        'Dim modFiller As New clsModular("Filler_Corrected", 20400, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, 0.988, 10)
        modCriticalMachine = modFiller

        Dim modTrp2 As New clsModular(clsModular.enumModularType.Transport)
        modTrp2.SetAccumulator(6 * prodPerSec, 5, prodPerSec)
        'modTrp2.SetAccumulator(6 * prodPerSec + 458, 5, prodPerSec)

        Dim modLabeler As New clsModular("Labeler", 27600, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, 0.887, 60)
        'Dim modLabeler As New clsModular("Labeler_Corrected", 27600, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, 0.923, 60)

        Dim modTrp3 As New clsModular(clsModular.enumModularType.Transport)

        Dim modDiv As New clsModular("Divider", 27600, clsModular.enumSpeedUnit.per_Hour)

        Dim modTrp4 As New clsModular(clsModular.enumModularType.Transport)
        modTrp4.SetAccumulator(35 * prodPerSec, 15, prodPerSec)

        Dim modWrapper As New clsModular("Wrapper", 23400, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, 0.943, 31)
        modWrapper.unitCycle = 24

        Dim modTrp5 As New clsModular(clsModular.enumModularType.Transport)
        modTrp5.SetAccumulator(31 * prodPerSec, 10, prodPerSec)
        'modTrp5.SetAccumulator(31 * prodPerSec + 40 * prodPerSec, 10, prodPerSec)


        Dim modPal As New clsModular("Pal_Original", 44200, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, 0.809, 182)
        'Dim modPal As New clsModular("Pal_GeboCermex", 24600, clsModular.enumSpeedUnit.per_Hour, clsModular.enumParameters.Eff_MTTR, 0.98, 42)
        modPal.unitCycle = 24


        allLinks.Add(New clsLink(modInput, modOrientador))
        allLinks.Add(New clsLink(modOrientador, modTrp1))
        allLinks.Add(New clsLink(modTrp1, modFiller))
        allLinks.Add(New clsLink(modFiller, modTrp2))
        allLinks.Add(New clsLink(modTrp2, modLabeler))
        allLinks.Add(New clsLink(modLabeler, modTrp3))
        allLinks.Add(New clsLink(modTrp3, modDiv))
        allLinks.Add(New clsLink(modDiv, modTrp4))
        allLinks.Add(New clsLink(modTrp4, modWrapper))
        allLinks.Add(New clsLink(modWrapper, modTrp5))
        allLinks.Add(New clsLink(modTrp5, modPal))
        allLinks.Add(New clsLink(modPal, modOutput))

        displayedModules.Add(modOrientador)
        displayedModules.Add(modTrp1)
        displayedModules.Add(modFiller)
        displayedModules.Add(modTrp2)
        displayedModules.Add(modLabeler)
        displayedModules.Add(modWrapper)
        displayedModules.Add(modPal)
        displayedModules.Add(modOutput)

    End Sub

    Public Sub Project_TestReturn()

        modInput.speed = 100

        Dim modGrp As New clsModular("Grouper", 300, clsModular.enumSpeedUnit.per_Sec)
        modGrp.InOutputs_Combining = False

        Dim modTrp As New clsModular(clsModular.enumModularType.Transport)
        modTrp.SetAccumulator(2000, 10, 200)

        Dim modDiv As New clsModular("Divider", 300, clsModular.enumSpeedUnit.per_Sec)
        modDiv.InOutputs_Combining = False

        Dim modMach As New clsModular("Critical Machine", 150, clsModular.enumSpeedUnit.per_Sec, clsModular.enumParameters.Eff_MTBF, 0.9, 30)

        Dim modRetTrp As New clsModular(clsModular.enumModularType.Transport)
        modRetTrp.SetAccumulator(2000, 10, 200)
        modRetTrp.Content_entering = 4000

        Dim modRetMach As New clsModular("Return Machine", 150, clsModular.enumSpeedUnit.per_Sec, clsModular.enumParameters.Eff_MTBF, 0.8, 30)

        modCriticalMachine = modMach

        allLinks.Add(New clsLink(modInput, modGrp))
        allLinks.Add(New clsLink(modGrp, modTrp))
        allLinks.Add(New clsLink(modTrp, modDiv))
        allLinks.Add(New clsLink(modDiv, modMach))
        allLinks.Add(New clsLink(modDiv, modRetTrp))
        allLinks.Add(New clsLink(modRetTrp, modRetMach))
        allLinks.Add(New clsLink(modRetMach, modGrp))
        allLinks.Add(New clsLink(modMach, modOutput))

        displayedModules.Add(modGrp)
        displayedModules.Add(modTrp)
        displayedModules.Add(modDiv)
        displayedModules.Add(modMach)
        displayedModules.Add(modOutput)
        displayedModules.Add(modRetTrp)
        displayedModules.Add(modRetMach)


    End Sub

    Friend Class clsLink

        Friend potential As Decimal = 0

        Friend upstream As clsModular
        Friend downstream As clsModular

        Friend state As enumStates = enumStates.Normal

        Friend Enum enumStates As Integer
            Full
            Empty
            Normal
        End Enum

        Public Sub New(ByRef _upstream As clsModular, ByRef _downstream As clsModular)
            upstream = _upstream
            downstream = _downstream
            upstream.Outputs.Add(Me)
            downstream.Inputs.Add(Me)
        End Sub

        Public Sub resetPotential()
            potential = 0
            'Flux tendus
            upstream.alreadyPlannedPotential = 0
            downstream.alreadyPlannedPotential = 0
            'Accumulators
            upstream.RunAccumulator()
        End Sub

    End Class

    Friend Class clsAccu

        ''' <summary>
        ''' One Cell (timing separation for product distribution)
        ''' </summary>
        ''' <remarks></remarks>
        Private Class clsCell

            Friend previousCell As clsCell = Nothing

            Public content As Decimal = 0

            'Receive production at a cell and transfer excess to previous cells
            Public Sub ReceiveProd(ByVal _quantity As Decimal, ByVal _max_by_cell As Decimal)

                If Me.content + _quantity + EPSILON > _max_by_cell And Not IsNothing(previousCell) Then
                    'Accumulate max and forward to previous cell
                    _quantity -= _max_by_cell - Me.content
                    Me.content = _max_by_cell
                    previousCell.ReceiveProd(_quantity, _max_by_cell)
                Else
                    'Accumulate quantity
                    Me.content += _quantity
                End If

            End Sub

            'Count quantity accumulated in previous cells recursively
            Public Function Quantity() As Decimal

                If Not IsNothing(previousCell) Then
                    Return Me.content + previousCell.Quantity
                Else
                    Return Me.content
                End If

            End Function

        End Class

        Dim linkedModular As clsModular

        Dim FirstCell As clsCell 'Firsts products to enter
        Dim LastCell As clsCell 'Firsts products to exit
        Dim LastCellPoped As Boolean = False

        Dim MaxContent As Decimal
        Dim MaxCells As Decimal
        Dim Content_by_cell As Decimal

        ''' <summary>
        ''' Create an accu with the specified parameters
        ''' </summary>
        ''' <param name="_max_content">Total number of product (capacity)</param>
        ''' <param name="_transit_time">Total time (in time units) between input and output</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal _max_content As Decimal, ByVal _transit_time As Integer, ByRef _modular As clsModular)
            linkedModular = _modular
            MaxContent = _max_content
            MaxCells = _transit_time
            Content_by_cell = MaxContent / MaxCells

            'Create first unique 
            FirstCell = New clsCell()
            LastCell = FirstCell

            'Create additional cells and link them
            For i = 2 To MaxCells
                Dim newCell As New clsCell
                FirstCell.previousCell = newCell
                FirstCell = newCell
            Next
        End Sub

        ''' <summary>
        ''' Return or change Max Content of the Accu
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Max_Content() As Decimal
            Get
                Return MaxContent
            End Get
            Set(ByVal _max_content As Decimal)
                MaxContent = _max_content
                Content_by_cell = MaxContent / MaxCells
            End Set
        End Property

        'Prepare the next cycle to be executed
        Public Sub init()

            'Reset last cell poped indicator
            LastCellPoped = False

            'Add a new cell at the biggining
            Dim newCell As New clsCell
            FirstCell.previousCell = newCell
            FirstCell = newCell

            'Remove last cell and distribute it content
            LastCell.previousCell.ReceiveProd(LastCell.content, Content_by_cell)
            LastCell = LastCell.previousCell

        End Sub

        'Read number of prod that can be delivered next turn
        Public ReadOnly Property OutputPotential() As Decimal
            Get
                Return LastCell.content
            End Get
        End Property

        'Read number of prod that can enter next turn
        Public ReadOnly Property InputPotential() As Decimal
            Get
                Return Math.Max(0, Content_by_cell - FirstCell.content)
            End Get
        End Property

        'Return total number of products in the accu
        Public ReadOnly Property TotalContent() As Decimal
            Get
                Return LastCell.Quantity
            End Get
        End Property

        'Return the total quantity that is available and remove it from last cell content
        Public Function Pop(ByVal _quantity As Decimal) As Decimal

            If linkedModular.InOutputs_Combining Then
                'On combining equipment
                Pop = Math.Min(LastCell.content, _quantity)
                LastCell.content -= Pop
            Else
                'On assembling equipment
                Pop = _quantity 'We don't check the last cell content and assume that the potential is right
                If Not LastCellPoped Then LastCell.content -= Pop
                LastCellPoped = True
            End If

        End Function

        'Put content into the firs cell
        Public Sub Put(ByVal _quantity As Decimal)
            FirstCell.content += _quantity
        End Sub

        'Get entering content
        Public ReadOnly Property EnteringContent() As Decimal
            Get
                If Not IsNothing(LastCell.previousCell) Then
                    Return LastCell.previousCell.Quantity
                Else
                    Return 0
                End If
            End Get
        End Property


    End Class

    Friend Class clsModular

        Friend name As String
        Friend modType As enumModularType
        Friend speed As Decimal = 0
        Friend unitCycle As Decimal = 0

        Friend acumulatedPotential As Decimal = 0
        Friend alreadyPlannedPotential As Decimal = 0
        Friend rejectedPotential As Decimal = 0
        Friend injectedPotential As Decimal = 0

        Friend rejectrate As Decimal = 0
        Friend injectrate As Decimal = 0

        Friend MTTR As Decimal = 0
        Friend MTBF As Decimal = Decimal.MaxValue

        Friend MTTR_next As Decimal = 0
        Friend MTBF_next As Decimal = Decimal.MaxValue

        Friend Inputs As New List(Of clsLink)
        Friend Outputs As New List(Of clsLink)

        Private Content_Accu As New clsAccu(0, 1, Me)

        Friend statsState As New Dictionary(Of enumStates, Decimal)
        Friend statsCounts As New Dictionary(Of enumType, Decimal)

        Friend statsMTBF As New List(Of Decimal)
        Friend statsMTTR As New List(Of Decimal)

        Friend Initialized As Boolean = False

        ''' <summary>
        ''' Combining 1+1=2 or Assembling 1+1=1
        ''' </summary>
        ''' <remarks></remarks>
        Friend InOutputs_Combining As Boolean = True 'Or Assembling inputs/outputs

        Friend Enum enumStates As Integer
            Stopped
            Running
            WaitingInput
            WaitingOutput
        End Enum

        Friend Enum enumType As Integer
            Processed
            Inject
            Reject
        End Enum

        Friend Enum enumModularType As Integer
            Input
            Output
            Transport
            Machine
        End Enum

        Public Sub New(ByVal _type As enumModularType)
            speed = Decimal.MaxValue
            modType = _type
            Select Case _type
                Case enumModularType.Input
                    name = "Input"
                    Content_Accu.Put(Decimal.MaxValue)
                Case enumModularType.Output
                    name = "Output"
                Case enumModularType.Transport
                    LastTRPNum += 1
                    name = "Transport_" & LastTRPNum
                Case enumModularType.Machine
                    LastTRPNum += 1
                    name = "Machine_" & LastTRPNum
            End Select

        End Sub

        Public Sub New(ByVal _name As String, ByVal _speed As Decimal, ByVal _unit As enumSpeedUnit)
            name = _name
            modType = enumModularType.Machine
            Select Case _unit
                Case enumSpeedUnit.per_Sec
                    speed = _speed
                Case enumSpeedUnit.per_min
                    speed = _speed / 60
                Case enumSpeedUnit.per_Hour
                    speed = _speed / 3600
                Case enumSpeedUnit.sec_per_Cycle
                    speed = 1 / _speed
            End Select
        End Sub

        Friend Enum enumSpeedUnit As Integer
            per_Sec
            per_min
            per_Hour
            sec_per_Cycle
        End Enum

        Friend Enum enumParameters As Integer
            MTTR_MTBF
            Eff_MTBF
            Eff_MTTR
        End Enum

        Public Sub New(ByVal _name As String, ByVal _speed As Decimal, ByVal _unit As enumSpeedUnit, ByVal _params As enumParameters, ByVal _MTTR_or_Eff As Decimal, ByVal _MTBF_or_MTTR As Decimal)
            name = _name
            modType = enumModularType.Machine
            Select Case _unit
                Case enumSpeedUnit.per_Sec
                    speed = _speed
                Case enumSpeedUnit.per_min
                    speed = _speed / 60
                Case enumSpeedUnit.per_Hour
                    speed = _speed / 3600
                Case enumSpeedUnit.sec_per_Cycle
                    speed = 1 / _speed
            End Select

            Select Case _params
                Case enumParameters.MTTR_MTBF
                    MTTR = _MTTR_or_Eff
                    MTBF = _MTBF_or_MTTR
                Case enumParameters.Eff_MTTR
                    MTTR = _MTBF_or_MTTR
                    MTBF = _MTBF_or_MTTR / ((1 / _MTTR_or_Eff) - 1)
                Case enumParameters.Eff_MTBF
                    MTBF = _MTBF_or_MTTR
                    MTTR = _MTBF_or_MTTR * ((1 / _MTTR_or_Eff) - 1)
            End Select

        End Sub

        Public Sub init()

            If Initialized Then Return

            If Not MTTR = 0 Then
                MTTR_next = NormalMTTR(MTTR)
            Else
                MTTR_next = 0
            End If

            If Not MTBF = Decimal.MaxValue Then
                MTBF_next = NormalMTBF(MTBF)
            Else
                MTBF_next = Decimal.MaxValue
            End If

            Initialized = True

        End Sub

        Public Sub defineRoutes(ByRef _modules As List(Of clsModular))

            If Not _modules.Contains(Me) Then _modules.Add(Me)
            For Each oneInputLink As clsLink In Inputs

                If Not _modules.Contains(oneInputLink.upstream) Then
                    oneInputLink.upstream.defineRoutes(_modules)
                End If
            Next

        End Sub

        ''' <summary>
        ''' Check the potential of each link by evaluating from outfeed and going upstream
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub checkPotential_pass1()

            'Manage time before breackdown and breackdown duration
            If MTBF < MAX_TMBF And Not MTTR = 0 Then
                If MTTR_next < 1 And MTBF_next < 1 Then
                    'Renew MTBF and MTTR
                    MTTR_next = NormalMTTR(MTTR)
                    MTBF_next = NormalMTBF(MTBF)
                    statsMTBF.Add(MTBF_next)
                    statsMTTR.Add(MTTR_next)
                End If
            End If

            Dim randomizedInputs As New SortedList(Of Single, clsLink)
            For Each one_inputlink As clsLink In Inputs
                Dim key As Single = Rnd()
                While randomizedInputs.ContainsKey(key)
                    key = Rnd()
                End While
                randomizedInputs.Add(Rnd, one_inputlink)
            Next

            If InOutputs_Combining Then 'Outputs are combined I tot. = I1 + I1 + ...

                For Each one_inputlink As clsLink In randomizedInputs.Values

                    one_inputlink.state = clsLink.enumStates.Normal
                    Dim currentpotential As Decimal = Decimal.MaxValue

                    'Restriction by Upstream equipment content size
                    currentpotential = Math.Min(currentpotential, one_inputlink.upstream.OutputPotential - one_inputlink.upstream.alreadyPlannedPotential)
                    'Restriction by Own speed
                    If speed < MAX_SPEED Then currentpotential = Math.Min(currentpotential, Me.speed / Inputs.Count)
                    'Restriction by Upstream equipment speed
                    currentpotential = Math.Min(currentpotential, one_inputlink.upstream.speed / IIf(one_inputlink.upstream.InOutputs_Combining, one_inputlink.upstream.Outputs.Count, 1))

                    'Restriction by Downstream equipment
                    Dim capacity As Decimal = 0
                    'If contentMax is big enough to be considered as accumulation
                    If Me.Content_Max > IIf(Me.speed < MAX_SPEED, Me.speed, 0) Then
                        'Restriction by Accumulation potential
                        'With one_inputlink.upstream
                        '    capacity = Math.Max(Me.Content_Max, Math.Min(Me.speed / Inputs.Count, .speed / .Outputs.Count)) + alreadyPlannedPotential
                        'End With
                        'currentpotential = Math.Min(currentpotential, capacity - Me.Content - Me.Content_entering)
                        currentpotential = Math.Min(currentpotential, InputPotential)

                    ElseIf Not Me.modType = enumModularType.Output Then

                        If speed < MAX_SPEED Then
                            capacity = IIf(unitCycle = 0, Me.speed / Inputs.Count, unitCycle + Me.speed / Inputs.Count)
                            capacity += alreadyPlannedPotential
                        Else
                            With one_inputlink.upstream
                                If .speed < MAX_SPEED Then
                                    'capacity = IIf(.unitCycle = 0, .speed / IIf(.InOutputs_Combining, .Outputs.Count, 1), .unitCycle + .speed / IIf(.InOutputs_Combining, .Outputs.Count, 1))
                                    capacity = IIf(.unitCycle = 0, .speed, .unitCycle + .speed)
                                Else
                                    capacity = .OutputPotential
                                End If
                                capacity += alreadyPlannedPotential
                                capacity = capacity / IIf(.InOutputs_Combining, .Outputs.Count, 1)
                            End With
                        End If
                        'Restriction by continuous flow rupture
                        currentpotential = Math.Min(currentpotential, capacity - Me.OutputPotential)

                    End If

                    'Define the current state if Build back or Starvation
                    If one_inputlink.upstream.OutputPotential - one_inputlink.upstream.alreadyPlannedPotential < EPSILON Then
                        one_inputlink.state = clsLink.enumStates.Empty
                    ElseIf currentpotential < EPSILON Then
                        currentpotential = 0
                        one_inputlink.state = clsLink.enumStates.Full
                    End If

                    'Define the breakdown state
                    If MTTR_next > 1 And MTBF_next < 1 Then
                        currentpotential = 0
                        one_inputlink.state = clsLink.enumStates.Full
                    End If

                    'We consider the link current potential calculated
                    one_inputlink.potential = currentpotential

                    'We make reservation of the products only if the machine is not assembling them
                    '(for assembling machine, every output will receive the same amount of products)
                    If one_inputlink.upstream.InOutputs_Combining Then
                        one_inputlink.upstream.alreadyPlannedPotential += currentpotential
                    End If

                Next

            Else 'Outputs are assembled/separated, I tot. = min(O1, O2, ...) 

                Dim totalcurrentpotential As Decimal = Decimal.MaxValue

                For Each one_inputlink As clsLink In randomizedInputs.Values
                    Dim currentpotential As Decimal = Decimal.MaxValue
                    one_inputlink.state = clsLink.enumStates.Normal

                    'Restriction by Upstream equipment content size
                    currentpotential = Math.Min(currentpotential, one_inputlink.upstream.OutputPotential - one_inputlink.upstream.alreadyPlannedPotential)
                    'Restriction by Own speed
                    If speed < MAX_SPEED Then currentpotential = Math.Min(currentpotential, Me.speed)
                    'Restriction by Upstream equipment speed
                    currentpotential = Math.Min(currentpotential, one_inputlink.upstream.speed / IIf(one_inputlink.upstream.InOutputs_Combining, one_inputlink.upstream.Outputs.Count, 1))

                    'Restriction by Downstream equipment
                    Dim capacity As Decimal = 0
                    'If contentMax is big enough to be considered as accumulation
                    If Me.Content_Max > IIf(Me.speed < MAX_SPEED, Me.speed, 0) Then
                        'Restriction by Accumulation potential
                        currentpotential = Math.Min(currentpotential, InputPotential)

                    ElseIf Not Me.modType = enumModularType.Output Then

                        If speed < MAX_SPEED Then
                            capacity = IIf(unitCycle = 0, Me.speed, unitCycle + Me.speed)
                            capacity += alreadyPlannedPotential
                        Else
                            With one_inputlink.upstream
                                If .speed < MAX_SPEED Then
                                    'capacity = IIf(.unitCycle = 0, .speed / IIf(.InOutputs_Combining, .Outputs.Count, 1), .unitCycle + .speed / IIf(.InOutputs_Combining, .Outputs.Count, 1))
                                    capacity = IIf(.unitCycle = 0, .speed, .unitCycle + .speed)
                                Else
                                    capacity = .OutputPotential
                                End If
                                capacity += alreadyPlannedPotential
                                capacity = capacity / .Outputs.Count
                            End With
                        End If
                        'Restriction by continuous flow rupture
                        currentpotential = Math.Min(currentpotential, capacity - Me.OutputPotential)

                    End If

                    'Define the current state if Build back or Starvation
                    If one_inputlink.upstream.OutputPotential - one_inputlink.upstream.alreadyPlannedPotential < EPSILON Then
                        one_inputlink.state = clsLink.enumStates.Empty
                    ElseIf currentpotential < EPSILON Then
                        currentpotential = 0
                        one_inputlink.state = clsLink.enumStates.Full
                    End If

                    'Define the breakdown state
                    If MTTR_next > 1 And MTBF_next < 1 Then
                        currentpotential = 0
                        one_inputlink.state = clsLink.enumStates.Full
                    End If

                    'We consider the link current potential calculated
                    one_inputlink.potential = currentpotential

                    'We make reservation of the products only if the machine is not assembling them
                    '(for assembling machine, every output will receive the same amount of products)
                    If one_inputlink.upstream.InOutputs_Combining Then
                        one_inputlink.upstream.alreadyPlannedPotential += currentpotential
                    End If

                    'Define effective potential according to previously processed links
                    totalcurrentpotential = Math.Min(currentpotential, totalcurrentpotential)
                Next

                'Report the effetive potential on every link
                For Each one_inputlink As clsLink In randomizedInputs.Values
                    one_inputlink.potential = totalcurrentpotential
                    'Change the link state to report that the output equipment is not ready
                    If totalcurrentpotential = 0 And one_inputlink.state = clsLink.enumStates.Normal Then
                        one_inputlink.state = clsLink.enumStates.Full
                    End If
                Next

            End If

        End Sub

        ''' <summary>
        ''' Check the potential on assembling machines, balancing outputs, and effective presence of products
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub checkPotential_pass2()

            'TODO: Check effective presence of products

            If Me.InOutputs_Combining Then 'Outputs are combined I tot. = I1 + I1 + ...

                'NOP

            Else 'Outputs are assembled/separated, I tot. = min(O1, O2, ...)

                Dim balancedPotential As Decimal = Decimal.MaxValue

                'Check the potential on eahc output
                For Each one_outputlink As clsLink In Outputs
                    balancedPotential = Math.Min(balancedPotential, one_outputlink.potential)
                Next

                'Apply on every link
                For Each one_outputlink As clsLink In Outputs
                    one_outputlink.potential = balancedPotential
                Next

            End If

        End Sub


        Public Sub run()

            'If there is no breakdown, the equipment is processing
            If MTBF_next > 0 Then

                If Not statsCounts.ContainsKey(enumType.Processed) Then statsCounts.Add(enumType.Processed, 0)

                Dim enteringProducts As Decimal = 0
                Dim rejectedProducts As Decimal = 0
                Dim injectedProducts As Decimal = 0

                Dim upstreamwait As Boolean = True

                If InOutputs_Combining Then 'Outputs are combined I tot. = I1 + I1 + ...

                    For Each one_inputlink As clsLink In Inputs

                        'if the link is declared empty, we don't process it
                        If Not one_inputlink.state = clsLink.enumStates.Empty Then

                            'Calculate the number of entering products for the link
                            If unitCycle > 0 Then 'Cyclical behaviour
                                acumulatedPotential += one_inputlink.upstream.Content_Accu.Pop(one_inputlink.potential)
                                enteringProducts = Decimal.Truncate(EPSILON + acumulatedPotential / unitCycle) * unitCycle
                                acumulatedPotential -= enteringProducts
                            Else 'Continuous behaviour
                                enteringProducts = one_inputlink.upstream.Content_Accu.Pop(one_inputlink.potential)
                            End If

                            'Calculate the rejected products at upstream equipment
                            If one_inputlink.upstream.rejectrate > 0 Then
                                If Not one_inputlink.upstream.statsCounts.ContainsKey(enumType.Reject) Then
                                    one_inputlink.upstream.statsCounts.Add(enumType.Reject, 0)
                                End If
                                If unitCycle > 0 Then 'Cyclical behaviour
                                    rejectedPotential += enteringProducts * one_inputlink.upstream.rejectrate
                                    rejectedProducts = Decimal.Truncate(EPSILON + rejectedPotential / unitCycle) * unitCycle
                                    one_inputlink.upstream.statsCounts(enumType.Reject) += rejectedProducts
                                    rejectedPotential -= rejectedProducts
                                Else 'Continuous behaviour
                                    rejectedProducts = enteringProducts * one_inputlink.upstream.rejectrate
                                    one_inputlink.upstream.statsCounts(enumType.Reject) += rejectedProducts
                                End If
                            End If

                            'Put the products in internal accumulator
                            Me.Content_Accu.Put(enteringProducts - rejectedProducts)
                            statsCounts(enumType.Processed) += enteringProducts - rejectedProducts

                            'Calculate the secondary product injection rate
                            If injectrate > 0 Then
                                If Not statsCounts.ContainsKey(enumType.Inject) Then statsCounts.Add(enumType.Inject, 0)
                                If unitCycle > 0 Then
                                    injectedPotential += injectrate * (enteringProducts - rejectedProducts)
                                    injectedProducts = Decimal.Truncate(EPSILON + injectedPotential / unitCycle) * unitCycle
                                    statsCounts(enumType.Inject) += injectedProducts
                                    injectedPotential -= injectedProducts
                                Else
                                    injectedProducts = injectrate * (enteringProducts - rejectedProducts)
                                    statsCounts(enumType.Inject) += injectedProducts
                                End If
                            End If

                            upstreamwait = False 'At least one link has products

                        End If

                    Next 'next link to be processed

                Else 'Outputs are assembled/separated, I tot. = min(O1, O2, ...) 

                    'Presupposing maximum entering products
                    Dim totalPotential As Decimal = Decimal.MaxValue

                    'For each link, we check the best potential for combining
                    For Each one_inputlink As clsLink In Inputs
                        'if the link is declared empty, we don't process it
                        If Not one_inputlink.state = clsLink.enumStates.Empty Then
                            totalPotential = Math.Min(one_inputlink.potential, totalPotential)
                        Else
                            totalPotential = 0
                        End If
                    Next

                    'If potential is not null
                    If totalPotential > EPSILON Then

                        For Each one_inputlink As clsLink In Inputs

                            'Calculate the number of entering products for the link
                            If unitCycle > 0 Then 'Cyclical behaviour
                                acumulatedPotential += one_inputlink.upstream.Content_Accu.Pop(totalPotential)
                                enteringProducts = Decimal.Truncate(EPSILON + acumulatedPotential / unitCycle) * unitCycle
                                acumulatedPotential -= enteringProducts
                            Else 'Continuous behaviour
                                enteringProducts = one_inputlink.upstream.Content_Accu.Pop(totalPotential)
                            End If

                            'Calculate the rejected products at upstream equipment
                            If one_inputlink.upstream.rejectrate > 0 Then
                                If Not one_inputlink.upstream.statsCounts.ContainsKey(enumType.Reject) Then
                                    one_inputlink.upstream.statsCounts.Add(enumType.Reject, 0)
                                End If
                                If unitCycle > 0 Then 'Cyclical behaviour
                                    rejectedPotential += enteringProducts * one_inputlink.upstream.rejectrate
                                    rejectedProducts = Decimal.Truncate(EPSILON + rejectedPotential / unitCycle) * unitCycle
                                    one_inputlink.upstream.statsCounts(enumType.Reject) += rejectedProducts
                                    rejectedPotential -= rejectedProducts
                                Else 'Continuous behaviour
                                    rejectedProducts = enteringProducts * one_inputlink.upstream.rejectrate
                                    one_inputlink.upstream.statsCounts(enumType.Reject) += rejectedProducts
                                End If
                            End If

                            'Calculate the secondary product injection rate
                            If injectrate > 0 Then
                                If Not statsCounts.ContainsKey(enumType.Inject) Then statsCounts.Add(enumType.Inject, 0)
                                If unitCycle > 0 Then
                                    injectedPotential += injectrate * (enteringProducts - rejectedProducts)
                                    injectedProducts = Decimal.Truncate(EPSILON + injectedPotential / unitCycle) * unitCycle
                                    statsCounts(enumType.Inject) += injectedProducts
                                    injectedPotential -= injectedProducts
                                Else
                                    injectedProducts = injectrate * (enteringProducts - rejectedProducts)
                                    statsCounts(enumType.Inject) += injectedProducts
                                End If
                            End If

                        Next 'next link to be processed

                        'Put the products in internal accumulator
                        Me.Content_Accu.Put(enteringProducts - rejectedProducts)
                        statsCounts(enumType.Processed) += enteringProducts - rejectedProducts

                        upstreamwait = False 'All links have products
                    End If

                End If



                'Manage downstream backup
                Dim downstreamwait As Boolean = True
                For Each one_outputlink As clsLink In Outputs
                    If Not one_outputlink.state = clsLink.enumStates.Full Then downstreamwait = False
                Next

                'Manage stats timebase
                If upstreamwait Then
                    If Not statsState.ContainsKey(enumStates.WaitingInput) Then statsState.Add(enumStates.WaitingInput, 0)
                    statsState(enumStates.WaitingInput) += TIMEBASE
                ElseIf downstreamwait Then
                    If Not statsState.ContainsKey(enumStates.WaitingOutput) Then statsState.Add(enumStates.WaitingOutput, 0)
                    statsState(enumStates.WaitingOutput) += TIMEBASE
                Else
                    If Not statsState.ContainsKey(enumStates.Running) Then statsState.Add(enumStates.Running, 0)
                    statsState(enumStates.Running) += TIMEBASE
                    If MTBF < MAX_TMBF And Not MTTR = 0 Then MTBF_next -= TIMEBASE
                End If

            Else 'Equipment if stopped
                If Not statsState.ContainsKey(enumStates.Stopped) Then statsState.Add(enumStates.Stopped, 0)

                statsState(enumStates.Stopped) += TIMEBASE
                If MTBF < MAX_TMBF And Not MTTR = 0 Then MTTR_next -= TIMEBASE
            End If



        End Sub

        Public Sub details(Optional ByVal _infoByType As Boolean = False, Optional ByVal _bufferOnly As Boolean = False, Optional ByVal _counterOnly As Boolean = False, Optional ByVal _advancedResults As Boolean = False)

            If _infoByType Then
                Select Case modType
                    Case enumModularType.Input, enumModularType.Output
                        _counterOnly = True
                    Case enumModularType.Transport
                        _bufferOnly = True
                End Select
            End If
            If _advancedResults Then
                _counterOnly = False
                _bufferOnly = False
            End If

            Console.Write(name & " (speed: " & displaySpeed(speed) & ")")
            If MTBF_next <= 0 Then Console.Write(" [OWN STOP for " & toTimeDuration(MTTR_next) & "]")
            If MTBF_next > 0 And MTBF_next < MAX_TMBF Then Console.Write(" [RUNNING for " & toTimeDuration(MTBF_next) & "]")
            Console.WriteLine()

            If Content_Max > 0 Then Console.WriteLine(" - content: " & displayDecimal(Content) & " (entering=" & displayDecimal(Content_entering) & ")/" & displayDecimal(Content_Max))

            If Not (_bufferOnly Or _counterOnly) Then
                Console.WriteLine(" - upstream: ")
                For Each one_inputlink As clsLink In Inputs
                    Console.Write(" - - " & one_inputlink.upstream.name & "[")
                    Select Case one_inputlink.state
                        Case clsLink.enumStates.Empty
                            Console.Write("EMPTY")
                        Case clsLink.enumStates.Normal
                            Console.Write("OK")
                        Case clsLink.enumStates.Full
                            Console.Write("FULL")
                    End Select
                    Console.WriteLine("] (" & displaySpeed(one_inputlink.potential) & "),")
                Next

                Console.WriteLine(" - outstream: ")
                For Each one_outputlink As clsLink In Outputs
                    Console.Write(" - - " & one_outputlink.downstream.name & "[")
                    Select Case one_outputlink.state
                        Case clsLink.enumStates.Empty
                            Console.Write("EMPTY")
                        Case clsLink.enumStates.Normal
                            Console.Write("OK")
                        Case clsLink.enumStates.Full
                            Console.Write("FULL")
                    End Select
                    Console.WriteLine("] (" & displaySpeed(one_outputlink.potential) & "),")
                Next
            End If

            If Not (_bufferOnly) Then
                Console.WriteLine(" - counters: ")
                For Each oneCounter As enumType In statsCounts.Keys
                    Select Case oneCounter
                        Case enumType.Processed
                            Console.Write(" - - processed = ")
                        Case enumType.Inject
                            Console.Write(" - - injects = ")
                        Case enumType.Reject
                            Console.Write(" - - rejects = ")
                    End Select
                    Console.WriteLine(displayDecimal(statsCounts(oneCounter)))
                Next
            End If

            Dim totalTime As Decimal = 0
            For Each oneState As enumStates In statsState.Keys
                totalTime += statsState(oneState)
            Next

            If Not (_bufferOnly Or _counterOnly) Then
                Console.WriteLine(" - states: ")
                For Each oneState As enumStates In statsState.Keys
                    Select Case oneState
                        Case enumStates.Running
                            Console.Write(" - - running = ")
                        Case enumStates.Stopped
                            Console.Write(" - - stopped = ")
                        Case enumStates.WaitingInput
                            Console.Write(" - - waiting input = ")
                        Case enumStates.WaitingOutput
                            Console.Write(" - - waiting output = ")
                    End Select
                    Console.WriteLine(toTimeDuration(statsState(oneState)) & " [" & displayDecimal(100 * statsState(oneState) / totalTime) & " %]")
                Next
            End If

            If _advancedResults Then
                Console.WriteLine(" - indicators: ")
                Dim opAvailab As Decimal = totalTime
                If statsState.ContainsKey(enumStates.WaitingInput) Then opAvailab -= statsState(enumStates.WaitingInput)
                If statsState.ContainsKey(enumStates.WaitingOutput) Then opAvailab -= statsState(enumStates.WaitingOutput)
                If statsCounts.ContainsKey(enumType.Processed) And speed < MAX_SPEED Then
                    Dim goodProd As Decimal = statsCounts(enumType.Processed)
                    If statsCounts.ContainsKey(enumType.Reject) Then goodProd -= statsCounts(enumType.Reject)
                    If totalTime > 0 And speed > 0 Then Console.WriteLine(" - - OEE = " & displayDecimal(100 * goodProd / (totalTime * speed)) & " %")
                    If statsCounts(enumType.Processed) > 0 Then Console.WriteLine(" - - × Quality = " & displayDecimal(100 * goodProd / statsCounts(enumType.Processed)) & " %")
                    If opAvailab > 0 And speed > 0 Then Console.WriteLine(" - - × Eficiency = " & displayDecimal(100 * statsCounts(enumType.Processed) / (opAvailab * speed)) & " %")
                End If
                Console.WriteLine(" - - × Operational availability = " & displayDecimal(100 * opAvailab / totalTime) & " %")
                If statsState.ContainsKey(enumStates.Running) Then
                    If statsCounts.ContainsKey(enumType.Processed) And speed < MAX_SPEED Then
                        If statsCounts(enumType.Processed) > 0 And speed > 0 Then
                            Console.WriteLine(" - - Performance = " & displayDecimal(100 * statsCounts(enumType.Processed) / (statsState(enumStates.Running) * speed)) & " %")
                        End If
                    End If
                    If opAvailab > 0 Then Console.WriteLine(" - - Reliability = " & displayDecimal(100 * statsState(enumStates.Running) / opAvailab) & " %")
                End If
                If statsState.ContainsKey(enumStates.Stopped) Then
                    'Console.WriteLine("MTTR")
                    'ShowDistributions(statsMTTR)
                    'Console.WriteLine("MTBF")
                    'ShowDistributions(statsMTBF)
                End If
            End If

            Console.WriteLine()


        End Sub

        ''' <summary>
        ''' Get or Change number of entering products
        ''' </summary>
        ''' <value>Numer of products to put inside</value>
        ''' <returns>Numer of products entering (not availables)</returns>
        ''' <remarks></remarks>
        Public Property Content_entering() As Decimal
            Get
                Return Content_Accu.EnteringContent
            End Get
            Set(ByVal value As Decimal)
                Content_Accu.Put(value)
            End Set
        End Property

        ''' <summary>
        ''' Get total content inside the equipment
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Content() As Decimal
            Get
                Return Content_Accu.TotalContent
            End Get
        End Property

        ''' <summary>
        ''' Get or Change max content of the equipment
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Content_Max() As Decimal
            Get
                Return Content_Accu.Max_Content
            End Get
            Set(ByVal _content_max As Decimal)
                Content_Accu.Max_Content = Math.Max(_content_max, IIf(Me.speed < MAX_SPEED, Me.speed, 0))
            End Set
        End Property

        ''' <summary>
        ''' Set up an accumulator for an equipment with an High quantity of products inside
        ''' </summary>
        ''' <param name="_accumulated_content">Accumulation capacity of the equipment (total capacity - minimum for transit)</param>
        ''' <param name="_transit_time">Transit time from infeed to outfeed at normal speed</param>
        ''' <param name="_speed">Transit speed in products per second (to define minimum quantity for transit)</param>
        ''' <remarks></remarks>
        Public Sub SetAccumulator(ByVal _accumulated_content As Decimal, ByVal _transit_time As Integer, ByVal _speed As Decimal)
            Content_Accu = New clsAccu(_accumulated_content + _transit_time * _speed, _transit_time, Me)
        End Sub

        Public ReadOnly Property OutputPotential() As Decimal
            Get
                Return Content_Accu.OutputPotential()
            End Get
        End Property

        Public ReadOnly Property InputPotential() As Decimal
            Get
                Return Content_Accu.InputPotential()
            End Get
        End Property

        ''' <summary>
        ''' Run the accumulator between each turn
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub RunAccumulator()
            Content_Accu.init()
        End Sub

    End Class

    Public Sub initDistributions()

        Randomize()

        Dim variance As Long = 4
        Dim mean As Decimal = 0

        'Filler
        While Math.Abs((mean - 1)) > 0.005
            ReDim distriFiller(92)
            distriFiller(0) = 0
            For i = 1 To 18
                distriFiller(i) = NormalRandomKM(0.2311557789, 0.2311557789 / variance)
            Next
            For i = 19 To 61
                distriFiller(i) = NormalRandomKM(0.6934673367, (0.6934673367 - 0.2311557789) / variance)
            Next
            For i = 62 To 75
                distriFiller(i) = NormalRandomKM(1.1557788945, (1.1557788945 - 0.6934673367) / variance)
            Next
            For i = 76 To 79
                distriFiller(i) = NormalRandomKM(1.6180904523, (1.6180904523 - 1.1557788945) / variance)
            Next
            For i = 80 To 85
                distriFiller(i) = NormalRandomKM(2.0804020101, (2.0804020101 - 1.6180904523) / variance)
            Next
            For i = 86 To 89
                distriFiller(i) = NormalRandomKM(2.5427135678, (2.5427135678 - 2.0804020101) / variance)
            Next
            distriFiller(90) = 3.4673366834
            distriFiller(91) = 4.391959799
            distriFiller(92) = 4.8542713568

            mean = 0
            For i = 1 To 92
                mean += distriFiller(i)
            Next
            mean = mean / 92
            'Console.WriteLine("Generated mean = " & mean)
        End While

    End Sub

    Public Function displaySpeed(ByVal _input As Decimal) As String


        If _input > MAX_SPEED Then
            Return "MAX"
        Else
            Return displayDecimal(_input * 3600, 0) & " UPH"
        End If


    End Function

    Public Function displayDecimal(ByVal _input As Decimal, Optional ByVal _numDecimal As Integer = NUMDECIMAL)

        Return Decimal.Round(_input, _numDecimal)

    End Function

    Public Function toTimeDuration(ByVal _input As Decimal) As String
        Dim seconds As Integer = _input
        Dim hours As Integer = Math.DivRem(seconds, 3600, seconds)
        Dim min As Integer = Math.DivRem(seconds, 60, seconds)
        Dim result As String = ""
        If hours > 0 Then result &= hours & "H "
        If min > 0 Then result &= min & "M "
        result &= seconds & "s"
        Return result
    End Function

    Public Function NormalRandomKM(Optional ByVal Esperance As Double = 0, _
                               Optional ByVal EcartType As Double = 1, _
                               Optional ByVal ForceRandomize As Boolean = False) As Double
        'Générateur de nombres pseudo-aléatoires à distribution normale  : Ratio method (Kinderman - Monahan)
        'Implementation en VBA par Philben - v1.0 - Free to use
        Const c As Double = 1.71552776992141 + 0.00000000000000359296   'sqrt(8/e)
        Static isRandomize As Boolean
        Dim u As Double, v As Double, x As Double

        If Not isRandomize Or ForceRandomize Then
            Randomize()
            isRandomize = True
        End If

        Do
            v = Rnd()
            Do : u = Rnd() : Loop While u = 0
            x = (v - 0.5) * c / u
        Loop Until (x * x < -4 * Math.Log(u))

        NormalRandomKM = Esperance + (x * EcartType)
    End Function

    Public Function NormalMTTR(Optional ByVal Esperance As Double = 0) As Double
        If RealisticDistributions Then
            Return RealisticMTTR(Esperance)
        Else
            Return SimplifiedMTTR(Esperance)
        End If
    End Function

    Public Function SimplifiedMTTR(Optional ByVal _Esperance As Double = 0) As Double
        'Using the LogNormal distribution
        Dim _Deviation As Double = _Esperance * 10

        Dim location As Decimal = Math.Log((_Esperance * _Esperance) / (Math.Sqrt((_Deviation + (_Esperance * _Esperance)))))
        Dim scale As Double = Math.Sqrt(Math.Log(1 + (_Deviation / (_Esperance * _Esperance))))

        SimplifiedMTTR = Math.Exp(location + scale * NormalRandomKM())

    End Function

    Public Function RealisticMTTR(Optional ByVal Esperance As Double = 0) As Double
        'Using a feedback by experience distribution
        RealisticMTTR = distriFiller(Math.Ceiling(Rnd() * (UBound(distriFiller) - EPSILON))) * Esperance
    End Function

    Public Function NormalMTBF(Optional ByVal Esperance As Double = 0) As Double
        If RealisticDistributions Then
            Return RealisticMTBF(Esperance)
        Else
            Return SimplifiedMTBF(Esperance)
        End If
    End Function

    Public Function SimplifiedMTBF(Optional ByVal Esperance As Double = 0) As Double
        'Using the Normal distribution
        Return NormalRandomKM(Esperance, Esperance)
    End Function

    Public Function RealisticMTBF(Optional ByVal Esperance As Double = 0) As Double
        'Using a rectangular distribution
        '' http://metgen.pagesperso-orange.fr/metrologiefr24.htm

        Dim a As Double = 0.2 * Esperance
        Dim c As Double = 0.4 * Esperance
        Dim d As Double = 1.6 * Esperance
        Dim b As Double = 1.8 * Esperance

        Dim h As Double = 2 / (d - a + b - c)

        Dim y As Double = Rnd()

        Dim int1 As Double = (h / 2) * (c - a)
        Dim int2 As Double = 1 - (h / 2) * (b - d)

        If y < int1 Then
            RealisticMTBF = a + Math.Sqrt(2 * (c - a) / h) * Math.Sqrt(y)
        ElseIf y >= int1 And y < int2 Then
            RealisticMTBF = (a + c) / 2 + y / h
        Else
            RealisticMTBF = b - Math.Sqrt(2 * (b - d) / h) * Math.Sqrt(1 - y)
        End If

    End Function

End Module
Imports System.Diagnostics.Eventing.Reader

Module Module1

    Dim rx As System.Text.RegularExpressions.Regex

    Public bVerbose As Boolean = False

    Sub Main()

        Dim sArg() As String = Environment.GetCommandLineArgs

        If UBound(sArg) = 0 Then
            Console.ForegroundColor = ConsoleColor.Cyan
            Console.WriteLine("ETLDump, written in 2021 by Glenn Larsson.")
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("-f  File to read")
            Console.WriteLine("-w  File to write")
            Console.WriteLine("-v  Verbose")
            Console.WriteLine("-e  Dump only eventpayload (if any)")
            Console.ForegroundColor = ConsoleColor.White
            Console.WriteLine("Example: ETLDump.exe -fc:\folder\file.etl -wc:\folder.xml -v")
            End
        End If

        Dim sFilename As String = ""
        Dim sFilenameOut As String = ""
        Dim sOperator As String = ""
        Dim bEventPayload As Boolean = False

        Dim param As String = ""
        Try
            For n = 0 To UBound(sArg)
                param = Microsoft.VisualBasic.Left(sArg(n), 2)
                Select Case LCase(param)
                    Case "-f" ' File to process
                        sFilename = sArg(n).Replace(param, "").ToString
                    Case "-v" ' Verbose
                        bVerbose = True
                    Case "-w" ' Write output file
                        sFilenameOut = sArg(n).Replace(param, "").ToString
                    Case "-e"
                        bEventPayload = True
                End Select
            Next
        Catch ex As Exception
        End Try

        If sFilename = "" Then
            Console.WriteLine("Need to enter a filename.")
            End
        End If

        If System.IO.File.Exists(sFilename) = False Then
            Console.WriteLine("File " & sFilename & " does not exist.")
            End
        End If

        Dim fl As New System.IO.FileInfo(sFilename)
        If fl.Length = 0 Then
            Console.WriteLine("File " & sFilename & " is 0 length - or locked. Cant continue.")
            End
        End If
        fl = Nothing

        Dim sResult As String = ""

        Dim evtReader As New System.Diagnostics.Eventing.Reader.EventLogReader(sFilename, PathType.FilePath)
        Dim evtRecord As EventRecord
        Dim bKeepReading As Boolean = True
        Dim sTemp As String = ""

        While (bKeepReading)
            Try
                evtRecord = evtReader.ReadEvent
                sTemp = evtRecord.ToXml
                If bEventPayload = True Then
                    sTemp = rx.Replace(sTemp, "\<\/EventPayload.*", "")
                    sTemp = rx.Replace(sTemp, ".*<EventPayload>", "")
                End If
                sResult = sResult & sTemp & vbCrLf
                If bVerbose = True Or sFilenameOut = "" Then
                    Console.WriteLine(sTemp)
                End If
            Catch ex As Exception
                bKeepReading = False
            End Try
        End While

        Try
            System.IO.File.WriteAllText(sFilenameOut, sResult)
            Console.WriteLine("Successfully wrote XML data to " & sFilenameOut)
        Catch ex As Exception
        End Try

    End Sub

End Module

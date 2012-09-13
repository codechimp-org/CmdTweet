Module Module1
    Sub Main()

        Const CONSUMER_KEY As String = "YOURKEY"
        Const CONSUMER_KEY_SECRET As String = "YOURKEYSECRET"

        Dim tw As New TwitterVB2.TwitterAPI


        If My.Application.CommandLineArgs.Count = 0 Then
            DisplayHelp()
        Else
            Select Case My.Application.CommandLineArgs(0).ToLower
                Case "/?"
                    DisplayHelp()

                Case "auth" 'show authorise page
                    Dim authURL As String
                    authURL = tw.GetAuthorizationLink(CONSUMER_KEY, CONSUMER_KEY_SECRET)


                    Console.WriteLine(authURL)

                    Process.Start(authURL)

                    Console.WriteLine("Enter Pin")

                    Dim pin As String
                    pin = Console.ReadLine

                    If tw.ValidatePIN(pin) Then
                        My.Settings.OAuthToken = tw.OAuth_Token
                        My.Settings.OAuthTokenSecret = tw.OAuth_TokenSecret

                        My.Settings.Save()

                        Console.WriteLine("Pin Validated")

                        Console.WriteLine("Token: " & tw.OAuth_Token)
                        Console.WriteLine("TokenSecret: " & tw.OAuth_TokenSecret)

                    Else
                        Console.WriteLine("Pin Validation Failed")
                    End If

                Case "update"
                    Try
                        tw.AuthenticateWith(CONSUMER_KEY, CONSUMER_KEY_SECRET, My.Settings.OAuthToken, My.Settings.OAuthTokenSecret)

                        Dim updateMsg As String = ""

                        For i As Integer = 1 To My.Application.CommandLineArgs.Count - 1
                            updateMsg += " " + My.Application.CommandLineArgs(i)
                        Next

                        'Remove any leading/trailing space
                        updateMsg = updateMsg.Trim

                        'Trim down to 140 chars
                        If updateMsg.Length > 140 Then updateMsg = updateMsg.Substring(0, 140)

                        Dim success As Boolean = False
                        Dim tries As Integer = 0
                        While Not success
                            Try

                                tw.Update(updateMsg)

                                success = True
                            Catch ex As Exception
                                tries += 1

                                If tries < 3 Then
                                    System.Threading.Thread.Sleep(500) '5 Seconds
                                Else
                                    success = True
                                    Throw
                                End If
                            End Try
                        End While


                        Console.WriteLine("Tweet: " & updateMsg)
                        Logging.WriteToEventLog("Tweet: " & updateMsg, "CmdTweet", EventLogEntryType.Information)
                    Catch ex As Exception
                        Console.WriteLine("An error occured trying to update")
                        Console.WriteLine(ex.Message)
                        Console.WriteLine(ex.InnerException.Message)

                        Logging.WriteToEventLog(ex.Message & ControlChars.NewLine & ex.InnerException.Message, "CmdTweet", EventLogEntryType.Error)
                    End Try

            End Select


#If CONFIG = "Debug" Then
            'Await user to press a key before exiting
            Console.ReadLine()
#End If

        End If
    End Sub

    Private Function CheckOAuth() As Boolean
        Dim ret As Boolean = False

        If My.Settings.OAuthToken.Length > 0 Then
            ret = True
        End If

        Return ret
    End Function

    Private Sub DisplayHelp()
        Console.WriteLine("CmdTweet Help")
        Console.WriteLine("auth - Starts the authentication process")
        Console.WriteLine("update message - Posts a new tweet")
    End Sub
End Module

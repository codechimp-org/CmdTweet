Public Class Logging

    Public Shared Function WriteToEventLog(ByVal Entry As String, _
       Optional ByVal AppName As String = "VB.NET Application", _
       Optional ByVal EventType As  _
       EventLogEntryType = EventLogEntryType.Information, _
       Optional ByVal LogName As String = "Application") As Boolean

        '*************************************************************
        'PURPOSE: Write Entry to Event Log using VB.NET
        'PARAMETERS: Entry - Value to Write
        '            AppName - Name of Client Application. Needed 
        '              because before writing to event log, you must 
        '              have a named EventLog source. 
        '            EventType - Entry Type, from EventLogEntryType 
        '              Structure e.g., EventLogEntryType.Warning, 
        '              EventLogEntryType.Error
        '            LogName: Name of Log (System, Application; 
        '              Security is read-only) If you 
        '              specify a non-existent log, the log will be
        '              created

        'RETURNS:   True if successful, false if not

        'EXAMPLES: 
        '1. Simple Example, Accepting All Defaults
        '    WriteToEventLog "Hello Event Log"

        '2.  Specify EventSource, EventType, and LogName
        '    WriteToEventLog("Danger, Danger, Danger", "MyVbApp", _
        '                      EventLogEntryType.Warning, "System")
        '
        'NOTE:     EventSources are tightly tied to their log. 
        '          So don't use the same source name for different 
        '          logs, and vice versa
        '******************************************************

        Dim objEventLog As New EventLog()

        Try
            'Register the App as an Event Source
            If Not EventLog.SourceExists(AppName) Then

                EventLog.CreateEventSource(AppName, LogName)
            End If

            objEventLog.Source = AppName

            'WriteEntry is overloaded; this is one
            'of 10 ways to call it
            objEventLog.WriteEntry(Entry, EventType)
            Return True
        Catch Ex As Exception
            Return False

        End Try

    End Function
End Class

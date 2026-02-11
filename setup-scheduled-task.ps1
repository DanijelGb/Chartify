# Create scheduled task for daily ingestion
$taskName = "Chartify Daily Ingestion"
$exePath = "C:\Users\DaciBaci\Desktop\Chartify\ingestion-publish\Chartify.Ingestion.exe"
$workingDir = "C:\Users\DaciBaci\Desktop\Chartify\ingestion-publish"

# Create the action
$action = New-ScheduledTaskAction -Execute $exePath -WorkingDirectory $workingDir

# Create the trigger for 1 AM daily
$trigger = New-ScheduledTaskTrigger -Daily -At 1am

# Register the task
Register-ScheduledTask -TaskName $taskName -Action $action -Trigger $trigger -RunLevel Highest -Force

Write-Host "Scheduled task '$taskName' created successfully"
Write-Host "The ingestion will run daily at 1 AM"
